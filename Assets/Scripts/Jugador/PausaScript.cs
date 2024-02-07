using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausaScript : MonoBehaviour
{
    public Slider sensitivitySlider; 
    public NetworkClient networkClient;

    public void OnClickContinuar()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }

    public void OnSensitivyChange()
    {
        Debug.Log("Sensitivity: " + sensitivitySlider.value);
        networkClient.sensitivity = sensitivitySlider.value;
    }

   public void OnClickSalir()
    {
        Application.Quit();
    }
}
