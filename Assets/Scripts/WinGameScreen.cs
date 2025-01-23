using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGameScreen : MonoBehaviour
{
    //[SerializeField,Anywhere] private TextMeshProUGUI loseResonText;
    
   

    private string currentLoseReson;
    public void GameEnded()
    {
        //loseResonText.text = currentLoseReson;
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
