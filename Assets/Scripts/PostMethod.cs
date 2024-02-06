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
    public GameObject mensaje;
    public TextMeshProUGUI bienvenida;
    public GameObject moneda;
    public GameObject dinero;
    public int idUsuario;

    void Start()
    {
        nombre = GameObject.Find("Login Usuario").GetComponent<TMP_InputField>().text;
        password = GameObject.Find("Login Password").GetComponent<TMP_InputField>().text;
        GameObject.Find("Iniciar Sesion").GetComponent<Button>().onClick.AddListener(PostData);
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

    IEnumerator PostData_Coroutine()
    {
        outputArea.text = "Loading...";
        string uri = "https://retoiraitz.duckdns.org/auth";
        //string uri = "http://localhost:8069/auth";
        //WWWForm form = new WWWForm();
        //form.AddField("params", body);
        //using (UnityWebRequest request = UnityWebRequest.Post(uri, form))
        //{
        /*string login = "admin";
        string contrasena = "Almi123";*/
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
            outputArea.text = request.error;
            mensaje.SetActive(true);
            mensaje.GetComponent<TextMeshProUGUI>().text = "Error de conexión";
            Debug.Log("Error de conexion");
        }
        else
        {
            json = request.downloadHandler.text;
            respuesta = JsonUtility.FromJson<Respuesta>(json);
            if (respuesta.error != null)
            {
                mensaje.SetActive(true);
                mensaje.GetComponent<TextMeshProUGUI>().text = "Usuario o contraseña no válidos";
                Debug.Log("Error de usuario " + respuesta.error);
            }
            else
            {
                idUsuario = respuesta.result.uid;
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
        //}
    }
}
