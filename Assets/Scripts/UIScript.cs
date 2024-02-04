using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public int ammoInMag;
    public int ammoCount;

    public int health;

    public Slider healthPoints;
    public TextMeshProUGUI ammoCounter;

    void Update()
    {
        healthPoints.value = health;
        ammoCounter.text = ammoInMag.ToString() + "|" + ammoCount.ToString();
        
    }
}
