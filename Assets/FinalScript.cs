using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalScript : MonoBehaviour
{

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Cerrar()
    {
        Application.Quit();
    }
}
