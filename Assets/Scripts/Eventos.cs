using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using TMPro;

public class Eventos : MonoBehaviour
{
    public GameObject panelSesion;
    public GameObject panelInicio;
    public GameObject panelCompras;
    public GameObject panelArmas;
    public GameObject panelDetalle;
    public GameObject panelJugar;
    public GameObject empezar;

    public void jugarPartida()
    {
        SceneManager.LoadScene("Client");
    }
    public void retrocederAlPanelSesion()
    {
        panelInicio.SetActive(false);
        panelSesion.SetActive(true);
        panelSesion.GetComponent<PostMethod>().isUsuario = false;
        panelSesion.GetComponent<PostMethod>().moneda.SetActive(false);
        panelSesion.GetComponent<PostMethod>().dinero.SetActive(false);
        GameObject.Find("Login Usuario").GetComponent<TMP_InputField>().text = "";
        GameObject.Find("Login Password").GetComponent<TMP_InputField>().text = "";
    }
    public void avanzarAlPanelJugar()
    {
        panelInicio.SetActive(false);
        panelJugar.SetActive(true);
        panelJugar.GetComponent<Seleccion>().GetDataArmas();
    }
    public void retrocederDePanelJugarAlPanelInicio()
    {
        for (int i = 1; i < panelJugar.GetComponent<Seleccion>().articulos.Length; i++)
        {
            Destroy(panelJugar.GetComponent<Seleccion>().articulos[i]);
        }
        panelJugar.GetComponent<Seleccion>().articulos[0].transform.position = new Vector3(-7, -0.5f, panelJugar.GetComponent<Seleccion>().articulos[0].transform.position.z);
        panelJugar.GetComponent<Seleccion>().articulos[0].GetComponent<Inventario>().click = 0;
        for (int i = 0; i < panelJugar.GetComponent<Seleccion>().armasSeleccionadas.Length; i++)
        {
            panelJugar.GetComponent<Seleccion>().armasSeleccionadas[i] = null;
        }
        panelJugar.SetActive(false);
        panelInicio.SetActive(true);
    }
    public void avanzarAlPanelCompras()
    {
        panelInicio.SetActive(false);
        panelCompras.SetActive(true);
    }
    public void retrocederAlPanelInicio()
    {
        panelCompras.SetActive(false);
        panelInicio.SetActive(true);
    }
    public void avanzarAlPanelArmas()
    {
        panelCompras.SetActive(false);
        panelArmas.SetActive(true);
        panelArmas.GetComponent<GetMethod>().GetDataArmas("comprar");
    }
    public void avanzarAlPanelArmaduras()
    {
        panelCompras.SetActive(false);
        panelArmas.SetActive(true);
        panelArmas.GetComponent<GetMethod>().GetDataArmaduras("comprar");
    }
    public void avanzarAlPanelMedicamentos()
    {
        panelCompras.SetActive(false);
        panelArmas.SetActive(true);
        panelArmas.GetComponent<GetMethod>().GetDataMedicamentos("comprar");
    }
    public void retrocederAlPanelCompras()
    {
        for(int i=1; i<panelArmas.GetComponent<GetMethod>().articulos.Length; i++)
        {
            Destroy(panelArmas.GetComponent<GetMethod>().articulos[i]);
        }
        panelArmas.SetActive(false);
        panelCompras.SetActive(true);
    }
    public void avanzarAlPanelDetalle()
    {
        panelArmas.SetActive(false);
        panelDetalle.SetActive(true);
    }
    public void retrocederAlPanelArmas(string tipo)
    {
        panelDetalle.SetActive(false);
        panelArmas.SetActive(true);
        panelArmas.GetComponent<GetMethod>().atras.SetActive(true);
        panelArmas.GetComponent<GetMethod>().bienvenida.SetActive(true);
        for (int i = 1; i < panelArmas.GetComponent<GetMethod>().articulos.Length; i++)
        {
            Destroy(panelArmas.GetComponent<GetMethod>().articulos[i]);
        }
        if(tipo == "arma")
        {
            panelArmas.GetComponent<GetMethod>().GetDataArmas("comprar");
        }else if (tipo == "armadura")
        {
            panelArmas.GetComponent<GetMethod>().GetDataArmaduras("comprar");
        }
        else if (tipo == "medicamento")
        {
            panelArmas.GetComponent<GetMethod>().GetDataMedicamentos("comprar");
        }
        for (int i = 0; i < panelArmas.GetComponent<GetMethod>().articulos.Length; i++)
        {
            panelArmas.GetComponent<GetMethod>().articulos[i].SetActive(true);
        }
    }
}
