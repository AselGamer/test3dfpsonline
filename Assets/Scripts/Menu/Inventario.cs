using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    public GameObject panelJugar;
    public GameObject articulo;
    public Arma arma;
    public int click;
    public Vector3 pos;
    public string json;
    public GameObject siguiente;

    // Start is called before the first frame update
    void Start()
    {
        click = 0;
        pos = articulo.transform.position;
        articulo.GetComponent<Button>().onClick.AddListener(GetData);
    }

    public void GetData() => StartCoroutine(GetData_Coroutine());

    public IEnumerator GetData_Coroutine()
    {
        string uri = "https://retoiraitz.duckdns.org/api/arma/" + articulo.name + "/?query={id, name, descripcion, danyo, velocidad, precio, imagen, ventas, priority}";
        //string uri = "http://localhost:8069/api/arma/" + articulo.name + "/?query={id, name, descripcion, danyo, velocidad, precio, imagen, ventas, priority}";
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
                click++;
                if (click%2 == 1)
                {
                    if (panelJugar.GetComponent<Seleccion>().armasSeleccionadas[0] == null || panelJugar.GetComponent<Seleccion>().armasSeleccionadas[0].id == 0)
                    {
                        panelJugar.GetComponent<Seleccion>().armasSeleccionadas[0] = arma;
                        articulo.transform.position = new Vector3(3, -3.5f, articulo.transform.position.z);
                    }
                    else if (panelJugar.GetComponent<Seleccion>().armasSeleccionadas[1] == null || panelJugar.GetComponent<Seleccion>().armasSeleccionadas[1].id == 0)
                    {
                        panelJugar.GetComponent<Seleccion>().armasSeleccionadas[1] = arma;
                        articulo.transform.position = new Vector3(5, -3.5f, articulo.transform.position.z);
                    }
                    else if (panelJugar.GetComponent<Seleccion>().armasSeleccionadas[2] == null || panelJugar.GetComponent<Seleccion>().armasSeleccionadas[2].id == 0)
                    {
                        panelJugar.GetComponent<Seleccion>().armasSeleccionadas[2] = arma;
                        articulo.transform.position = new Vector3(7, -3.5f, articulo.transform.position.z);
                    }
                }
                else
                {
                    if (panelJugar.GetComponent<Seleccion>().armasSeleccionadas[2] != null && panelJugar.GetComponent<Seleccion>().armasSeleccionadas[2].id == arma.id)
                    {
                        panelJugar.GetComponent<Seleccion>().armasSeleccionadas[2] = null;
                        articulo.transform.position = pos;
                    }
                    else if (panelJugar.GetComponent<Seleccion>().armasSeleccionadas[1] != null && panelJugar.GetComponent<Seleccion>().armasSeleccionadas[1].id == arma.id)
                    {
                        panelJugar.GetComponent<Seleccion>().armasSeleccionadas[1] = null;
                        articulo.transform.position = pos;
                    }
                    else if (panelJugar.GetComponent<Seleccion>().armasSeleccionadas[0] != null && panelJugar.GetComponent<Seleccion>().armasSeleccionadas[0].id == arma.id)
                    {
                        panelJugar.GetComponent<Seleccion>().armasSeleccionadas[0] = null;
                        articulo.transform.position = pos;
                    }
                }

                Debug.Log("id---->" + panelJugar.GetComponent<Seleccion>().armasSeleccionadas[0]);
                if(panelJugar.GetComponent<Seleccion>().armasSeleccionadas[0] != null && panelJugar.GetComponent<Seleccion>().armasSeleccionadas[0].id != 0 &&
                    panelJugar.GetComponent<Seleccion>().armasSeleccionadas[1] != null && panelJugar.GetComponent<Seleccion>().armasSeleccionadas[1].id != 0 &&
                    panelJugar.GetComponent<Seleccion>().armasSeleccionadas[2] != null && panelJugar.GetComponent<Seleccion>().armasSeleccionadas[2].id != 0)
                {
                    siguiente.SetActive(true);
                }
                else
                {
                    siguiente.SetActive(false);
                }
            }
        }
    }
}
