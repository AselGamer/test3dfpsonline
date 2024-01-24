using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    public int ammoInMag;
    public int ammoCount;

    public int health;

    public TextMeshProUGUI healthPoints;
    public TextMeshProUGUI ammoCounter;

    void Update()
    {
        healthPoints.text = "Vida: " + health.ToString();
        ammoCounter.text = ammoInMag.ToString() + "/" + ammoCount.ToString();
        
    }
}
