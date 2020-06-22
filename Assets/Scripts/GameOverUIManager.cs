using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIManager : MonoBehaviour
{
     // Where the score value will be shown
    public Text score;
    
    // Where the high score value will be shown
    public Text highScore;
 
    // Run on the first frame
    void Start()
    {
        // Show the score and high score
        score.text = GameManager.instance.score.ToString();
        highScore.text = GameManager.instance.highScore.ToString();
    }
 
    public void RestartGame()
    {
        // Reset the game
        GameManager.instance.Reset();
    }

}
