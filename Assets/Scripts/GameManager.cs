using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Static instance of the Game Manager,
    // can be access from anywhere
    public static GameManager instance = null;
 
    // Player score
    public int score = 0;

    // High score
    public int highScore = 0;

    // Level, starting in level 1
    public int currentLevel = 1;
 
    // Highest level available in the game
    public int highestLevel = 2;

    // Is the game paused or not
    public bool isPaused;

    // Called when the object is initialized
    void Awake()
    {
        // if it doesn't exist
        if(instance == null)
        {
            // Set the instance to the current object (this)
            instance = this;
        }
 
        // There can only be a single instance of the game manager
        else if(instance != this)
        {
            // Destroy the current object, so there is just one manager
            Destroy(gameObject);
        }
 
        // Don't destroy this object when loading scenes
        DontDestroyOnLoad(gameObject);

        // Makes sure the game is unpaused
        isPaused = false;
    }
 
    // Increase score
    public void IncreaseScore(int amount)
    {
        // Increase the score by the given amount
        score += amount;
 
        // Show the new score in the console
        print("New Score: " + score.ToString());

        if (score > highScore)
        {
            highScore = score;
            print("New high score: " + highScore);
        }
    }

    // Restart game. Refresh previous score and send back to level 1
    public void Reset()
    {
        // Reset the score
        score = 0;
 
        // Set the current level to 1
        currentLevel = 1;
 
        // Load corresponding scene (level 1 or "splash screen" scene)
        SceneManager.LoadScene("Level" + currentLevel);
    }

     // Go to the next level
    public void IncreaseLevel()
    {
        if (currentLevel < highestLevel)
        {
            currentLevel++;            
        }
        else
        {
            currentLevel = 1;
        }
        SceneManager.LoadScene("Level" + currentLevel);
    }

}
