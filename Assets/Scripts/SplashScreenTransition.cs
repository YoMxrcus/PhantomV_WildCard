using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenTransition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("TransitionScene", 4.25f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TransitionScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
