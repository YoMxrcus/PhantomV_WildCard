using UnityEngine;

public class MusicSingleton : MonoBehaviour
{
    public static MusicSingleton Instance { get; private set; }

    private void Awake()
    {
        //singleton 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
