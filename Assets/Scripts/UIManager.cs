using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    string mainMenuScene = "MainMenu";
    string Level1 = "LevelOne_F";
    string levelTwo = "LevelTwo_F";
    string levelThree = "LevelThree_F";
    string levelBoss = "LevelTwoDesign";
    public GameObject main;
    public GameObject credits;
    public GameObject help;
    string winScene = "WIN";
    string levelThreeActual = "LevelThree";

    public AudioSource buttonSounds;
    public AudioClip click1, click2;
    
    public bool Paused = false;
    bool isDead = false;
    public GameObject Player;

    public GameObject gameOverMenu;
    public GameObject pauseMenu;
    

    private bool IsInTheGame;
    public int DeathCheck = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        isDead = false;

        Time.timeScale = 1.0f; //sets time scale

        if ((SceneManager.GetActiveScene().name == levelTwo) || (SceneManager.GetActiveScene().name == levelBoss)
            || (SceneManager.GetActiveScene().name == levelTwo) || (SceneManager.GetActiveScene().name == levelThree)|| (SceneManager.GetActiveScene().name == levelThreeActual))
        {
            Debug.Log("Open Pause Close");
            IsInTheGame = true;
            UnpauseGame();
            Paused = false;

            //Player = GameObject.Find("Player");
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);


        }

        else
        {
            IsInTheGame = false;
        }

        if (SceneManager.GetActiveScene().name == "WIN")
        {

        }

        

        
    }

    // Update is called once per frame
    void Update()
    {
        
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (!Paused)
                {

                    PauseGame();
 
                }
                else
                {
                    UnpauseGame();

                }
            }
    }

    //--------------Functions----------------


    public void StartGame()
    {
        SceneManager.LoadScene(Level1);
        buttonSounds.PlayOneShot(click1);
    }

    public void GameOver()
    {
        isDead = true;
        GameOverMenu();

    }

    public void GetHelp()
    {
        Debug.Log("Help Scene");
        help.SetActive(true);
        main.SetActive(false);
        buttonSounds.PlayOneShot(click1);
    }

    public void GetCredits()
    {
        Debug.Log("Credit Scene");
        credits.SetActive(true);
        main.SetActive(false);
        buttonSounds.PlayOneShot(click1);
    }
    public void BackToMenu()
    {
        Debug.Log("Menu");
        credits.SetActive(false);
        help.SetActive(false);
        main.SetActive(true);
        buttonSounds.PlayOneShot(click2);
    }
    public void QuitToMenu()
    {
        Debug.Log("Main Scene");
        SceneManager.LoadScene(mainMenuScene);
    }

    public void SecondLevel()
    {
        Debug.Log("Level Two Scene");
        SceneManager.LoadScene(levelTwo);
    }

    public void ThirdLevel()
    {
        Debug.Log("Level 3 Scene");
        SceneManager.LoadScene(levelThree);
    }

    public void BossLevel()
    {
        Debug.Log("Boss Scene");
        SceneManager.LoadScene(levelBoss);
    }


    public void QuitButton()
    {
        Debug.Log("Quit");
        Application.Quit();
        buttonSounds.PlayOneShot(click1);
    }

    public void GetWinScene()
    {
        Debug.Log("Loading Win Scene");
        SceneManager.LoadScene(winScene);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void GameOverMenu() //GameOverScene
    {
        Time.timeScale = 0f;

        gameOverMenu.SetActive(true);

        isDead = true;

    }

    public void PauseGame()
    {
        Time.timeScale = 0f;

        pauseMenu.SetActive(true);

        Paused = true;
    }
    public void UnpauseGame() //unpauses the game
    {
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        Paused = false;
    }

    public void DebugSkipLevel()
    {
        if(SceneManager.GetActiveScene().name == "LevelOneActual")
        {
            SceneManager.LoadScene("IntroLevelTwo");
        }

        else if (SceneManager.GetActiveScene().name == "LvlTwoProto")
        {
            SceneManager.LoadScene("IntroLevelThree");
        }

        else if (SceneManager.GetActiveScene().name == "LevelThree")
        {
            SceneManager.LoadScene("OutroLevelThree");
        }

        Time.timeScale = 1f;
    }

}
