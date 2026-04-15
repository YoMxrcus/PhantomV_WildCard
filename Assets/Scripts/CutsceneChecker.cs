
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CutsceneChecker : MonoBehaviour
{
    string mainMenuScene = "MainMenu";
    string LevelOneIntro = "ComicOneScene";
    string LevelOne = "LevelOneSKILLS";
    string LevelOneOutro = "OutroLevelOneAnimation";
    string LevelTwoIntro = "ComicTwoScene";
    string LevelTwo = "LevelTwoSkills";
    string LevelTwoOutro = "OutroLevelTwo";
    string LevelThreeIntro = "ComicThreeScene";
    string LevelThree = "LevelThreeSKILLS";
    string LevelThreeOutro = "OutroLevelThree";
    string winScene = "WIN";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == LevelOneIntro)
        {
            Invoke("LoadLevelTwo",21.25f);
        }

        if (SceneManager.GetActiveScene().name == LevelTwoIntro)
        {
            Invoke("LoadLevelThree", 21.25f);
        }


        if (SceneManager.GetActiveScene().name == LevelThreeIntro)
        {
            Invoke("LoadWinScene", 21.25f);
        }

   
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == LevelOneIntro)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                LoadLevelTwo();
            }
        }

        if (SceneManager.GetActiveScene().name == LevelTwoIntro)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                LoadLeveLThree();
            }
        }


        if (SceneManager.GetActiveScene().name == LevelThreeIntro)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                LoadWinScene() ;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "NextLevel":
                if (SceneManager.GetActiveScene().name == LevelOne)
                {
                    SceneManager.LoadScene(LevelOneIntro);
                }
                else if (SceneManager.GetActiveScene().name == LevelTwo)
                {
                    LoadLevelTwoIntro();
                }
                else if (SceneManager.GetActiveScene().name == LevelThree)
                {
                    LoadLevelThreeIntro();
                }
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
