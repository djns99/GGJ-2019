using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayScore : MonoBehaviour
{
    public ScoreScipt scoreScript;
    public TextMeshProUGUI text;

    // Start is called before the first frame update
    public void OnEnable()
    {
        int winner = scoreScript.Winner();
        
        switch (winner) {
            case -1:
                text.text = "Game Over.\n\nRed Player Wins!";
                break;
            case 0:
                text.text = "Game Over.\n\nGame Drawn!";
                break;
            case 1:
                text.text = "Game Over.\n\nBlue Player Wins!";
                break;

        }
    }

    public void OnRestartClicked()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
