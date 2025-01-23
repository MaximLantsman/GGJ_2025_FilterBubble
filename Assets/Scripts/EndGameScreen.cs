using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScreen : MonoBehaviour
{
    [SerializeField,Anywhere] private TextMeshProUGUI loseResonText;
    
    private string loseReasonRed = "NO MORE LEFT NEWS";
    private string loseReasonBlue = "NO MORE RIGHT NEWS";
    private string loseReasonGreen = "NO FUN NEWS LEFT :(";
    private string loseReasonYellow = "FAKE FAKE FAKE - ALL FAKE";

    private string currentLoseReson;
    public void GameEnded(newsType reason)
    {
        switch (reason)
        {
            case newsType.Blue:
                currentLoseReson = loseReasonBlue;
                break;
                
            case newsType.Red:
                currentLoseReson = loseReasonRed;
                break;
               
            case newsType.Green:
                currentLoseReson = loseReasonGreen;
                break;
                
            case newsType.Yellow:
                currentLoseReson = loseReasonYellow;
                break;
            
            default:
                
                break;
        }
 
        loseResonText.text = currentLoseReson;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
