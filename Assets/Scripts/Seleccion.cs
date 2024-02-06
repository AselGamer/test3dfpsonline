using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seleccion : MonoBehaviour
{
    public Arma[] armasSeleccionadas;

    // Start is called before the first frame update
    void Start()
    {
        armasSeleccionadas = new Arma[3];
    }
}
