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
    public GameObject pointsGained;

    private bool pointsActive = false;

    void Update()
    {
        healthPoints.value = health;
        ammoCounter.text = ammoInMag.ToString() + "|" + ammoCount.ToString();
        pointsActive = pointsGained.activeSelf;
    }

    public IEnumerator ShowPoints()
    {
        if (pointsActive)
        {
            yield break;
        }
        pointsGained.SetActive(true);
        yield return new WaitForSeconds(2f);
        pointsGained.SetActive(false);
    }
}
