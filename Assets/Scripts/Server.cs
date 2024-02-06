using NetworkMessages;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using Unity.Jobs;
using System;
using NetworkObject;
using GetGunsScript;
using System.Security.Cryptography;
using System.Text;
using AYellowpaper.SerializedCollections;

public class Server : MonoBehaviour
{
    [Header("Network Settings")]
    public NetworkDriver m_Driver;
    public ushort serverPort;
    private NativeList<NetworkConnection> m_Connections;
    public List<NetworkObject.NetworkPlayer> m_Players;
    //private static int nextId = 0;
    public static NetworkPipeline pipeline;
    JobHandle m_ServerJobHandle;

    [Header("Player variables")]
    private GetGuns getGuns = new GetGuns();
    public GameObject playerPrefab;
    [SerializedDictionary("id", "player")]
    public SerializedDictionary<string, GameObject> simulatedPlayers;
    public Dictionary<GameObject, string> simulatedPlayersInverse;
    public List<Transform> spawnPoints;
    //This code is cringe
    public Dictionary<string, NetworkConnection> playerConnection;
    public List<GameObject> guns;
    

    [Header("Bullet hole pool")]
    public GameObject bulletHolePrefab;
    public int bulletHolePoolSize;
    public List<GameObject> bulletHolePool;

    [Header("Items list")]
    public List<GameObject> itemsList;

    void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        var endpoint = NetworkEndpoint.AnyIpv4.WithPort(serverPort);
        simulatedPlayers = new SerializedDictionary<string, GameObject>();
        simulatedPlayersInverse = new Dictionary<GameObject, string>();
        playerConnection = new Dictionary<string, NetworkConnection>();
        pipeline = m_Driver.CreatePipeline(typeof(FragmentationPipelineStage),
            typeof(ReliableSequencedPipelineStage));

        getGuns.FetchGuns();

        for (int i = 0; i < bulletHolePoolSize; i++)
        {
            var auxBulletHole = Instantiate(bulletHolePrefab);
            auxBulletHole.SetActive(false);
            bulletHolePool.Add(auxBulletHole);
        }

        foreach (var item in itemsList)
        {
            item.GetComponent<ItemScript>().itemId = itemsList.IndexOf(item);
        }

        StartCoroutine(DestroyBulletHole());

        if (m_Driver.Bind(endpoint) != 0)
        {
            Debug.Log("Failed to bind to port " + serverPort);
        }
        else 
        {
            m_Driver.Listen();
        }
    }

    void OnDestroy()
    {
        if (m_Driver.IsCreated)
        {
            m_ServerJobHandle.Complete();
            m_Driver.Dispose();
            m_Connections.Dispose();
        }
    }

    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        NetworkConnection c = m_Driver.Accept();
        while (c != default(NetworkConnection))
        {
            OnConnect(c);
            c = m_Driver.Accept();
        }

        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream);
            while (cmd != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    OnData(stream, i);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    //Might need to handle when a player crashes on join
                    m_Driver.Disconnect(m_Connections[i]);
                    m_Connections.RemoveAtSwapBack(i);
                    var idDisconnect = m_Players[i].id;
                    m_Players.RemoveAt(i);
                    var playerAux = simulatedPlayers[idDisconnect];
                    simulatedPlayers.Remove(idDisconnect);
                    simulatedPlayersInverse.Remove(playerAux);
                    Destroy(playerAux);
                    PlayerDisconnectMsg pDisconnectMsg = new PlayerDisconnectMsg();
                    pDisconnectMsg.id = idDisconnect;
                    playerConnection.Remove(idDisconnect);
                    cmd = NetworkEvent.Type.Empty;
                    SendToAllClients(JsonUtility.ToJson(pDisconnectMsg));
                    i--;
                    continue;
                }
                if (m_Connections[i] != null)
                {
                    cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream);
                }
            }
        }
    }
    void FixedUpdate()
    {        
        //welcome to hell, population unoptimized code
        foreach (var playerKvp in simulatedPlayers)
        {
            var player = playerKvp.Value;
            var playerId = playerKvp.Key;
            var playerScript = player.GetComponent<PlayerScript>();


            if (playerScript.health <= 0 && !playerScript.dead)
            {
                playerScript.dead = true;
                playerScript.health = 100;
                playerScript.timeUntilRespawn = 5f;
                playerScript.RestoreAmmo();
                player.GetComponent<CapsuleCollider>().enabled = false;
                player.GetComponent<Rigidbody>().useGravity = false;
                int randomSpawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count - 1);
                player.transform.position = spawnPoints[randomSpawnIndex].position;
                PlayerKillMsg pKillMsg = new PlayerKillMsg();
                pKillMsg.id = playerId;
                SendToAllClients(JsonUtility.ToJson(pKillMsg));
            }

            if (playerScript.timeUntilRespawn == 0f && playerScript.dead)
            {
                player.GetComponent<CapsuleCollider>().enabled = true;
                player.GetComponent<Rigidbody>().useGravity = true;
                player.GetComponent<PlayerScript>().dead = false;
                PlayerRespawnMsg pRespawnMsg = new PlayerRespawnMsg();
                pRespawnMsg.id = playerKvp.Key;
                Debug.Log(pRespawnMsg.id);
                SendToAllClients(JsonUtility.ToJson(pRespawnMsg));
            }

            PlayerInventoryMsg pInventoryMsg = new PlayerInventoryMsg();
            pInventoryMsg.id = playerId;
            var playerScriptAux = player.GetComponent<PlayerScript>();
            pInventoryMsg.gunIndex = playerScriptAux.activeGunIndex;
            if (playerScriptAux.activeGun.transform.TryGetComponent<GunScript>(out GunScript gunScript))
            {
                pInventoryMsg.ammoCount = gunScript.ammoCount;
                pInventoryMsg.ammoInMag = gunScript.ammoInMag;
            }
            pInventoryMsg.health = player.GetComponent<PlayerScript>().health;

            SendToClient(JsonUtility.ToJson(pInventoryMsg), playerConnection[playerId]);
        }
        
    }

    private void OnData(DataStreamReader stream, int numJugador)
    {
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = System.Text.Encoding.ASCII.GetString(bytes);
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch (header.command)
        {
            case Commands.HANDSHAKE:
                HandshakeMsg hsMsg = JsonUtility.FromJson<HandshakeMsg>(recMsg);
                Debug.Log("Handshake message received from " + hsMsg.player.nombre);

                NetworkObject.NetworkPlayer player = new NetworkObject.NetworkPlayer();
                player.id = hsMsg.player.id;
                player.nombre = hsMsg.player.nombre;
                player.arrGuns = hsMsg.player.arrGuns;
                m_Players.Add(player);
                Debug.Log(m_Players.Count + " players connected");

                playerConnection.Add(hsMsg.player.id, m_Connections[numJugador]);

                //Message to spawn new player
                PlayerSpawnMsg pSpawnMsg = new PlayerSpawnMsg();
                int spawnRandomIndex = UnityEngine.Random.Range(0, spawnPoints.Count - 1);
                pSpawnMsg.id = hsMsg.player.id;
                var playerAux = Instantiate(playerPrefab, spawnPoints[spawnRandomIndex].position, Quaternion.identity);
                playerAux.GetComponent<PlayerScript>().LoadLoadOut(GetLoadOut(hsMsg.player.arrGuns));
                playerAux.GetComponent<PlayerScript>().playerId = pSpawnMsg.id;
                pSpawnMsg.spawnPlayer.pos = playerAux.transform.position;
                pSpawnMsg.spawnPlayer.arrGuns = hsMsg.player.arrGuns;
                playerAux.transform.name = hsMsg.player.nombre;
                foreach (var connection in m_Connections)
                {
                    if (connection != m_Connections[numJugador])
                    {
                        SendToClient(JsonUtility.ToJson(pSpawnMsg), connection);
                    }
                }
                simulatedPlayers.Add(hsMsg.player.id, playerAux);
                simulatedPlayersInverse.Add(playerAux, hsMsg.player.id);

                //Menssage to new player
                PlayerJoinMsg playerJoinMsg = new PlayerJoinMsg();
                playerJoinMsg.id = hsMsg.player.id;
                var auxPlayers = new List<NetworkObject.NetworkPlayer>();
                var auxItems = new List<NetworkObject.NetworkItem>();

                foreach (var auxPlayer in m_Players)
                {
                    //Why am i like this
                    var auxSimulatedPlayer3 = FindPlayerById(auxPlayer.id);
                    if (auxSimulatedPlayer3 == null)
                    {
                        break;
                    }
                    auxPlayer.activeGunIndex = auxSimulatedPlayer3.GetComponent<PlayerScript>().activeGunIndex;
                    auxPlayer.isDead = auxSimulatedPlayer3.GetComponent<PlayerScript>().dead;
                    auxPlayers.Add(auxPlayer);
                }

                foreach (var item in itemsList)
                {
                    var auxItem = new NetworkObject.NetworkItem();
                    auxItem.itemId = itemsList.IndexOf(item);
                    auxItem.enabled = item.activeSelf;
                    auxItems.Add(auxItem);
                }
                playerJoinMsg.playersList = auxPlayers;
                playerJoinMsg.itemList = auxItems;
                SendToClient(JsonUtility.ToJson(playerJoinMsg), m_Connections[numJugador]);
                break;
            case Commands.PLAYER_INPUT:
                PlayerInputMsg pInputMsg = JsonUtility.FromJson<PlayerInputMsg>(recMsg);
                var playerAux2 = FindPlayerById(pInputMsg.id);
                if (playerAux2 == null)
                {
                    return;
                }
                playerAux2.GetComponent<PlayerScript>().UpdateMovementVariables(pInputMsg);
                

                PlayerPosMsg pPosMsg = new PlayerPosMsg();
                pPosMsg.id = simulatedPlayersInverse[playerAux2];
                pPosMsg.pos.position = playerAux2.transform.position;
                pPosMsg.pos.rotation = playerAux2.transform.rotation;
                SendToAllClients(JsonUtility.ToJson(pPosMsg));
                break;
            case Commands.PLAYER_CAM_MOV:
                PlayerMoveCamera pCamMovMsg = JsonUtility.FromJson<PlayerMoveCamera>(recMsg);
                var playerAux6 = FindPlayerById(pCamMovMsg.id);
                if (playerAux6 == null)
                {
                    return;
                }
                playerAux6.GetComponentInChildren<CameraScript>().mouseX = pCamMovMsg.mouseX;
                playerAux6.GetComponentInChildren<CameraScript>().mouseY = pCamMovMsg.mouseY;

                PlayerCameraRotationMsg pCamRotMsg = new PlayerCameraRotationMsg();
                pCamRotMsg.id = pCamMovMsg.id;
                var eulerRot = playerAux6.transform.GetChild(0).transform.localEulerAngles;
                pCamRotMsg.rotation = new Vector3(eulerRot.x, eulerRot.y, playerAux6.transform.localEulerAngles.z);
                SendToAllClients(JsonUtility.ToJson(pCamRotMsg));
                break;
            case Commands.PLAYER_JUMP:
                PlayerJumpMsg pJumpMsg = JsonUtility.FromJson<PlayerJumpMsg>(recMsg);
                var auxSimulatedPlayer = FindPlayerById(pJumpMsg.id);
                if (auxSimulatedPlayer == null)
                {
                    return;
                }
                auxSimulatedPlayer.GetComponent<PlayerScript>().jumpInput = true;
                break;
            case Commands.PLAYER_SWITCH_GUN:
                PlayerSwitchGunMsg pSwitchGunMsg = JsonUtility.FromJson<PlayerSwitchGunMsg>(recMsg);
                var auxSimulatedPlayer2 = FindPlayerById(pSwitchGunMsg.id);
                if (auxSimulatedPlayer2 == null)
                {
                    return;
                }
                var playerScriptAux = auxSimulatedPlayer2.GetComponent<PlayerScript>();
                playerScriptAux.SwitchGun(pSwitchGunMsg);

                PlayerSwitchGunClient pSwitchGunMsgSend = new PlayerSwitchGunClient();
                pSwitchGunMsgSend.id = pSwitchGunMsg.id;
                pSwitchGunMsgSend.gunIndex = playerScriptAux.activeGunIndex;
                SendToAllClients(JsonUtility.ToJson(pSwitchGunMsgSend));
                break;
            default:
                break;
        }
    }

    public GameObject[] GetLoadOut(short[] gunIds)
    {
        GameObject[] gunsArr = new GameObject[gunIds.Length];
        for (int i = 0; i < gunIds.Length; i++)
        {
            gunsArr[i] = getGuns.GetGun(gunIds[i]).GetPrefabServer();
        }

        return gunsArr;
    }

    public GameObject FindPlayerById(string idJugador)
    {
        simulatedPlayers.TryGetValue(idJugador, out GameObject jugador);
        return jugador;
    }

    void OnConnect(NetworkConnection c)
    {
        m_Connections.Add(c);
        Debug.Log("Accepted a connection");

        HandshakeMsg m = new HandshakeMsg();
        m.player.id = Guid.NewGuid().ToString();
        SendToClient(JsonUtility.ToJson(m), c);
    }

    private void SendToClient(string message, NetworkConnection c)
    {
        if (!c.IsCreated)
        {
            return;
        }

        var encrypMsg = Aes256CbcEncrypter.EncryptString(message);

        DataStreamWriter writer;
        m_Driver.BeginSend(pipeline, c, out writer);
        if (!writer.IsCreated)
        {
            return;
        }
        NativeArray<byte> bytes = new NativeArray<byte>(System.Text.Encoding.ASCII.GetBytes(encrypMsg), Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }

    private void SendToAllClients(string message)
    {
        foreach (var connection in m_Connections)
        {
            SendToClient(message, connection);
        }
    }

    public void CreateBulletHole(RaycastHit hit)
    {
        var bulletHole = bulletHolePool.Find(x => !x.activeSelf);
        if (bulletHole != null)
        {
            bulletHole.transform.position = hit.point + (hit.normal * .01f);
            bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            bulletHole.SetActive(true);
            CreateBulletHoleMsg bHoleMsg = new CreateBulletHoleMsg();
            bHoleMsg.hit.position = bulletHole.transform.position;
            bHoleMsg.hit.rotation = bulletHole.transform.rotation;
            SendToAllClients(JsonUtility.ToJson(bHoleMsg));
        }
    }

    public IEnumerator DestroyBulletHole()
    {
        while (true)
        {
            foreach (var bulletHole in bulletHolePool)
            {
                if (bulletHole.activeSelf)
                {
                    bulletHole.SetActive(false);
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }

    /*For the love of god move this to the player script it doesen't make sense it being here
     */
    public void SendPlayerAnimation(string playerId, NetworkAnimation animationVariables)
    {
        PlayerAnimationMsg pAnimationMsg = new PlayerAnimationMsg();
        pAnimationMsg.id = playerId;
        pAnimationMsg.animation = animationVariables;
        SendToAllClients(JsonUtility.ToJson(pAnimationMsg));
    }

    public void SendItemStateChange(int itemId)
    {
        StateItemMsg stateItemMsg = new StateItemMsg();
        stateItemMsg.itemId = itemId;
        stateItemMsg.enabled = itemsList[itemId].activeSelf;
        SendToAllClients(JsonUtility.ToJson(stateItemMsg));

        var coroutine = itemsList[itemId].GetComponent<ItemScript>().RespawnItem();

        if (!itemsList[itemId].activeSelf) 
        {
            StartCoroutine(coroutine);
        } else
        {
            StopCoroutine(coroutine);
        }
    }

}
