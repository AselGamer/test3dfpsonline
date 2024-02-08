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
    public Venta[] ventas;
    public GameObject aviso;
    private float precioVenta;
    private Usuario usuario;

    // Start is called before the first frame update
    void Start()
    {
        idUsuario = panelSesion.GetComponent<PostMethod>().idUsuario;
        precioVenta = float.Parse(precio.GetComponent<TextMeshProUGUI>().text);
        comprar.GetComponent<Button>().onClick.AddListener(GetDataDinero);
    }

    void PostData() => StartCoroutine(PostData_Coroutine());
    void PutDataDinero() => StartCoroutine(PutData_Coroutine_Dinero());
    void GetDataDinero() => StartCoroutine(GetData_Coroutine_Dinero());

    IEnumerator GetData_Coroutine_Dinero()
    {
        string uri = "https://retoiraitz.duckdns.org/api/res.users/" + idUsuario + "/?query={id, name, x_dinero}";
        //string uri = "http://localhost:8069/api/res.users/" + idUsuario + "/?query={id, name, x_dinero}";
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                aviso.SetActive(true);
                aviso.GetComponent<TextMeshProUGUI>().text = "Error de conexión";
                Debug.Log(request.error);
            }
            else
            {
                json = request.downloadHandler.text;
                usuario = JsonUtility.FromJson<Usuario>(json);

                int cartera = usuario.x_dinero;
                if(cartera >= precioVenta)
                {
                    PutDataDinero();
                }
                else
                {
                    aviso.SetActive(true);
                    aviso.GetComponent<TextMeshProUGUI>().text = "No tienes dinero suficiente para comprar";
                }
            }
        }
    }

    IEnumerator PutData_Coroutine_Dinero()
    {
        string uri = "https://retoiraitz.duckdns.org/api/res.users/";
        //string uri = "http://localhost:8069/api/res.users/";

        int cartera = int.Parse(dinero.GetComponent<TextMeshProUGUI>().text);
        cartera -= (int)precioVenta;

        string jsonData = "{\"x_dinero\":" + cartera + "}";
        string jsonParams = "{\"filter\": [[\"id\", \"=\", " + idUsuario + " ]], \"data\":" + jsonData + "}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"params\":" + jsonParams + "}");
        UnityWebRequest request = UnityWebRequest.Put(uri, "");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            aviso.SetActive(true);
            aviso.GetComponent<TextMeshProUGUI>().text = "Error de conexión";
            Debug.Log(request.error);
        }
        else
        {
            json = request.downloadHandler.text;
            respuestaUsuario = JsonUtility.FromJson<RespuestaUsuario>(json);
            if (respuestaUsuario.error != null)
            {
                aviso.SetActive(true);
                aviso.GetComponent<TextMeshProUGUI>().text = "Error de conexión";
                Debug.Log(request.error);
            }
            else
            {
                resultadoPut = respuestaUsuario.result;
                dinero.GetComponent<TextMeshProUGUI>().text = cartera.ToString();
                PostData();
            }
        }
    }

    IEnumerator PostData_Coroutine()
    {
        string uri = "https://retoiraitz.duckdns.org/api/venta/";
        //string uri = "http://localhost:8069/api/venta/";
        int usuario_id = panelSesion.GetComponent<PostMethod>().idUsuario;
        string tipo = tipoEquipamiento.GetComponent<TextMeshProUGUI>().text;
        string fch_compra = DateTime.Now.ToString("yyyy-MM-dd");
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
        int currency_id = 1;
        string state = "comprado";
        string jsonData = "{\"usuario_id\":" + usuario_id + ", \"tipo\":\"" + tipo + "\", \"fch_compra\":\"" + fch_compra + "\"," +
            " \"" + equipamiento_id + "\":" + arma_id + ", \"currency_id\":" + currency_id + ", \"state\":\"" + state + "\"}";
        string jsonParams = "{\"data\":" + jsonData + "}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"params\":" + jsonParams + "}");
        UnityWebRequest request = UnityWebRequest.PostWwwForm(uri, "");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            aviso.SetActive(true);
            aviso.GetComponent<TextMeshProUGUI>().text = "Error de conexión";
            Debug.Log(request.error);
        }
        else
        {
            json = request.downloadHandler.text;
            respuestaVenta = JsonUtility.FromJson<RespuestaVenta>(json);
            if (respuestaVenta.error != null)
            {
                aviso.SetActive(true);
                aviso.GetComponent<TextMeshProUGUI>().text = "Error de conexión";
                Debug.Log(request.error);
            }
            else
            {
                idVenta = respuestaVenta.result;
                comprar.SetActive(false);
                aviso.SetActive(true);
            }
        }
    }
}
