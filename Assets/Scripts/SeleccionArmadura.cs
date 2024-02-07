using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SeleccionArmadura : MonoBehaviour
{
    public string json;
    public GameObject articulo;
    public Armadura[] armadurasSeleccionadas;
    public ResultadoVenta resultadoVenta;
    public GameObject[] articulos;
    public Venta[] ventas;
    public GameObject panelSesion;
    public int idUsuario;
    public GameObject aviso;

    // Start is called before the first frame update
    void Start()
    {
        armadurasSeleccionadas = new Armadura[1];
    }

    public void GetDataArmaduras() => StartCoroutine(GetData_Coroutine_Armaduras());

    public IEnumerator GetData_Coroutine_Armaduras()
    {
        string uri = "";
        int usuario_id = panelSesion.GetComponent<PostMethod>().idUsuario;
        uri = "https://retoiraitz.duckdns.org/api/venta/?query={*}&filter=[[\"usuario_id\", \"=\", " + usuario_id + "], [\"tipo\", \"=\", \"armadura\"], [\"state\", \"=\", \"comprado\"]]";
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
                    byte[] imageBytes = Convert.FromBase64String(resultadoVenta.result[i].armadura_imagen);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageBytes);
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                    articulos[i].GetComponent<Image>().sprite = sprite;
                    articulos[i].name = resultadoVenta.result[i].armadura_id.ToString();
                    ventas[i] = resultadoVenta.result[i];
                }
                if (resultadoVenta.count < 1)
                {
                    articulo.SetActive(false);
                }
            }
        }
    }
}
