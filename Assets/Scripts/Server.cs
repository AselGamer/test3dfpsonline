using NetworkMessages;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using Unity.Jobs;
using System;

public class Server : MonoBehaviour
{
    [Header("Network Settings")]
    public NetworkDriver m_Driver;
    public ushort serverPort;
    private NativeList<NetworkConnection> m_Connections;
    public List<NetworkObject.NetworkPlayer> m_Players;
    private static int nextId = 0;
    public static NetworkPipeline pipeline;
    JobHandle m_ServerJobHandle;

    [Header("Player variables")]
    public GameObject playerPrefab;
    public Dictionary<string, GameObject> simulatedPlayers;
    public List<GameObject> guns;
    void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        var endpoint = NetworkEndpoint.AnyIpv4.WithPort(serverPort);
        simulatedPlayers = new Dictionary<string, GameObject>();
        pipeline = m_Driver.CreatePipeline(typeof(FragmentationPipelineStage),
            typeof(ReliableSequencedPipelineStage));
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

    // Update is called once per frame
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
                    m_Driver.Disconnect(m_Connections[i]);
                    m_Connections.RemoveAtSwapBack(i);
                    m_Players.RemoveAt(i);
                    var playerAux = simulatedPlayers[i+""];
                    simulatedPlayers.Remove(i + "");
                    Destroy(playerAux);
                    PlayerDisconnectMsg pDisconnectMsg = new PlayerDisconnectMsg();
                    pDisconnectMsg.id = i.ToString();
                    cmd = NetworkEvent.Type.Empty;
                    SendToAllClients(JsonUtility.ToJson(pDisconnectMsg));
                    --i;
                    continue;
                }
                cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream);
            }
        }

        foreach (var playerKvp in simulatedPlayers)
        {
            PlayerPosMsg pPosMsg = new PlayerPosMsg();
            var player = playerKvp.Value;
            var playerId = playerKvp.Key;
            pPosMsg.id = playerId;
            pPosMsg.pos.position = player.transform.position;
            pPosMsg.pos.rotation = player.transform.rotation;
            pPosMsg.cameraRotation = player.transform.Find("Camara").transform.localEulerAngles;
            SendToAllClients(JsonUtility.ToJson(pPosMsg));

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

            SendToClient(JsonUtility.ToJson(pInventoryMsg), m_Connections[int.Parse(playerId)]);
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

                //Menssage to spawn new player
                PlayerSpawnMsg pSpawnMsg = new PlayerSpawnMsg();
                pSpawnMsg.id = hsMsg.player.id;
                var playerAux = Instantiate(playerPrefab);
                playerAux.GetComponent<PlayerScript>().LoadLoadOut(GetLoadOut(hsMsg.player.arrGuns));
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

                //Menssage to new player
                PlayerJoinMsg playerJoinMsg = new PlayerJoinMsg();
                playerJoinMsg.id = hsMsg.player.id;
                playerJoinMsg.playersList = m_Players;
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
                playerAux2.GetComponentInChildren<CameraScript>().mouseX = pInputMsg.mouseX;
                playerAux2.GetComponentInChildren<CameraScript>().mouseY = pInputMsg.mouseY;
                break;
            case Commands.PLAYER_JUMP:
                PlayerJumpMsg pJumpMsg = JsonUtility.FromJson<PlayerJumpMsg>(recMsg);
                FindPlayerById(pJumpMsg.id).GetComponent<PlayerScript>().jumpInput = true;
                break;
            case Commands.PLAYER_SWITCH_GUN:
                PlayerSwitchGunMsg pSwitchGunMsg = JsonUtility.FromJson<PlayerSwitchGunMsg>(recMsg);
                var playerScriptAux = FindPlayerById(pSwitchGunMsg.id).GetComponent<PlayerScript>();
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

    public GameObject[] GetLoadOut(short[] arrGuns)
    {
        GameObject[] gunsArr = new GameObject[arrGuns.Length];
        arrGuns.ToList().ForEach(x => gunsArr[arrGuns.ToList().IndexOf(x)] = guns[x]);

        return gunsArr;
    }

    public GameObject FindPlayerById(string idJugador)
    {
        return simulatedPlayers[idJugador];
    }

    void OnConnect(NetworkConnection c)
    {
        m_Connections.Add(c);
        Debug.Log("Accepted a connection");

        HandshakeMsg m = new HandshakeMsg();
        m.player.id = nextId.ToString();
        nextId++;
        SendToClient(JsonUtility.ToJson(m), c);
    }

    private void SendToClient(string message, NetworkConnection c)
    {
        if (!c.IsCreated)
        {
            return;
        }

        DataStreamWriter writer;
        m_Driver.BeginSend(pipeline, c, out writer);
        NativeArray<byte> bytes = new NativeArray<byte>(System.Text.Encoding.ASCII.GetBytes(message), Allocator.Temp);
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
}
