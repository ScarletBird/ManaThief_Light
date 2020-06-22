using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
        GameManager.instance.score = 0;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}