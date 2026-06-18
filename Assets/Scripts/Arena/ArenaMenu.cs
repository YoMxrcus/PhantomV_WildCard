using UnityEngine;

public class ArenaMenu : MonoBehaviour
{
    public GameObject arenaPAN;
    [SerializeField] private bool pauseOnStart = false;

    void Start()
    {
        if (pauseOnStart)
        {
            Time.timeScale = 0;
        }
    }

    public void ArenaStart()
    {
        arenaPAN.SetActive(false);
        Time.timeScale = 1;
    }
}
