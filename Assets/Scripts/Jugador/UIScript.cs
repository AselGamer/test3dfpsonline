using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public int ammoInMag;
    public int ammoCount;

    public int health;

    public int kills;

    public Slider healthPoints;
    public TextMeshProUGUI ammoCounter;
    public TextMeshProUGUI killCount;
    public GameObject pointsGained;

    private bool pointsActive = false;

    void Update()
    {
        healthPoints.value = health;
        ammoCounter.text = ammoInMag.ToString() + "|" + ammoCount.ToString();
        killCount.text = kills.ToString();
        pointsActive = pointsGained.activeSelf;

        if (kills >= 10)
        {
            SceneManager.LoadScene("Final");
            SceneManager.UnloadSceneAsync("Client");
        }
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
