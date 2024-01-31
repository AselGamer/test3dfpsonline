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
    public Resultado resultado;
    public GameObject[] articulos;
    public GameObject articulo;
    public string tipo;
    public Arma[] armas;
    public Armadura[] armaduras;
    public GameObject panelArmas;
    public GameObject panelDetalle;
    public GameObject atras;
    public GameObject atrasArma, atrasArmadura;
    public GameObject bienvenida;
    public GameObject nombre, descripcion, danyo, velocidad, precio, defensa;

    public void Start()
    {
        //botonArma.GetComponent<Button>().onClick.AddListener(GetData);
        //articulos[int.Parse(articulo.name)]
        articulo.GetComponent<Button>().onClick.AddListener(GetData);
    }

    public void GetData() => StartCoroutine(GetData_Coroutine());
    
    public void GetDataArmas() => StartCoroutine(GetData_Coroutine_Armas("arma"));
    public void GetDataArmaduras() => StartCoroutine(GetData_Coroutine_Armas("armadura"));

    //Obtener todas las armas
    public IEnumerator GetData_Coroutine_Armas(string tipo)
    {
        this.tipo = tipo;
        articulo.GetComponent<GetMethod>().tipo = tipo;
        outputArea.text = "Loading...";
        string uri = "";
        if(tipo == "arma")
        {
            atrasArma.SetActive(true);
            atrasArmadura.SetActive(false);
            //string uri = "https://retoiraitz.duckdns.org/api/arma/?query={id, name, descripcion, danyo, velocidad, precio, imagen, ventas, priority}";
            uri = "http://localhost:8069/api/arma/?query={id, name, descripcion, danyo, velocidad, precio, imagen, ventas, priority}";
        }else if(tipo == "armadura")
        {
            atrasArma.SetActive(false);
            atrasArmadura.SetActive(true);
            //string uri = "https://retoiraitz.duckdns.org/api/armadura/?query={id, name, descripcion, defensa, precio, imagen, ventas, priority}";
            uri = "http://localhost:8069/api/armadura/?query={id, name, descripcion, defensa, precio, imagen, ventas, priority}";
        }

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
                resultado = JsonUtility.FromJson<Resultado>(json);
                outputArea.text = resultado.count + "\n" + resultado.result[0].imagen;
                //armas = resultado.result;

                //Pasamos de Base64 a sprite
                /*byte[] imageBytes = Convert.FromBase64String(resultado.result[0].imagen);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imageBytes);
                Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                var gameObject = new GameObject(resultado.result[0].name);
                gameObject.transform.position = new Vector3(-6.5f, -0.5f, gameObject.transform.position.z);
                var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;*/

                articulos = new GameObject[resultado.count];
                armas = new Arma[resultado.count];
                /*GameObject articulo2 = Instantiate(articulo, generador.transform.parent);
                articulo2.transform.position = new Vector3(articulo.transform.position.x + 4, 
                    articulo2.transform.position.y, articulo2.transform.position.z);*/
                //Pasamos de Base64 a sprite
                for (int i=0; i<resultado.count; i++)
                {
                    Debug.Log(i + "++++++++++++");
                    if (i == 0)
                    {
                        articulos[i] = articulo;
                    }
                    else
                    {
                        articulos[i] = Instantiate(articulo, articulo.transform.parent);
                        if (i <= 5)
                        {
                            articulos[i].transform.position = new Vector3(articulos[i-1].transform.position.x + 3,
                                articulos[i].transform.position.y, articulos[i].transform.position.z);
                        }
                        else
                        {
                            articulos[i].transform.position = new Vector3(articulos[i-6].transform.position.x,
                                articulos[i-6].transform.position.y - 2, articulos[i].transform.position.z);
                        }
                    }
                    byte[] imageBytes = Convert.FromBase64String(resultado.result[i].imagen);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageBytes);
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    articulos[i].GetComponent<Image>().sprite = sprite;
                    articulos[i].name = resultado.result[i].id.ToString();
                    armas[i] = resultado.result[i];

                    /*var gameObject = new GameObject(resultado.result[i].name);
                    gameObject.transform.position = new Vector3((i-(6.5f-2*i)), -0.5f, gameObject.transform.position.z);
                    var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = sprite;
                    gameObject.AddComponent<Button>();
                    gameObject.AddComponent<RectTransform>();*/
                    //boton = gameObject.GetComponent<Button>();
                    //boton.onClick.AddListener(avanzarAlPanelDetalle);
                    //gameObject.AddComponent<Detalles>();
                }
            }
        }
    }

    /*private void avanzarAlPanelDetalle()
    {
        Debug.Log("click");
        GameObject.Find("Evento").GetComponent<Eventos>().avanzarAlPanelDetalle();
    }*/

    //Obtener un arma mediante su id
    public IEnumerator GetData_Coroutine()
    {
        //articulo.name -> El id del articulo
        Debug.Log(articulo.name + "-------------");
        
        for(int i=0; i<panelArmas.GetComponent<GetMethod>().articulos.Length; i++)
        {
            if(articulo.name != panelArmas.GetComponent<GetMethod>().articulos[i].name)
            {
                panelArmas.GetComponent<GetMethod>().articulos[i].SetActive(false);
            }
        }
        articulo.transform.position = new Vector3(-7, -0.5f, articulo.transform.position.z);
        atras.SetActive(false);
        bienvenida.SetActive(false);
        panelDetalle.SetActive(true);

        outputArea.text = "Loading...";

        if(tipo == "arma")
        {
            //string uri = "https://retoiraitz.duckdns.org/api/arma/" + articulo.name + "/?query={id, name, descripcion, danyo, velocidad, precio, imagen, ventas, priority}";
            string uri = "http://localhost:8069/api/arma/" + articulo.name + "/?query={id, name, descripcion, danyo, velocidad, precio, imagen, ventas, priority}";
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
                    arma = JsonUtility.FromJson<Arma>(json);
                    outputArea.text = arma.id + "\n" + arma.name + "\n" + arma.descripcion + "\n" + arma.danyo
                        + "\n" + arma.velocidad + "\n" + arma.precio + "\n" + arma.ventas + "\n" + arma.priority;

                    //Pasamos de Base64 a sprite
                    /*byte[] imageBytes = Convert.FromBase64String(arma.imagen);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageBytes);
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);*/
                    /*var gameObject = new GameObject(arma.name);
                    gameObject.transform.position = new Vector3(-6.5f, -0.5f, gameObject.transform.position.z);
                    var spriteRenderer = gameObject.AddComponent<SpriteRenderer>(); //Añade un componente
                    spriteRenderer.sprite = sprite;*/

                    //articulos[0].GetComponent<Image>().sprite = sprite;
                    danyo.SetActive(true);
                    nombre.GetComponent<TextMeshProUGUI>().text = arma.name;
                    descripcion.GetComponent<TextMeshProUGUI>().text = arma.descripcion;
                    danyo.GetComponent<TextMeshProUGUI>().text = "Daño: " + arma.danyo;
                    velocidad.GetComponent<TextMeshProUGUI>().text = "Velocidad: " + arma.velocidad.ToString();
                    precio.GetComponent<TextMeshProUGUI>().text = "Precio: " + arma.precio + "€";
                }
            }
        }else if(tipo == "armadura")
        {
            //string uri = "https://retoiraitz.duckdns.org/api/armadura/" + articulo.name + "/?query={id, name, descripcion, defensa, precio, imagen, ventas, priority}";
            string uri = "http://localhost:8069/api/armadura/" + articulo.name + "/?query={id, name, descripcion, defensa, precio, imagen, ventas, priority}";
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
                    armadura = JsonUtility.FromJson<Armadura>(json);
                    outputArea.text = armadura.id + "\n" + armadura.name + "\n" + armadura.descripcion + "\n"
                        + "\n" + armadura.defensa + "\n" + armadura.precio + "\n" + armadura.ventas + "\n" + armadura.priority;

                    //Pasamos de Base64 a sprite
                    /*byte[] imageBytes = Convert.FromBase64String(arma.imagen);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageBytes);
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);*/
                    /*var gameObject = new GameObject(arma.name);
                    gameObject.transform.position = new Vector3(-6.5f, -0.5f, gameObject.transform.position.z);
                    var spriteRenderer = gameObject.AddComponent<SpriteRenderer>(); //Añade un componente
                    spriteRenderer.sprite = sprite;*/

                    //articulos[0].GetComponent<Image>().sprite = sprite;
                    danyo.SetActive(false);
                    nombre.GetComponent<TextMeshProUGUI>().text = armadura.name;
                    descripcion.GetComponent<TextMeshProUGUI>().text = armadura.descripcion;
                    defensa.GetComponent<TextMeshProUGUI>().text = "Defensa: " + armadura.defensa.ToString();
                    precio.GetComponent<TextMeshProUGUI>().text = "Precio: " + armadura.precio + "€";
                }
            }
        }
        
    }


}
