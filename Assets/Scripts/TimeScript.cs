using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeScript : MonoBehaviour
{
    public float totalSeconds;
    public GameObject scoreDisplay;
    public TextMeshProUGUI time;
    private float secondsSinceStart;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (totalSeconds >= 0)
        {
            secondsSinceStart += Time.fixedDeltaTime;

            float timeRemaining = totalSeconds - secondsSinceStart;

            if(timeRemaining < 60)
                time.text = "Time: " + Mathf.FloorToInt(timeRemaining).ToString("00") + " : " + (Mathf.FloorToInt(timeRemaining * 100) % 100).ToString("00");
            else
                time.text = "Time: " + Mathf.FloorToInt(timeRemaining / 60).ToString("00") + " : " + (Mathf.FloorToInt(timeRemaining) % 60).ToString("00");
            if (secondsSinceStart > totalSeconds)
            {
                scoreDisplay.SetActive(true);
                totalSeconds = -1.0f;
                time.text = "Time: -- : --";
            }
        }
    }
}
