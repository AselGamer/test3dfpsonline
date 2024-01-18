using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Server : MonoBehaviour
{
    public NetworkDriver m_Driver;
    public ushort m_ServerPort;
    private NativeList<NetworkConnection> m_Connections;
    private List<NetworkObject.NetworkPlayer> m_Players;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
