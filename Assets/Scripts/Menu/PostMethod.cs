using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class PostMethod : MonoBehaviour
{
    public TMP_InputField outputArea;
    public string nombre;
    public string password;
    public string json;
    public Respuesta respuesta;
    public bool isUsuario;
    public GameObject panelSesion;
    public GameObject panelInicio;
    public GameObject panelRegistro;
    public GameObject mensaje;
    public TextMeshProUGUI bienvenida;
    public GameObject moneda;
    public GameObject dinero;
    public GameObject crearUsuario;
    public int idUsuario;

    void Start()
    {
        nombre = GameObject.Find("Login Usuario").GetComponent<TMP_InputField>().text;
        password = GameObject.Find("Login Password").GetComponent<TMP_InputField>().text;
        GameObject.Find("Iniciar Sesion").GetComponent<Button>().onClick.AddListener(PostData);
        crearUsuario.GetComponent<Button>().onClick.AddListener(PostData_Crear);
        isUsuario = false;
    }

    private void Update()
    {
        if(isUsuario == false)
        {
            nombre = GameObject.Find("Login Usuario").GetComponent<TMP_InputField>().text;
            password = GameObject.Find("Login Password").GetComponent<TMP_InputField>().text;
        }
    }

    void PostData() => StartCoroutine(PostData_Coroutine());
    void PostData_Crear() => StartCoroutine(PostData_Coroutine_Crear());

    IEnumerator PostData_Coroutine()
    {
        string uri = "https://retoiraitz.duckdns.org/auth";
        //string uri = "http://localhost:8069/auth";
        string db = "almi";

        string jsonParams = "{\"login\":\"" + nombre + "\", \"password\":\"" + password + "\", \"db\":\"" + db + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"params\":" + jsonParams + "}");
        UnityWebRequest request = UnityWebRequest.PostWwwForm(uri, "");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {

            Debug.Log(request.error);
            mensaje.SetActive(true);
            mensaje.GetComponent<TextMeshProUGUI>().text = "Error de conexión";
        }
        else
        {
            json = request.downloadHandler.text;
            respuesta = JsonUtility.FromJson<Respuesta>(json);
            if (respuesta.error != null)
            {
                Debug.Log(respuesta.error);
                mensaje.SetActive(true);
                mensaje.GetComponent<TextMeshProUGUI>().text = "Usuario o contraseña no válidos";
            }
            else
            {
                idUsuario = respuesta.result.uid;
                PerfilJugador.Id = respuesta.result.uid;
                outputArea.text = request.downloadHandler.text;
                mensaje.SetActive(false);
                panelSesion.SetActive(false);
                panelInicio.SetActive(true);
                moneda.SetActive(true);
                dinero.SetActive(true);
                bienvenida.text = "Hola " + nombre;
                isUsuario = true;
                panelInicio.GetComponent<GetMethod>().GetDataDinero(idUsuario);
            }
        }
    }

    IEnumerator PostData_Coroutine_Crear()
    {
        string uri = "https://retoiraitz.duckdns.org/auth";
        //string uri = "http://localhost:8069/auth";
        string nombre = "Almi";
        string password = "Almi123";
        string db = "almi";

        string jsonParams = "{\"login\":\"" + nombre + "\", \"password\":\"" + password + "\", \"db\":\"" + db + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"params\":" + jsonParams + "}");
        UnityWebRequest request = UnityWebRequest.PostWwwForm(uri, "");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
            mensaje.SetActive(true);
            mensaje.GetComponent<TextMeshProUGUI>().text = "Error de conexión";
        }
        else
        {
            json = request.downloadHandler.text;
            respuesta = JsonUtility.FromJson<Respuesta>(json);
            if (respuesta.error != null)
            {
                Debug.Log(respuesta.error);
                mensaje.SetActive(true);
                mensaje.GetComponent<TextMeshProUGUI>().text = "Error de conexión";
            }
            else
            {
                idUsuario = respuesta.result.uid;
                outputArea.text = request.downloadHandler.text;
                mensaje.SetActive(false);
                panelSesion.SetActive(false);
                panelRegistro.SetActive(true);
            }
        }
    }
}
