
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneChecker : MonoBehaviour
{
    string mainMenuScene = "MainMenu";
    string LevelOneIntro = "LevelOneIntroAnimation";
    string LevelOne = "LevelOneDesignAlejandroTesting";
    string LevelOneOutro = "OutroLevelOneAnimation";
    string LevelTwoIntro = "LevelTwoIntroAnimations";
    string LevelTwo = "LevelTwoDesign";
    string LevelTwoOutro = "OutroLevelTwo";
    string LevelThreeIntro = "IntroLevelThree";
    string LevelThree = "LeveLThree";
    string LevelThreeOutro = "OutroLevelThree";
    string winScene = "WIN";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == LevelOneIntro)
        {
            Invoke("LoadLevelOne",10f);
        }

        if (SceneManager.GetActiveScene().name == LevelOneOutro)
        {
            Invoke("LoadLevelTwoIntro", 5f);
        }

        if (SceneManager.GetActiveScene().name == LevelTwoIntro)
        {
            Invoke("LoadLevelTwo", 8f);
        }

        if (SceneManager.GetActiveScene().name == LevelTwoOutro)
        {
            Invoke("LoadLevelThreeIntro", 7f);
        }

        if (SceneManager.GetActiveScene().name == LevelThreeIntro)
        {
            Invoke("LoadLeveLThree", 5f);
        }

        if (SceneManager.GetActiveScene().name == LevelThreeOutro)
        {
            //
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "NextLevel":
                if (SceneManager.GetActiveScene().name == LevelOne)
                {
                    LoadLevelTwoIntro();
                }
                else if (SceneManager.GetActiveScene().name == LevelThree)
                {
                    LoadWinScene();
                }
                    break;

            case "DeathPlane":
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                
                break;

        }
    }

    void LoadLevelOne()
    {
        SceneManager.LoadScene(LevelOne);
    }

    void LoadLevelTwoIntro()
    {
        SceneManager.LoadScene(LevelTwoIntro);
    }

    void LoadLevelOneEnd()
    {
        SceneManager.LoadScene(LevelOneOutro);
    }

    void LoadLevelTwo()
    {
        SceneManager.LoadScene(LevelTwo);
    }

    void LoadLevelThreeIntro()
    {
        SceneManager.LoadScene(LevelThreeIntro);
    }

    void LoadLeveLThree()
    {
        SceneManager.LoadScene(LevelThree);
    }

    void LoadLeveLThreeoOutro()
    {
        SceneManager.LoadScene(LevelThreeOutro);
    }

    void LoadWinScene()
    {
        SceneManager.LoadScene(winScene);
    }


}
