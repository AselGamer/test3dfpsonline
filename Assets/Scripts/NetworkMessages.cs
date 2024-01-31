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
        public int activeGunIndex;
    }

    [System.Serializable]
    public class NetworkTransform : NetworkObject
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    [System.Serializable]
    public class NetworkAnimation : NetworkObject
    {
        public int velocidad_y;
        public int velocidad_x;
        public int fire_axis;
        public int aim_axis;
        public bool isGrounded;
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
        PLAYER_JUMP,
        PLAYER_INVENTORY,
        PLAYER_SWITCH_GUN,
        PLAYER_DISCONNECT,
        PLAYER_KILL,
        PLAYER_ANIMATION,
        CREATE_BULLET_HOLE,
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
        public byte aimInput;
        public byte reloadInput;

        public float mouseX;
        public float mouseY;
        public PlayerInputMsg()
        {
            command = Commands.PLAYER_INPUT;
            verticalInput = 0;
            horizontalInput = 0;
            leanInput = 0;
            fireInput = 0;
            aimInput = 0;
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

    [System.Serializable]
    public class PlayerSwitchGunMsg : NetworkHeader
    {
        public string id;
        public byte switchGunInput;
        public float mouseScrollInput;
        public PlayerSwitchGunMsg()
        {
            command = Commands.PLAYER_SWITCH_GUN;
            switchGunInput = 0;
            mouseScrollInput = 0;
        }
    }

    [System.Serializable]
    public class PlayerSwitchGunClient : NetworkHeader
    {
        public string id;
        public int gunIndex;
        public PlayerSwitchGunClient()
        {
            command = Commands.PLAYER_SWITCH_GUN;
            gunIndex = 0;
        }
    }

    [System.Serializable]
    public class PlayerDisconnectMsg : NetworkHeader
    {
        public string id;
        public PlayerDisconnectMsg()
        {
            command = Commands.PLAYER_DISCONNECT;
            id = "";
        }
    }

    [System.Serializable]
    public class PlayerInventoryMsg : NetworkHeader
    {
        public string id;
        public int gunIndex;
        public int ammoCount;
        public int ammoInMag;
        public int health;

        public PlayerInventoryMsg()
        {
            command = Commands.PLAYER_INVENTORY;
            gunIndex = 0;
            ammoCount = 0;
            ammoInMag = 0;
            health = 0;
        }
    }

    [System.Serializable]
    public class CreateBulletHoleMsg : NetworkHeader
    {
        public NetworkObject.NetworkTransform hit;
        public CreateBulletHoleMsg()
        {
            command = Commands.CREATE_BULLET_HOLE;
            hit = new NetworkObject.NetworkTransform();
        }
    }

    [System.Serializable]
    public class PlayerKillMsg : NetworkHeader
    {
        public string id;
        public int respCountDown;
        public PlayerKillMsg()
        {
            command = Commands.PLAYER_KILL;
            id = "";
            respCountDown = 0;
        }
    }

    [System.Serializable]
    public class PlayerAnimationMsg : NetworkHeader
    {
        public string id;
        public NetworkObject.NetworkAnimation animation;
        public PlayerAnimationMsg()
        {
            command = Commands.PLAYER_ANIMATION;
            id = "";
            animation = new NetworkObject.NetworkAnimation();
        }
    }
}
