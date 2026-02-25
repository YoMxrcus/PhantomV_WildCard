using UnityEngine;

public class MusicDeleter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameObject.Find("MenuSoundEffectBG"))
        {
            Destroy(GameObject.Find("MenuSoundEffectBG"));
        }
        else
        {
            Debug.Log("Couldnt Find");
        }
    }


}
