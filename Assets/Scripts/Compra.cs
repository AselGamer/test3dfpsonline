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
    public RespuestaUsuario respuestaUsuario;
    public int idVenta;
    public bool resultadoPut;
    public GameObject panelSesion;
    public GameObject articulo;
    public GameObject id;
    public GameObject tipoEquipamiento;
    public GameObject nombre;
    public GameObject precio;
    public GameObject dinero;
    public int idUsuario;

    // Start is called before the first frame update
    void Start()
    {
        comprar.GetComponent<Button>().onClick.AddListener(PostData);
    }

    void PostData() => StartCoroutine(PostData_Coroutine());
    void PutDataDinero(float precioVenta) => StartCoroutine(PutData_Coroutine_Dinero(precioVenta));

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
            Debug.Log(request.error);
        }
        else
        {
            json = request.downloadHandler.text;
            respuestaVenta = JsonUtility.FromJson<RespuestaVenta>(json);
            if (respuestaVenta.error != null)
            {
                Debug.Log(request.error);
            }
            else
            {
                idVenta = respuestaVenta.result;
                Debug.Log("idVenta--------->" + idVenta);
                float precioVenta = float.Parse(precio.GetComponent<TextMeshProUGUI>().text);
                Debug.Log("precio--------->" + precioVenta);
                PutDataDinero(precioVenta);
            }
        }
    }

    IEnumerator PutData_Coroutine_Dinero(float precioVenta)
    {
        string uri = "https://retoiraitz.duckdns.org/api/res.users/";
        //string uri = "http://localhost:8069/api/res.users/";

        int cartera = int.Parse(dinero.GetComponent<TextMeshProUGUI>().text);
        cartera -= (int)precioVenta;

        idUsuario = panelSesion.GetComponent<PostMethod>().idUsuario;

        string jsonData = "{\"x_dinero\":" + cartera + "}";
        Debug.Log("jsonData---> " + jsonData);
        string jsonParams = "{\"filter\": [[\"id\", \"=\", " + idUsuario + " ]], \"data\":" + jsonData + "}";
        Debug.Log("jsonData---> " + jsonParams);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"params\":" + jsonParams + "}");
        UnityWebRequest request = UnityWebRequest.Put(uri, "");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            json = request.downloadHandler.text;
            respuestaUsuario = JsonUtility.FromJson<RespuestaUsuario>(json);
            if (respuestaUsuario.error != null)
            {
                Debug.Log(request.error);
            }
            else
            {
                resultadoPut = respuestaUsuario.result;
                Debug.Log("resultadoPut--------->" + resultadoPut);
                dinero.GetComponent<TextMeshProUGUI>().text = cartera.ToString();
            }
        }
    }
}
