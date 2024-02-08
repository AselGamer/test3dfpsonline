using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InventarioMedicamento : MonoBehaviour
{
    public GameObject panelJugar3;
    public GameObject articulo;
    public Medicamento medicamento;
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
        string uri = "https://retoiraitz.duckdns.org/api/medicamento/" + articulo.name + "/?query={id, name, descripcion, curacion, precio, imagen, ventas, priority}";
        //string uri = "http://localhost:8069/api/medicamento/" + articulo.name + "/?query={id, name, descripcion, curacion, precio, imagen, ventas, priority}";
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

                click++;
                if (click % 2 == 1)
                {
                    if (panelJugar3.GetComponent<SeleccionMedicamento>().medicamentosSeleccionados[0] == null || panelJugar3.GetComponent<SeleccionMedicamento>().medicamentosSeleccionados[0].id == 0)
                    {
                        panelJugar3.GetComponent<SeleccionMedicamento>().medicamentosSeleccionados[0] = medicamento;
                        articulo.transform.position = new Vector3(4, -3.5f, articulo.transform.position.z);
                    }
                }
                else
                {
                    if (panelJugar3.GetComponent<SeleccionMedicamento>().medicamentosSeleccionados[0] != null && panelJugar3.GetComponent<SeleccionMedicamento>().medicamentosSeleccionados[0].id == medicamento.id)
                    {
                        panelJugar3.GetComponent<SeleccionMedicamento>().medicamentosSeleccionados[0] = null;
                        articulo.transform.position = pos;
                    }
                }
            }
        }
    }
}
