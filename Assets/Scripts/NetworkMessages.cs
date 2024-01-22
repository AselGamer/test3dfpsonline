using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkObject
{
    [System.Serializable]
    public class NetworkObject
    {
        public string id;
    }

    [System.Serializable]
    public class NetworkPlayer : NetworkObject
    {
        public Vector3 pos;
        public string nombre;
        public short[] arrGuns;
    }

    [System.Serializable]
    public class NetworkTransform : NetworkObject
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}

namespace NetworkMessages
{
    public enum Commands
    {
        HANDSHAKE,
        PLAYER_INPUT,
        PLAYER_POS,
        PLAYER_SPAWN,
        PLAYER_JOIN,
        PLAYER_JUMP
    }

    [System.Serializable]
    public class NetworkHeader
    {
        public Commands command;
    }

    [System.Serializable]
    public class HandshakeMsg : NetworkHeader
    {
        public NetworkObject.NetworkPlayer player;
        public HandshakeMsg()
        {
            command = Commands.HANDSHAKE;
            player = new NetworkObject.NetworkPlayer();
        }
    }

    [System.Serializable]
    public class PlayerInputMsg : NetworkHeader
    {
        public string id;
        public short verticalInput;
        public short horizontalInput;
        public short leanInput;

        public byte fireInput;

        public float mouseX;
        public float mouseY;
        public PlayerInputMsg()
        {
            command = Commands.PLAYER_INPUT;
            verticalInput = 0;
            horizontalInput = 0;
            leanInput = 0;
            fireInput = 0;
            mouseX = 0;
            mouseY = 0;
        }
    }

    [System.Serializable]
    public class PlayerJumpMsg : NetworkHeader
    {
        public string id;
        public PlayerJumpMsg()
        {
            command = Commands.PLAYER_JUMP;
        }
    }


    [System.Serializable]
    public class PlayerPosMsg : NetworkHeader
    {
        public string id;
        public NetworkObject.NetworkTransform pos;
        public Vector3 cameraRotation;
        public PlayerPosMsg()
        {
            command = Commands.PLAYER_POS;
            pos = new NetworkObject.NetworkTransform();
            cameraRotation = Vector3.zero;
        }
    }

    [System.Serializable]
    public class PlayerSpawnMsg : NetworkHeader
    {
        public string id;
        public NetworkObject.NetworkPlayer spawnPlayer;
        public PlayerSpawnMsg()
        {
            command = Commands.PLAYER_SPAWN;
            spawnPlayer = new();
        }
    }

    [System.Serializable]
    public class PlayerJoinMsg : NetworkHeader
    {
        public string id;
        public List<NetworkObject.NetworkPlayer> playersList;
        public PlayerJoinMsg()
        {
            command = Commands.PLAYER_JOIN;
            playersList = new List<NetworkObject.NetworkPlayer>();
        }
    }
}
