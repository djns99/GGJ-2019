using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    public TimeScript timeScript;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
    }

    public void OnEndlessClicked()
    {
        timeScript.totalSeconds = -1.0f;
        Time.timeScale = 1f;
    }

    public void OnTimedClicked()
    {
        timeScript.totalSeconds = 3.0f;
        Time.timeScale = 1f;
    }
}
