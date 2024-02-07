using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class CrearUsuario : MonoBehaviour
{
    public GameObject mensaje;
    public string json;
    public Respuesta respuesta;
    public GameObject panelSesion;
    public GameObject panelRegistro;
    public GameObject crearUsuario;
    public string nombre;
    public string password;

    // Start is called before the first frame update
    void Start()
    {
        nombre = GameObject.Find("Signup Usuario").GetComponent<TMP_InputField>().text;
        password = GameObject.Find("Signup Password").GetComponent<TMP_InputField>().text;
        crearUsuario.GetComponent<Button>().onClick.AddListener(PostData_Crear);
    }

    private void Update()
    {
        nombre = GameObject.Find("Signup Usuario").GetComponent<TMP_InputField>().text;
        password = GameObject.Find("Signup Password").GetComponent<TMP_InputField>().text;
    }

    void PostData_Crear() => StartCoroutine(PostData_Coroutine_Crear());

    IEnumerator PostData_Coroutine_Crear()
    {
        if(nombre == "" || password == "")
        {
            mensaje.SetActive(true);
            mensaje.GetComponent<TextMeshProUGUI>().text = "Los campos no pueden estar vacíos";
        }
        else
        {
            string uri = "https://retoiraitz.duckdns.org/api/res.users/";
            //string uri = "http://localhost:8069/api/res.users/";
            int x_dinero = 10000;
            int permiso = 14;

            string jsonData = "{\"name\":\"" + nombre + "\", \"login\":\"" + nombre + "\", \"password\":\"" + password + "\"," +
                " \"x_dinero\":" + x_dinero + ", \"sel_groups_14_15\":\"" + permiso + "\"}";
            string jsonParams = "{\"data\":" + jsonData + "}";
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"params\":" + jsonParams + "}");
            UnityWebRequest request = UnityWebRequest.PostWwwForm(uri, "");
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                mensaje.SetActive(true);
                mensaje.GetComponent<TextMeshProUGUI>().text = "Error de conexión";
                Debug.Log(request.error);
            }
            else
            {
                json = request.downloadHandler.text;
                respuesta = JsonUtility.FromJson<Respuesta>(json);
                if (respuesta.error != null)
                {
                    Debug.Log(respuesta.error);
                    mensaje.SetActive(true);
                    mensaje.GetComponent<TextMeshProUGUI>().text = "El usuario ya existe";
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);
                    mensaje.SetActive(false);
                    panelRegistro.SetActive(false);
                    panelSesion.SetActive(true);
                    panelSesion.GetComponent<PostMethod>().mensaje.SetActive(true);
                    panelSesion.GetComponent<PostMethod>().mensaje.GetComponent<TextMeshProUGUI>().text = "Usuario creado";
                }
            }
        }
    }
}
