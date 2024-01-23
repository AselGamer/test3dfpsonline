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
    public List<GameObject> simulatedPlayers;
    public float sensitivity;
    public short[] arrGuns;

    [Header("Guns Variables")]
    public List<GameObject> guns;

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

        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (!empezar || !m_Connection.IsCreated || idPlayer == string.Empty)
        {
            return;
        }
        short verticalInput = (short)(Input.GetKey("d") ? 1 : Input.GetKey("a") ? -1 : 0);
        short horizontalInput = (short)(Input.GetKey("w") ? 1 : Input.GetKey("s") ? -1 : 0);

        short leanInput = (short)(Input.GetKey("e") ? -1 : Input.GetKey("q") ? 1 : 0);

        byte fireInput = (byte)(Input.GetMouseButton(0) ? 1 : 0);

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

        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        PlayerInputMsg pInputMsg = new PlayerInputMsg();
        pInputMsg.id = idPlayer;
        pInputMsg.horizontalInput = horizontalInput;
        pInputMsg.verticalInput = verticalInput;
        pInputMsg.leanInput = leanInput;
        pInputMsg.fireInput = fireInput;
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
                hsMsgSend.player.arrGuns = arrGuns;
                SendToServer(JsonUtility.ToJson(hsMsgSend));
                break;
            case Commands.PLAYER_SPAWN:
                PlayerSpawnMsg psMsg = JsonUtility.FromJson<PlayerSpawnMsg>(recMsg);
                var playerAux = Instantiate(playerPrefab, psMsg.spawnPlayer.pos, Quaternion.identity);
                playerAux.GetComponent<PlayerScriptClient>().LoadLoadOut(GetLoadOut(psMsg.spawnPlayer.arrGuns));
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
                for (int i = 0; i < pJoinMsg.playersList.Count; i++)
                {
                    var playerAux3 = Instantiate(playerPrefab, pJoinMsg.playersList[i].pos, Quaternion.identity);
                    playerAux3.GetComponent<PlayerScriptClient>().LoadLoadOut(GetLoadOut(pJoinMsg.playersList[i].arrGuns));
                    simulatedPlayers.Add(playerAux3);
                }

                FindPlayerById(pJoinMsg.id).GetComponent<PlayerScriptClient>().HideLoadOut();
                var playerCamera = FindPlayerById(pJoinMsg.id).transform.Find("Camara").GetComponent<Camera>();
                var uiCamera = playerCamera.transform.Find("UI Camara").GetComponent<Camera>();
                playerCamera.enabled = true;
                uiCamera.enabled = true;
                interfaz.SetActive(true);
                interfaz.GetComponent<Canvas>().worldCamera = uiCamera;
                interfaz.GetComponent<Canvas>().planeDistance = 1f;
                break;

            case Commands.PLAYER_SWITCH_GUN:
                PlayerSwitchGunClient pSwitchGunMsg = JsonUtility.FromJson<PlayerSwitchGunClient>(recMsg);
                var playerAux4 = FindPlayerById(pSwitchGunMsg.id);
                playerAux4.GetComponent<PlayerScriptClient>().SwitchGun(pSwitchGunMsg.gunIndex);
                break;
            case Commands.PLAYER_DISCONNECT:
                PlayerDisconnectMsg pDisconnectMsg = JsonUtility.FromJson<PlayerDisconnectMsg>(recMsg);
                int id = int.Parse(pDisconnectMsg.id);
                Destroy(simulatedPlayers[id]);
                simulatedPlayers.RemoveAt(id);
                break;
            default:
                break;
        }
    }

    public GameObject FindPlayerById(string idJugador)
    {
        return simulatedPlayers.Find(x => simulatedPlayers.IndexOf(x) == int.Parse(idJugador));
    }

    public GameObject[] GetLoadOut(short[] arrGuns)
    {
        GameObject[] returnGunsArr = new GameObject[arrGuns.Length];
        for (int i = 0; i < arrGuns.Length; i++)
        {
            
            returnGunsArr[i] = guns[arrGuns[i]];
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
}
