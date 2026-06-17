using UnityEngine;

public class ArenaMenu : MonoBehaviour
{
    public GameObject arenaPAN;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0;
    }

    public void ArenaStart()
    {
        arenaPAN.SetActive(false);
        Time.timeScale = 1;
    }
}
