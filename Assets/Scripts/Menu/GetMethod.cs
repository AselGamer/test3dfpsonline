using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using UnityEditor;


public class GetMethod : MonoBehaviour
{
    public TMP_InputField outputArea;
    public string json;
    public Arma arma;
    public Armadura armadura;
    public Medicamento medicamento;
    public Resultado resultado;
    //public ResultadoVenta resultadoVenta;
    public GameObject[] articulos;
    public GameObject articulo;
    public string tipo;
    public Arma[] armas;
    public Arma[] armasSeleccionadas;
    //public Venta[] ventas;
    public Armadura[] armaduras;
    public GameObject panelArmas;
    public GameObject panelDetalle;
    public GameObject atras;
    public GameObject atrasArma, atrasArmadura, atrasMedicamento;
    public GameObject bienvenida;
    public GameObject id, tipoEquipamiento, nombre, descripcion, danyo, velocidad, precio, defensa, curacion;
    public Usuario usuario;
    public GameObject dinero;
    public GameObject aviso;
    public GameObject panelSesion;
    public ResultadoVenta resultadoVenta;
    public GameObject comprar;

    public void Start()
    {
        articulo.GetComponent<Button>().onClick.AddListener(GetData);
    }

    public void GetData() => StartCoroutine(GetData_Coroutine());
    
    public void GetDataArmas() => StartCoroutine(GetData_Coroutine_Armas("arma"));
    public void GetDataArmaduras() => StartCoroutine(GetData_Coroutine_Armas("armadura"));
    public void GetDataMedicamentos() => StartCoroutine(GetData_Coroutine_Armas("medicamento"));

    public void GetDataDinero(int idUsuario) => StartCoroutine(GetData_Coroutine_Dinero(idUsuario));

    //Obtener todas las armas
    public IEnumerator GetData_Coroutine_Armas(string tipo)
    {
        this.tipo = tipo;
        articulo.GetComponent<GetMethod>().tipo = tipo;
        string uri = "";
        if (tipo == "arma")
        {
            atrasArma.SetActive(true);
            atrasArmadura.SetActive(false);
            atrasMedicamento.SetActive(false);
            uri = "https://retoiraitz.duckdns.org/api/arma/?query={id, name, descripcion, danyo, velocidad, precio, imagen, ventas, priority}";
            //uri = "http://localhost:8069/api/arma/?query={id, name, descripcion, danyo, velocidad, precio, imagen, ventas, priority}";
        }
        else if (tipo == "armadura")
        {
            atrasArma.SetActive(false);
            atrasArmadura.SetActive(true);
            atrasMedicamento.SetActive(false);
            uri = "https://retoiraitz.duckdns.org/api/armadura/?query={id, name, descripcion, defensa, precio, imagen, ventas, priority}";
            //uri = "http://localhost:8069/api/armadura/?query={id, name, descripcion, defensa, precio, imagen, ventas, priority}";
        }
        else if (tipo == "medicamento")
        {
            atrasArma.SetActive(false);
            atrasArmadura.SetActive(false);
            atrasMedicamento.SetActive(true);
            uri = "https://retoiraitz.duckdns.org/api/medicamento/?query={id, name, descripcion, curacion, precio, imagen, ventas, priority}";
            //uri = "http://localhost:8069/api/medicamento/?query={id, name, descripcion, curacion, precio, imagen, ventas, priority}";
        }

        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
                articulo.SetActive(true);
                json = request.downloadHandler.text;
                resultado = JsonUtility.FromJson<Resultado>(json);
                articulos = new GameObject[resultado.count];
                armas = new Arma[resultado.count];

                //Pasamos de Base64 a sprite
                for (int i = 0; i < resultado.count; i++)
                {
                    if (i == 0)
                    {
                        articulos[i] = articulo;
                    }
                    else
                    {
                        articulos[i] = Instantiate(articulo, articulo.transform.parent);
                        if (i <= 5)
                        {
                            articulos[i].transform.position = new Vector3(articulos[i - 1].transform.position.x + 3,
                                articulos[i].transform.position.y, articulos[i].transform.position.z);
                        }
                        else
                        {
                            articulos[i].transform.position = new Vector3(articulos[i - 6].transform.position.x,
                                articulos[i - 6].transform.position.y - 2, articulos[i].transform.position.z);
                        }
                    }
                    byte[] imageBytes = Convert.FromBase64String(resultado.result[i].imagen);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageBytes);
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    articulos[i].GetComponent<Image>().sprite = sprite;
                    articulos[i].name = resultado.result[i].id.ToString();
                    armas[i] = resultado.result[i];
                }
            }
        }
    }

    //Obtener un arma mediante su id
    public IEnumerator GetData_Coroutine()
    {
        //articulo.name -> El id del articulo
        comprar.SetActive(false);
        aviso.SetActive(false);

        for (int i = 0; i < panelArmas.GetComponent<GetMethod>().articulos.Length; i++)
        {
            if (articulo.name != panelArmas.GetComponent<GetMethod>().articulos[i].name)
            {
                panelArmas.GetComponent<GetMethod>().articulos[i].SetActive(false);
            }
        }

        articulo.transform.position = new Vector3(-7, -0.5f, articulo.transform.position.z);
        atras.SetActive(false);
        bienvenida.SetActive(false);
        panelDetalle.SetActive(true);

        string uri = "";
        if (tipo == "arma")
        {
            uri = "https://retoiraitz.duckdns.org/api/arma/" + articulo.name + "/?query={id, name, descripcion, danyo, velocidad, precio, imagen, ventas, priority}";
            //uri = "http://localhost:8069/api/arma/" + articulo.name + "/?query={id, name, descripcion, danyo, velocidad, precio, imagen, ventas, priority}";
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    json = request.downloadHandler.text;
                    arma = JsonUtility.FromJson<Arma>(json);

                    //Pasamos de Base64 a sprite
                    danyo.SetActive(true);
                    id.GetComponent<TextMeshProUGUI>().text = arma.id.ToString();
                    tipoEquipamiento.GetComponent<TextMeshProUGUI>().text = tipo;
                    nombre.GetComponent<TextMeshProUGUI>().text = arma.name;
                    descripcion.GetComponent<TextMeshProUGUI>().text = arma.descripcion;
                    danyo.GetComponent<TextMeshProUGUI>().text = "Daño: " + arma.danyo;
                    velocidad.GetComponent<TextMeshProUGUI>().text = "Velocidad: " + arma.velocidad.ToString();
                    precio.GetComponent<TextMeshProUGUI>().text = arma.precio.ToString();
                }
            }
        }
        else if (tipo == "armadura")
        {
            uri = "https://retoiraitz.duckdns.org/api/armadura/" + articulo.name + "/?query={id, name, descripcion, defensa, precio, imagen, ventas, priority}";
            //uri = "http://localhost:8069/api/armadura/" + articulo.name + "/?query={id, name, descripcion, defensa, precio, imagen, ventas, priority}";
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    json = request.downloadHandler.text;
                    armadura = JsonUtility.FromJson<Armadura>(json);

                    //Pasamos de Base64 a sprite
                    danyo.SetActive(false);
                    id.GetComponent<TextMeshProUGUI>().text = armadura.id.ToString();
                    tipoEquipamiento.GetComponent<TextMeshProUGUI>().text = tipo;
                    nombre.GetComponent<TextMeshProUGUI>().text = armadura.name;
                    descripcion.GetComponent<TextMeshProUGUI>().text = armadura.descripcion;
                    defensa.GetComponent<TextMeshProUGUI>().text = "Defensa: " + armadura.defensa.ToString();
                    precio.GetComponent<TextMeshProUGUI>().text = armadura.precio.ToString();
                }
            }
        }
        else if (tipo == "medicamento")
        {
            uri = "https://retoiraitz.duckdns.org/api/medicamento/" + articulo.name + "/?query={id, name, descripcion, curacion, precio, imagen, ventas, priority}";
            //uri = "http://localhost:8069/api/medicamento/" + articulo.name + "/?query={id, name, descripcion, curacion, precio, imagen, ventas, priority}";
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    json = request.downloadHandler.text;
                    medicamento = JsonUtility.FromJson<Medicamento>(json);

                    //Pasamos de Base64 a sprite
                    danyo.SetActive(false);
                    id.GetComponent<TextMeshProUGUI>().text = medicamento.id.ToString();
                    tipoEquipamiento.GetComponent<TextMeshProUGUI>().text = tipo;
                    nombre.GetComponent<TextMeshProUGUI>().text = medicamento.name;
                    descripcion.GetComponent<TextMeshProUGUI>().text = medicamento.descripcion;
                    curacion.GetComponent<TextMeshProUGUI>().text = "Defensa: " + medicamento.curacion.ToString();
                    precio.GetComponent<TextMeshProUGUI>().text = medicamento.precio.ToString();
                }
            }
        }

        //Buscamos si el articulo está comprado por el usuario
        int usuario_id = panelSesion.GetComponent<PostMethod>().idUsuario;
        uri = "https://retoiraitz.duckdns.org/api/venta/?query={*}&filter=[[\"usuario_id\", \"=\", " + usuario_id + "], [\"tipo\", \"=\", \"" + tipo + "\"], [\"state\", \"=\", \"comprado\"]]";
        //uri = "http://localhost:8069/api/venta/?query={*}&filter=[[\"usuario_id\", \"=\", " + usuario_id + "], [\"tipo\", \"=\", \"arma\"], [\"state\", \"=\", \"comprado\"]]";

        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
                json = request.downloadHandler.text;
                resultadoVenta = JsonUtility.FromJson<ResultadoVenta>(json);
                //ventas = new Venta[resultadoVenta.count];

                int equipamiento_id = int.Parse(id.GetComponent<TextMeshProUGUI>().text);
                bool encontrado = false;

                for (int i = 0; i < resultadoVenta.count; i++)
                {
                    if (tipo == "arma")
                    {
                        if (equipamiento_id == resultadoVenta.result[i].arma_id)
                        {
                            encontrado = true;
                            break;
                        }
                    }
                    else if (tipo == "armadura")
                    {
                        if (equipamiento_id == resultadoVenta.result[i].armadura_id)
                        {
                            encontrado = true;
                            break;
                        }
                    }
                    else if (tipo == "medicamento")
                    {
                        if (equipamiento_id == resultadoVenta.result[i].medicamento_id)
                        {
                            encontrado = true;
                            break;
                        }
                    }
                }

                if (encontrado)
                {
                    aviso.SetActive(true);
                    comprar.SetActive(false);
                }
                else
                {
                    aviso.SetActive(false);
                    comprar.SetActive(true);
                }
            }
        }
    }

    IEnumerator GetData_Coroutine_Dinero(int idUsuario)
    {
        string uri = "https://retoiraitz.duckdns.org/api/res.users/" + idUsuario + "/?query={id, name, x_dinero}";
        //string uri = "http://localhost:8069/api/res.users/" + idUsuario + "/?query={id, name, x_dinero}";
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                outputArea.text = request.error;
            }
            else
            {
                json = request.downloadHandler.text;
                usuario = JsonUtility.FromJson<Usuario>(json);

                dinero.GetComponent<TextMeshProUGUI>().text = usuario.x_dinero.ToString();
            }
        }
    }
}
