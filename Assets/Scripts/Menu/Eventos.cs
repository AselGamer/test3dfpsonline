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
    public GameObject panelJugar2;
    public GameObject panelJugar3;
    public GameObject empezar;

    public void jugarPartida()
    {
        SceneManager.LoadScene("Client", LoadSceneMode.Single);
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
        panelJugar.GetComponent<Seleccion>().articulo.GetComponent<Inventario>().siguiente.SetActive(false);
        panelJugar.GetComponent<Seleccion>().articulo.SetActive(false);
        panelJugar.GetComponent<Seleccion>().aviso.SetActive(false);
        panelJugar.GetComponent<Seleccion>().GetDataArmas();
    }
    public void retrocederDePanelJugarAlPanelInicio()
    {
        for (int i = 1; i < panelJugar.GetComponent<Seleccion>().articulos.Length; i++)
        {
            Destroy(panelJugar.GetComponent<Seleccion>().articulos[i]);
        }
        if(panelJugar.GetComponent<Seleccion>().articulos.Length > 0)
        {
            panelJugar.GetComponent<Seleccion>().articulos[0].transform.position = new Vector3(-7, -0.5f, panelJugar.GetComponent<Seleccion>().articulos[0].transform.position.z);
            panelJugar.GetComponent<Seleccion>().articulos[0].GetComponent<Inventario>().click = 0;
            for (int i = 0; i < panelJugar.GetComponent<Seleccion>().armasSeleccionadas.Length; i++)
            {
                panelJugar.GetComponent<Seleccion>().armasSeleccionadas[i] = null;
            }
        }
        panelJugar.SetActive(false);
        panelInicio.SetActive(true);
    }
    public void avanzarAlPanelJugar2()
    {
        panelJugar.SetActive(false);
        panelJugar2.SetActive(true);
        panelJugar2.GetComponent<SeleccionArmadura>().articulo.SetActive(false);
        panelJugar2.GetComponent<SeleccionArmadura>().aviso.SetActive(false);
        panelJugar2.GetComponent<SeleccionArmadura>().GetDataArmaduras();
    }
    public void retrocederAlPanelJugar()
    {
        for (int i = 1; i < panelJugar2.GetComponent<SeleccionArmadura>().articulos.Length; i++)
        {
            Destroy(panelJugar2.GetComponent<SeleccionArmadura>().articulos[i]);
        }
        if (panelJugar2.GetComponent<SeleccionArmadura>().articulos.Length > 0)
        {
            panelJugar2.GetComponent<SeleccionArmadura>().articulos[0].transform.position = new Vector3(-7, -0.5f, panelJugar2.GetComponent<SeleccionArmadura>().articulos[0].transform.position.z);
            panelJugar2.GetComponent<SeleccionArmadura>().articulos[0].GetComponent<InventarioArmadura>().click = 0;
            for (int i = 0; i < panelJugar2.GetComponent<SeleccionArmadura>().armadurasSeleccionadas.Length; i++)
            {
                panelJugar2.GetComponent<SeleccionArmadura>().armadurasSeleccionadas[i] = null;
            }
        }
        panelJugar2.SetActive(false);
        panelJugar.SetActive(true);
    }
    public void avanzarAlPanelJugar3()
    {
        panelJugar2.SetActive(false);
        panelJugar3.SetActive(true);
        panelJugar3.GetComponent<SeleccionMedicamento>().articulo.SetActive(false);
        panelJugar3.GetComponent<SeleccionMedicamento>().aviso.SetActive(false);
        panelJugar3.GetComponent<SeleccionMedicamento>().GetDataMedicamentos();
    }
    public void retrocederAlPanelJugar2()
    {
        for (int i = 1; i < panelJugar3.GetComponent<SeleccionMedicamento>().articulos.Length; i++)
        {
            Destroy(panelJugar3.GetComponent<SeleccionMedicamento>().articulos[i]);
        }
        if (panelJugar3.GetComponent<SeleccionMedicamento>().articulos.Length > 0)
        {
            panelJugar3.GetComponent<SeleccionMedicamento>().articulos[0].transform.position = new Vector3(-7, -0.5f, panelJugar3.GetComponent<SeleccionMedicamento>().articulos[0].transform.position.z);
            panelJugar3.GetComponent<SeleccionMedicamento>().articulos[0].GetComponent<InventarioMedicamento>().click = 0;
            for (int i = 0; i < panelJugar3.GetComponent<SeleccionMedicamento>().medicamentosSeleccionados.Length; i++)
            {
                panelJugar3.GetComponent<SeleccionMedicamento>().medicamentosSeleccionados[i] = null;
            }
        }
        panelJugar3.SetActive(false);
        panelJugar2.SetActive(true);
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
        panelArmas.GetComponent<GetMethod>().articulo.SetActive(false);
        panelArmas.GetComponent<GetMethod>().GetDataArmas();
    }
    public void avanzarAlPanelArmaduras()
    {
        panelCompras.SetActive(false);
        panelArmas.SetActive(true);
        panelArmas.GetComponent<GetMethod>().articulo.SetActive(false);
        panelArmas.GetComponent<GetMethod>().GetDataArmaduras();
    }
    public void avanzarAlPanelMedicamentos()
    {
        panelCompras.SetActive(false);
        panelArmas.SetActive(true);
        panelArmas.GetComponent<GetMethod>().articulo.SetActive(false);
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
        if (tipo == "arma")
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
