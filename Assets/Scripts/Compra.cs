using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class Compra : MonoBehaviour
{
    public GameObject comprar;
    public string json;
    public RespuestaVenta respuestaVenta;
    public int idVenta;
    public GameObject panelSesion;
    public GameObject articulo;
    public GameObject id;
    public GameObject tipoEquipamiento;
    public GameObject nombre;
    public GameObject precio;

    // Start is called before the first frame update
    void Start()
    {
        comprar.GetComponent<Button>().onClick.AddListener(PostData);
    }

    void PostData() => StartCoroutine(PostData_Coroutine());
    //void PostDataDinero(float precioVenta) => StartCoroutine(PostData_Coroutine_Dinero(precioVenta));
    ////////////////////////////////////////////////////////////////
    IEnumerator PostData_Coroutine()
    {
        string uri = "https://retoiraitz.duckdns.org/api/venta/";
        //string uri = "http://localhost:8069/api/venta/";
        int usuario_id = panelSesion.GetComponent<PostMethod>().idUsuario;
        Debug.Log("id--------->" + usuario_id);
        string tipo = tipoEquipamiento.GetComponent<TextMeshProUGUI>().text;
        Debug.Log("tipo--------->" + tipo);
        string fch_compra = DateTime.Now.ToString("yyyy-MM-dd");
        Debug.Log("fch_compra--------->" + fch_compra);
        string equipamiento_id = "";
        if(tipo == "arma")
        {
            equipamiento_id = "arma_id";
        }else if (tipo == "armadura")
        {
            equipamiento_id = "armadura_id";
        }
        else if (tipo == "medicamento")
        {
            equipamiento_id = "medicamento_id";
        }
        string arma_id = id.GetComponent<TextMeshProUGUI>().text;
        Debug.Log("arma_id--------->" + arma_id);
        int currency_id = 1;
        string state = "comprado";
        string jsonData = "{\"usuario_id\":" + usuario_id + ", \"tipo\":\"" + tipo + "\", \"fch_compra\":\"" + fch_compra + "\"," +
            " \"" + equipamiento_id + "\":" + arma_id + ", \"currency_id\":" + currency_id + ", \"state\":\"" + state + "\"}";
        Debug.Log("jsonData---> " + jsonData);
        string jsonParams = "{\"data\":" + jsonData + "}";
        Debug.Log("jsonData---> " + jsonParams);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"params\":" + jsonParams + "}");
        UnityWebRequest request = UnityWebRequest.PostWwwForm(uri, "");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error++++++++++++");
        }
        else
        {
            json = request.downloadHandler.text;
            respuestaVenta = JsonUtility.FromJson<RespuestaVenta>(json);
            if (respuestaVenta.error != null)
            {
                Debug.Log("Error++++++++++");
            }
            else
            {
                idVenta = respuestaVenta.result;
                Debug.Log("idVenta--------->" + idVenta);
                float precioVenta = float.Parse(precio.GetComponent<TextMeshProUGUI>().text);
                Debug.Log("precio--------->" + precioVenta);
                //PostDataDinero(precioVenta);/////////////////////
            }
        }
    }

    /*IEnumerator PostData_Coroutine_Dinero(float precioVenta)//////////////////////////
    {
        string uri = "https://retoiraitz.duckdns.org/api/res.users/";
        //string uri = "http://localhost:8069/api/res.users/";

    }*/
}
