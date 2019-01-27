using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScipt : MonoBehaviour
{
    public PlayerMovement player1;
    public PlayerMovement player2;

    public float speed = 1.0f;

    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }


    public int Winner() {
        float player1Score = player1.GetPlayerScoreSurrounded();
        float player2Score = player2.GetPlayerScoreSurrounded();
        if (player1Score == player2Score)
            return 0;
        else if (player1Score > player2Score)
            return -1;
        else
            return 1;
    }

    // Update is called once per frame
    void Update()
    {
        float percentage = slider.value;
        float player1Score = player1.GetPlayerScoreSurrounded();
        float player2Score = player2.GetPlayerScoreSurrounded();
        float targetPercentage;

        if (player1Score + player2Score == 0)
        {
            targetPercentage = 0.5f;
        } else {
            targetPercentage = player1Score / (player1Score + player2Score);
        }

        float step = speed * Time.deltaTime;
        if ( Mathf.Abs(targetPercentage - percentage ) < step) {
            slider.value = targetPercentage;

            return;
        }

        if (targetPercentage > percentage)
        {
            slider.value = percentage + step;
        }
        else
        {
            slider.value = percentage - step;
        }
    }
}
