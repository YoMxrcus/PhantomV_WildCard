using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    string mainMenuScene = "MainMenu";
    string Level1 = "LevelOneIntroAnimation";
    string levelTwo = "LevelOneDesign";
    string levelThree = "LevelOneActual";
    string levelBoss = "LevelTwoDesign";
    string Credits = "Credits";
    string Help = "Help";
    string winScene = "WIN";
    string levelThreeActual = "LevelThree";

    
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
    }

    public void GameOver()
    {
        isDead = true;
        GameOverMenu();

    }

    public void GetHelp()
    {
        Debug.Log("Help Scene");
        SceneManager.LoadScene(Help);
    }

    public void GetCredits()
    {
        Debug.Log("Credit Scene");
        SceneManager.LoadScene(Credits);
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

    public void BossLevel()
    {
        Debug.Log("Boss Scene");
        SceneManager.LoadScene(levelBoss);
    }


    public void QuitButton()
    {
        Debug.Log("Quit");
        Application.Quit();

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
