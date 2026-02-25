using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    string mainMenuScene = "MainMenu";
    string levelOne = "IntroLevelOneAnimation";
    string levelTwo = "LvlTwoProto";
    string levelThree = "LevelOneActual";
    string levelBoss = "LEVEL2TESTSCENE";
    string creditsScene = "Credits";
    string helpScene = "Controls";
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

        if ((SceneManager.GetActiveScene().name == levelOne) || (SceneManager.GetActiveScene().name == levelBoss)
            || (SceneManager.GetActiveScene().name == levelTwo) || (SceneManager.GetActiveScene().name == levelThree)|| (SceneManager.GetActiveScene().name == levelThreeActual))
        {
            Debug.Log("Open Pause Close");
            IsInTheGame = true;
            UnpauseGame();
            Paused = false;

            //Player = GameObject.Find("Player");
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        else
        {
            IsInTheGame = false;
        }

        if (SceneManager.GetActiveScene().name == "WIN")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        

        
    }

    // Update is called once per frame
    void Update()
    {
        

        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isDead)
            {
                if (!Paused)
                {

                    PauseGame();
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    UnpauseGame();
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }

    //--------------Functions----------------


    public void StartGame()
    {
        SceneManager.LoadScene(levelOne);
    }

    public void GameOver()
    {
        isDead = true;
        GameOverMenu();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GetHelp()
    {
        Debug.Log("Help Scene");
        SceneManager.LoadScene(helpScene);
    }

    public void GetCredits()
    {
        Debug.Log("Credit Scene");
        SceneManager.LoadScene(creditsScene);
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
