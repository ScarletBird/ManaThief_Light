using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public Text healthLabel;
    public Text manaLabel;

    public GameObject pauseMenu;

    public GameObject optionsMenu;

    public GameObject player;

    // Use this for initialization

    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    void Start()
    {
        pauseMenu.SetActive(false);

        Refresh();
    }
 
    // Show player stats in the HUD
    public void Refresh()
    {
        healthLabel.text = "Life: " + player.GetComponent<PlayerPlatformerController>().health;
        manaLabel.text = "Mana: " + GameManager.instance.score;
    }

    public void Pause()
    {
        // Pause the game
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
        GameManager.instance.isPaused = true;
    }

    public void Options()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        GameManager.instance.isPaused = true;
    }

    public void UnPause()
    {
        // Pause the game
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        GameManager.instance.isPaused = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
