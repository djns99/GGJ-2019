using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    public TimeScript timeScript;
    public PlayerMovement player1;
    public PlayerMovement player2;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
        player1.gameStopped = true;
        player2.gameStopped = true;
    }

    public void OnEndlessClicked()
    {
        player1.gameStopped = false;
        player2.gameStopped = false;
        timeScript.totalSeconds = -1.0f;
        Time.timeScale = 1f;
    }

    public void OnTimedClicked()
    {
        player1.gameStopped = false;
        player2.gameStopped = false;
        timeScript.totalSeconds = 300.0f;
        Time.timeScale = 1f;
    }
}
