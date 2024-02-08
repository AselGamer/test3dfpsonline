using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GetGunsScript
{
    public class GetGuns
    {
        private Dictionary<int, Gun> gunsDict;
        public GetGuns()
        {
            gunsDict = new Dictionary<int, Gun>();
        }

        //replace with odoo call
        public void FetchGuns()
        {
            gunsDict.Add(0, new Gun(0, "9mm Pistol", Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol/hl2_pistol"), Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol/hl2_pistol_viewmodel"), Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol/hl2_pistol_client")));
            gunsDict.Add(2, new Gun(0, "9mm Pistol", Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol/hl2_pistol"), Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol/hl2_pistol_viewmodel"), Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol/hl2_pistol_client")));
            gunsDict.Add(5, new Gun(1, "SPAS-12", Resources.Load<GameObject>("Prefabs/Guns/hl2_shotgun/hl2_shotgun"), Resources.Load<GameObject>("Prefabs/Guns/hl2_shotgun/hl2_shotgun_viewmodel"), Resources.Load<GameObject>("Prefabs/Guns/hl2_shotgun/hl2_shotgun_client")));
            gunsDict.Add(4, new Gun(2, "Beretta", Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol_purple/hl2_pistol_purple"), Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol_purple/hl2_pistol_purple_viewmodel"), Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol_purple/hl2_pistol_purple_client")));
            gunsDict.Add(1, new Gun(3, "Magnum", Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol_blue/hl2_pistol_blue"), Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol_blue/hl2_pistol_blue_viewmodel"), Resources.Load<GameObject>("Prefabs/Guns/hl2_pistol_blue/hl2_pistol_blue_client")));
            gunsDict.Add(6, new Gun(4, "SMG", Resources.Load<GameObject>("Prefabs/Guns/hl2_shotgun_green/hl2_shotgun_green"), Resources.Load<GameObject>("Prefabs/Guns/hl2_shotgun_green/hl2_shotgun_green_viewmodel"), Resources.Load<GameObject>("Prefabs/Guns/hl2_shotgun_green/hl2_shotgun_green_client")));
        }

        public Gun GetGun(int id)
        {
            return gunsDict[id];
        }
    }
    
    public class Gun
    {
        private int id;
        private string name;
        private GameObject prefabServer;
        private GameObject prefabViewModel;
        private GameObject prefabClient;

        public Gun(int id, string name, GameObject prefabServer, GameObject prefabViewModel, GameObject prefabClient)
        {
            this.id = id;
            this.name = name;
            this.prefabServer = prefabServer;
            this.prefabViewModel = prefabViewModel;
            this.prefabClient = prefabClient;
        }

        public int GetId()
        {
            return id;
        }

        public string GetName()
        {
            return name;
        }

        public GameObject GetPrefabServer()
        {
            return prefabServer;
        }

        public GameObject GetPrefabViewModel()
        {
            return prefabViewModel;
        }

        public GameObject GetPrefabClient()
        {
            return prefabClient;
        }
    }


}
