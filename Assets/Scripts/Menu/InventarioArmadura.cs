using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InventarioArmadura : MonoBehaviour
{
    public GameObject panelJugar2;
    public GameObject articulo;
    public Armadura armadura;
    public int click;
    public Vector3 pos;
    public string json;

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
        string uri = "https://retoiraitz.duckdns.org/api/armadura/" + articulo.name + "/?query={id, name, descripcion, defensa, precio, imagen, ventas, priority}";
        //string uri = "http://localhost:8069/api/armadura/" + articulo.name + "/?query={id, name, descripcion, defensa, precio, imagen, ventas, priority}";
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

                click++;
                if (click % 2 == 1)
                {
                    if (panelJugar2.GetComponent<SeleccionArmadura>().armadurasSeleccionadas[0] == null || panelJugar2.GetComponent<SeleccionArmadura>().armadurasSeleccionadas[0].id == 0)
                    {
                        panelJugar2.GetComponent<SeleccionArmadura>().armadurasSeleccionadas[0] = armadura;
                        articulo.transform.position = new Vector3(3, -3.5f, articulo.transform.position.z);
                    }
                }
                else
                {
                    if (panelJugar2.GetComponent<SeleccionArmadura>().armadurasSeleccionadas[0] != null && panelJugar2.GetComponent<SeleccionArmadura>().armadurasSeleccionadas[0].id == armadura.id)
                    {
                        panelJugar2.GetComponent<SeleccionArmadura>().armadurasSeleccionadas[0] = null;
                        articulo.transform.position = pos;
                    }
                }
            }
        }
    }
}
