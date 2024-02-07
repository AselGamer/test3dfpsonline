using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Seleccion : MonoBehaviour
{
    public string json;
    public GameObject articulo;
    public Arma[] armasSeleccionadas;
    public ResultadoVenta resultadoVenta;
    public GameObject[] articulos;
    public Venta[] ventas;
    public GameObject panelSesion;
    public int idUsuario;
    public GameObject aviso;

    // Start is called before the first frame update
    void Start()
    {
        armasSeleccionadas = new Arma[3];
    }

    private void FixedUpdate()
    {
        var arrArmasJug = new short[3];
        for (int i = 0; i < armasSeleccionadas.Length; i++)
        {
            if (armasSeleccionadas[i] != null)
            {
                arrArmasJug[i] = (short)armasSeleccionadas[i].id;
            }
            
        }
        PerfilJugador.Armas = arrArmasJug;
    }

    public void GetDataArmas() => StartCoroutine(GetData_Coroutine_Armas());

    public IEnumerator GetData_Coroutine_Armas()
    {
        //this.tipo = tipo;
        //articulo.GetComponent<GetMethod>().tipo = tipo;
        //outputArea.text = "Loading...";
        string uri = "";
        int usuario_id = panelSesion.GetComponent<PostMethod>().idUsuario;
        uri = "https://retoiraitz.duckdns.org/api/venta/?query={*}&filter=[[\"usuario_id\", \"=\", " + usuario_id + "], [\"tipo\", \"=\", \"arma\"], [\"state\", \"=\", \"comprado\"]]";
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
                articulo.SetActive(true);
                json = request.downloadHandler.text;
                resultadoVenta = JsonUtility.FromJson<ResultadoVenta>(json);
                articulos = new GameObject[resultadoVenta.count];
                ventas = new Venta[resultadoVenta.count];

                //Pasamos de Base64 a sprite
                for (int i = 0; i < resultadoVenta.count; i++)
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
                            articulos[i].transform.position = new Vector3(articulos[i - 1].transform.position.x + 3,
                                articulos[i].transform.position.y, articulos[i].transform.position.z);
                        }
                        else
                        {
                            articulos[i].transform.position = new Vector3(articulos[i - 6].transform.position.x,
                                articulos[i - 6].transform.position.y - 2, articulos[i].transform.position.z);
                        }
                    }
                    byte[] imageBytes = Convert.FromBase64String(resultadoVenta.result[i].arma_imagen);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageBytes);
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    articulos[i].GetComponent<Image>().sprite = sprite;
                    articulos[i].name = resultadoVenta.result[i].arma_id.ToString();
                    ventas[i] = resultadoVenta.result[i];
                }
                if (resultadoVenta.count < 3)
                {
                    aviso.SetActive(true);
                }
            }
        }
    }
}
