using GetGunsScript;
using NetworkMessages;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetworkClient : MonoBehaviour
{
    [Header("Network Settings")]
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public string serverIp;
    public ushort serverPort;
    public string idPlayer;
    public bool empezar = false;
    public NetworkPipeline pipeline;

    [Header("Player variables")]
    public GameObject playerPrefab;
    public GameObject interfaz;
    public GameObject interfazMuerte;
    public Transform deathCamPos;
    public Dictionary<string, GameObject> simulatedPlayers;
    public float sensitivity;
    public short[] arrGuns;
    public bool muerto = false;

    [Header("Guns Variables")]
    private GetGuns getGuns = new GetGuns();
    public List<GameObject> guns;

    [Header("Bullet hole pool")]
    public GameObject bulletHolePrefab;
    public int bulletHolePoolSize = 30;
    public List<GameObject> bulletHolePool;

    [Header("Items list")]
    public List<GameObject> itemsList;

    void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkEndpoint.Parse(serverIp, serverPort);
        simulatedPlayers = new Dictionary<string, GameObject>();
        pipeline = m_Driver.CreatePipeline(typeof(FragmentationPipelineStage),
            typeof(ReliableSequencedPipelineStage));
        m_Connection = m_Driver.Connect(endpoint);
        Debug.Log("Connecting to server");
        empezar = true;

        /*
         * Place in a loop somewhere
        */

        getGuns.FetchGuns();


        for (int i = 0; i < bulletHolePoolSize; i++)
        {
            var auxBulletHole = Instantiate(bulletHolePrefab);
            auxBulletHole.SetActive(false);
            bulletHolePool.Add(auxBulletHole);
        }

        StartCoroutine(DestroyBulletHole());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    void OnDestroy()
    {
        m_Driver.Disconnect(m_Connection);
        m_Driver.ScheduleUpdate().Complete();
        m_Driver.Dispose();
    }


    void Update()
    {
        if (!empezar)
        {
            return;
        }

        m_Driver.ScheduleUpdate().Complete();
        if (!m_Connection.IsCreated)
        {
            return;
        }

        Unity.Collections.DataStreamReader stream;
        NetworkEvent.Type cmd = m_Connection.PopEvent(m_Driver, out stream);

        while (cmd != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("Connected to the server");
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                OnData(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                m_Connection = default(NetworkConnection);
            }
            cmd = m_Connection.PopEvent(m_Driver, out stream);
        }

        PlayerMovement();
    }

    private void PlayerMovement()
    {
        /*Most of the code in this script should be place in jobs but i don't know how to do that
         */
        if (!empezar || !m_Connection.IsCreated || idPlayer == string.Empty || muerto)
        {
            return;
        }
        short verticalInput = (short)(Input.GetKey("d") ? 1 : Input.GetKey("a") ? -1 : 0);
        short horizontalInput = (short)(Input.GetKey("w") ? 1 : Input.GetKey("s") ? -1 : 0);

        short runInput = (short)(Input.GetKey("x") ? 1 : 0);

        short leanInput = (short)(Input.GetKey("e") ? -1 : Input.GetKey("q") ? 1 : 0);

        byte fireInput = (byte)(Input.GetMouseButton(0) ? 1 : 0);

        byte aimInput = (byte)(Input.GetMouseButton(1) ? 1 : 0);

        byte reloadInput = (byte)(Input.GetKey("r") ? 1 : 0);

        float mouseScrollInput = Input.mouseScrollDelta.y * 0.1f;

        byte switchGunInput = (byte)(Input.GetKeyDown(KeyCode.Alpha1) ? 1 : Input.GetKeyDown(KeyCode.Alpha2) ? 2 : Input.GetKeyDown(KeyCode.Alpha3) ? 3 : 0);

        if (mouseScrollInput != 0 || switchGunInput != 0)
        {
            PlayerSwitchGunMsg pSwitchGunMsg = new PlayerSwitchGunMsg();
            pSwitchGunMsg.id = idPlayer;
            pSwitchGunMsg.mouseScrollInput = mouseScrollInput;
            pSwitchGunMsg.switchGunInput = switchGunInput;
            SendToServer(JsonUtility.ToJson(pSwitchGunMsg));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerJumpMsg pJumpMsg = new PlayerJumpMsg();
            pJumpMsg.id = idPlayer;
            SendToServer(JsonUtility.ToJson(pJumpMsg));
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        PlayerInputMsg pInputMsg = new PlayerInputMsg();
        pInputMsg.id = idPlayer;
        pInputMsg.horizontalInput = horizontalInput;
        pInputMsg.verticalInput = verticalInput;
        pInputMsg.runInput = runInput;
        pInputMsg.leanInput = leanInput;
        pInputMsg.fireInput = fireInput;
        pInputMsg.aimInput = aimInput;
        pInputMsg.reloadInput = reloadInput;
        

        SendToServer(JsonUtility.ToJson(pInputMsg));

        PlayerMoveCamera pMoveCamera = new PlayerMoveCamera();
        pMoveCamera.id = idPlayer;
        pMoveCamera.mouseX = mouseX;
        pMoveCamera.mouseY = mouseY;
        SendToServer(JsonUtility.ToJson(pMoveCamera));
    }

    private void OnData(DataStreamReader stream)
    { 
        NativeArray<byte> bytes = new NativeArray<byte>(stream.Length, Allocator.Temp);
        stream.ReadBytes(bytes);
        string recMsg = System.Text.Encoding.ASCII.GetString(bytes.ToArray());
        NetworkHeader header = JsonUtility.FromJson<NetworkHeader>(recMsg);

        switch(header.command)
        {
            case Commands.HANDSHAKE:
                HandshakeMsg hsMsg = JsonUtility.FromJson<HandshakeMsg>(recMsg);
                HandshakeMsg hsMsgSend = new HandshakeMsg();
                idPlayer = hsMsg.player.id;
                Debug.Log("Handshake message received!");
                hsMsgSend.player.nombre = "Client " + hsMsg.player.id;
                hsMsgSend.player.id = hsMsg.player.id;
                hsMsgSend.player.arrGuns = arrGuns;
                SendToServer(JsonUtility.ToJson(hsMsgSend));
                break;
            case Commands.PLAYER_SPAWN:
                PlayerSpawnMsg psMsg = JsonUtility.FromJson<PlayerSpawnMsg>(recMsg);
                var playerAux = Instantiate(playerPrefab, psMsg.spawnPlayer.pos, Quaternion.identity);
                playerAux.GetComponent<PlayerScriptClient>().LoadLoadOut(GetLoadOut(psMsg.spawnPlayer.arrGuns));
                simulatedPlayers.Add(psMsg.id, playerAux);
                break;
            case Commands.PLAYER_POS:
                PlayerPosMsg pPosMsg = JsonUtility.FromJson<PlayerPosMsg>(recMsg);
                var playerAux2 = FindPlayerById(pPosMsg.id);

                if (muerto && pPosMsg.id == idPlayer)
                {
                    return;
                }

                if (playerAux2 != null)
                {
                    var eulerAngles = pPosMsg.pos.rotation.eulerAngles;
                    playerAux2.transform.position = pPosMsg.pos.position;
                    playerAux2.transform.localEulerAngles = new Vector3(eulerAngles.x, eulerAngles.y);
                    playerAux2.transform.GetChild(5).localEulerAngles = new Vector3(0, 0, eulerAngles.z);
                }
                break;
            case Commands.PLAYER_CAM_ROT:
                PlayerCameraRotationMsg pCamRotMsg = JsonUtility.FromJson<PlayerCameraRotationMsg>(recMsg);
                var playerAux7 = FindPlayerById(pCamRotMsg.id);

                if (playerAux7 == null)
                {
                    return;
                }

                playerAux7.transform.GetChild(0).localEulerAngles = pCamRotMsg.rotation;
                break;
            case Commands.PLAYER_JOIN:
                PlayerJoinMsg pJoinMsg = JsonUtility.FromJson<PlayerJoinMsg>(recMsg);
                for (int i = 0; i < pJoinMsg.playersList.Count; i++)
                {
                    var playerAux3 = Instantiate(playerPrefab, pJoinMsg.playersList[i].pos, Quaternion.identity);
                    playerAux3.GetComponent<PlayerScriptClient>().LoadLoadOut(GetLoadOut(pJoinMsg.playersList[i].arrGuns));
                    playerAux3.GetComponent<PlayerScriptClient>().SwitchGun(pJoinMsg.playersList[i].activeGunIndex);
                    if (pJoinMsg.playersList[i].isDead)
                    {
                        playerAux3.SetActive(false);
                    }
                    simulatedPlayers.Add(pJoinMsg.playersList[i].id, playerAux3);
                }

                for (int i = 0; i < pJoinMsg.itemList.Count; i++)
                {
                    itemsList[i].SetActive(pJoinMsg.itemList[i].enabled);
                }
                /*
                 * Reload loadout of the client
                 * this sucks, why did i access the player script from outside so many times
                 * well im not refactoring this so i dont care
                 */
                FindPlayerById(pJoinMsg.id).GetComponent<PlayerScriptClient>().HideLoadOut(GetLoadOutViewModel(pJoinMsg.playersList.Find(x => x.id == pJoinMsg.id).arrGuns));

                var playerCamera = FindPlayerById(pJoinMsg.id).transform.Find("Camara").GetComponent<Camera>();
                var uiCamera = playerCamera.transform.Find("UI Camara").GetComponent<Camera>();
                playerCamera.transform.GetChild(3).GetComponent<Camera>().enabled = true;

                playerCamera.enabled = true;
                uiCamera.enabled = true;
                interfaz.SetActive(true);
                interfaz.GetComponent<Canvas>().worldCamera = uiCamera;
                interfaz.GetComponent<Canvas>().planeDistance = 1f;

                interfazMuerte.GetComponent<Canvas>().worldCamera = uiCamera;
                interfazMuerte.GetComponent<Canvas>().planeDistance = 1f;
                break;

            case Commands.PLAYER_SWITCH_GUN:
                PlayerSwitchGunClient pSwitchGunMsg = JsonUtility.FromJson<PlayerSwitchGunClient>(recMsg);
                var playerAux4 = FindPlayerById(pSwitchGunMsg.id);
                playerAux4.GetComponent<PlayerScriptClient>().SwitchGun(pSwitchGunMsg.gunIndex);
                break;

            case Commands.PLAYER_DISCONNECT:
                PlayerDisconnectMsg pDisconnectMsg = JsonUtility.FromJson<PlayerDisconnectMsg>(recMsg);
                Debug.Log("Player " + pDisconnectMsg.id + " disconnected");
                var playerAux5 = simulatedPlayers[pDisconnectMsg.id];
                simulatedPlayers.Remove(pDisconnectMsg.id);
                Destroy(playerAux5);
                break;

            case Commands.PLAYER_INVENTORY:
                PlayerInventoryMsg pInventoryMsg = JsonUtility.FromJson<PlayerInventoryMsg>(recMsg);
                var uiScript = interfaz.GetComponent<UIScript>();
                uiScript.ammoInMag = pInventoryMsg.ammoInMag;
                uiScript.ammoCount = pInventoryMsg.ammoCount;
                uiScript.health = pInventoryMsg.health;
                break;

            case Commands.CREATE_BULLET_HOLE:
                CreateBulletHoleMsg cBulletHoleMsg = JsonUtility.FromJson<CreateBulletHoleMsg>(recMsg);
                CreateBulletHole(cBulletHoleMsg.hit);
                break;

            case Commands.PLAYER_KILL:
                PlayerKillMsg pKillMsg = JsonUtility.FromJson<PlayerKillMsg>(recMsg);
                playerAux5 = simulatedPlayers[pKillMsg.id];
                playerAux5.transform.position = deathCamPos.position;
                playerAux5.transform.rotation = deathCamPos.rotation;

                if (pKillMsg.id == idPlayer)
                {
                    muerto = true;
                    interfaz.SetActive(false);
                    interfazMuerte.SetActive(true);
                    interfazMuerte.transform.Find("TxtRespawn").GetComponent<TMPro.TextMeshProUGUI>().text = "Preparate para volver a MATAR";
                }
                else 
                {
                    playerAux5.SetActive(false);
                }
                break;
            case Commands.PLAYER_RESPAWN:
                PlayerRespawnMsg pRespawnMsg = JsonUtility.FromJson<PlayerRespawnMsg>(recMsg);
                playerAux5 = simulatedPlayers[pRespawnMsg.id];
                Debug.Log(pRespawnMsg.id);
                if (pRespawnMsg.id == idPlayer)
                {
                    muerto = false;
                    interfaz.SetActive(true);
                    interfazMuerte.SetActive(false);
                } 
                else
                {
                    playerAux5.SetActive(true);
                }
                break;
            case Commands.PLAYER_ANIMATION:
                PlayerAnimationMsg pAnimationMsg = JsonUtility.FromJson<PlayerAnimationMsg>(recMsg);
                simulatedPlayers.TryGetValue(pAnimationMsg.id, out GameObject playerAux6);
                if (playerAux6 != null)
                {
                    playerAux6.GetComponent<PlayerScriptClient>().PlayAnimations(pAnimationMsg.animation);
                }
                break;
            case Commands.ITEM_STATE:
                StateItemMsg iStateMsg = JsonUtility.FromJson<StateItemMsg>(recMsg);
                itemsList[iStateMsg.itemId].SetActive(iStateMsg.enabled);
                break;
            default:
                break;
        }
    }

    //Can't be removed later
    public GameObject FindPlayerById(string idJugador)
    {
        simulatedPlayers.TryGetValue(idJugador, out GameObject player);
        return player;
    }

    public GameObject[] GetLoadOut(short[] gunsIds)
    {
        GameObject[] returnGunsArr = new GameObject[gunsIds.Length];
        for (int i = 0; i < gunsIds.Length; i++)
        {
            
            returnGunsArr[i] = getGuns.GetGun(gunsIds[i]).GetPrefabClient();
        }

        return returnGunsArr;
    }

    public GameObject[] GetLoadOutViewModel(short[] gunsIds)
    {
        GameObject[] returnGunsArr = new GameObject[gunsIds.Length];
        for (int i = 0; i < gunsIds.Length; i++)
        {

            returnGunsArr[i] = getGuns.GetGun(gunsIds[i]).GetPrefabViewModel();
        }

        return returnGunsArr;
    }

    private void SendToServer(string message)
    {
        DataStreamWriter writer;
        m_Driver.BeginSend(pipeline, m_Connection, out writer);
        NativeArray<byte> bytes = new NativeArray<byte>(System.Text.Encoding.ASCII.GetBytes(message), Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }

    public void CreateBulletHole(NetworkObject.NetworkTransform hit)
    {
        var bulletHole = bulletHolePool.Find(x => !x.activeSelf);
        if (bulletHole != null)
        {
            bulletHole.transform.position = hit.position;
            bulletHole.transform.rotation = hit.rotation;
            bulletHole.SetActive(true);
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
}
