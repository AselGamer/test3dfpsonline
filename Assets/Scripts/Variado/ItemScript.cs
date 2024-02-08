using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public int itemId;
    private Server server;
    private float respawnWait = 60f;


    //Please don't lag, please don't lag, please don't lag
    void Start()
    {
        server = GameObject.Find("Server").GetComponent<Server>();
    }

    void OnDisable()
    {
        //StartCoroutine(RespawnItem());
        server.SendItemStateChange(itemId);
    }

    private void OnEnable()
    {
        if (server == null)
        {
            return;
        }
        server.SendItemStateChange(itemId);
    }

    public IEnumerator RespawnItem()
    {
        yield return new WaitForSeconds(respawnWait);
        gameObject.SetActive(true);
    }
}
