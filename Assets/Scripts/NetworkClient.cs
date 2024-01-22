using NetworkMessages;
using System.Collections;
using System.Collections.Generic;
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
    public List<GameObject> simulatedPlayers;
    public float sensitivity;

    void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);
        var endpoint = NetworkEndpoint.Parse(serverIp, serverPort);
        pipeline = m_Driver.CreatePipeline(typeof(FragmentationPipelineStage),
            typeof(ReliableSequencedPipelineStage));
        m_Connection = m_Driver.Connect(endpoint);
        Debug.Log("Connecting to server");
        empezar = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    void OnDestroy()
    {
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

        while(cmd != NetworkEvent.Type.Empty)
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

        if (!empezar || !m_Connection.IsCreated || idPlayer == string.Empty)
        {
            return;
        }
        short verticalInput = (short)(Input.GetKey("d") ? 1 : Input.GetKey("a") ? -1 : 0);
        short horizontalInput = (short)(Input.GetKey("w") ? 1 : Input.GetKey("s") ? -1 : 0);

        short leanInput = (short)(Input.GetKey("e") ? -1 : Input.GetKey("q") ? 1 : 0);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerJumpMsg pJumpMsg = new PlayerJumpMsg();
            pJumpMsg.id = idPlayer;
            SendToServer(JsonUtility.ToJson(pJumpMsg));
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        PlayerInputMsg pInputMsg = new PlayerInputMsg();
        pInputMsg.id = idPlayer;
        pInputMsg.horizontalInput = horizontalInput;
        pInputMsg.verticalInput = verticalInput;
        pInputMsg.leanInput = leanInput;
        pInputMsg.mouseX = mouseX;
        pInputMsg.mouseY = mouseY;
        SendToServer(JsonUtility.ToJson(pInputMsg));
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
                SendToServer(JsonUtility.ToJson(hsMsgSend));
                break;
            case Commands.PLAYER_SPAWN:
                PlayerSpawnMsg psMsg = JsonUtility.FromJson<PlayerSpawnMsg>(recMsg);
                var playerAux = Instantiate(playerPrefab, psMsg.pos, Quaternion.identity);
                /*
                if (psMsg.id.Equals(idPlayer))
                {
                    playerAux.GetComponentInChildren<Camera>().enabled = true;
                }
                */
                simulatedPlayers.Add(playerAux);
                break;
            case Commands.PLAYER_POS:
                PlayerPosMsg pPosMsg = JsonUtility.FromJson<PlayerPosMsg>(recMsg);
                var playerAux2 = FindPlayerById(pPosMsg.id);
                if (playerAux2 != null)
                {
                    playerAux2.transform.position = pPosMsg.pos.position;
                    playerAux2.transform.rotation = pPosMsg.pos.rotation;
                    playerAux2.transform.Find("Camara").localEulerAngles = pPosMsg.cameraRotation;
                }
                break;
            case Commands.PLAYER_JOIN:
                PlayerJoinMsg pJoinMsg = JsonUtility.FromJson<PlayerJoinMsg>(recMsg);
                for (int i = 0; i < pJoinMsg.playerPos.Count; i++)
                {
                    var playerAux3 = Instantiate(playerPrefab, pJoinMsg.playerPos[i], Quaternion.identity);
                    simulatedPlayers.Add(playerAux3);
                }

                FindPlayerById(pJoinMsg.id).GetComponentInChildren<Camera>().enabled = true;
                break;
            default:
                break;
        }
    }

    public GameObject FindPlayerById(string idJugador)
    {
        return simulatedPlayers.Find(x => simulatedPlayers.IndexOf(x) == int.Parse(idJugador));
    }

    private void SendToServer(string message)
    {
        DataStreamWriter writer;
        m_Driver.BeginSend(pipeline, m_Connection, out writer);
        NativeArray<byte> bytes = new NativeArray<byte>(System.Text.Encoding.ASCII.GetBytes(message), Allocator.Temp);
        writer.WriteBytes(bytes);
        m_Driver.EndSend(writer);
    }
}
