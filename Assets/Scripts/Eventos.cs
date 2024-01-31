using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Eventos : MonoBehaviour
{
    public GameObject panelSesion;
    public GameObject panelInicio;
    public GameObject panelCompras;
    public GameObject panelArmas;
    public GameObject panelDetalle;

    public void retrocederAlPanelSesion()
    {
        panelInicio.SetActive(false);
        panelSesion.SetActive(true);
        panelSesion.GetComponent<PostMethod>().isUsuario = false;
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
        panelArmas.GetComponent<GetMethod>().GetDataArmas();
    }
    public void avanzarAlPanelArmaduras()
    {
        panelCompras.SetActive(false);
        panelArmas.SetActive(true);
        panelArmas.GetComponent<GetMethod>().GetDataArmaduras();
    }
    public void avanzarAlPanelMedicamentos()
    {
        panelCompras.SetActive(false);
        panelArmas.SetActive(true);
        panelArmas.GetComponent<GetMethod>().GetDataMedicamentos();
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
            panelArmas.GetComponent<GetMethod>().GetDataArmas();
        }else if (tipo == "armadura")
        {
            panelArmas.GetComponent<GetMethod>().GetDataArmaduras();
        }
        else if (tipo == "medicamento")
        {
            panelArmas.GetComponent<GetMethod>().GetDataMedicamentos();
        }
        for (int i = 0; i < panelArmas.GetComponent<GetMethod>().articulos.Length; i++)
        {
            panelArmas.GetComponent<GetMethod>().articulos[i].SetActive(true);
        }
    }
}
