using UnityEngine;
using System.Collections;
//using UnityEditor.AdaptivePerformance.Editor; Required for Coroutines

public class MenuSounds : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip windAmbient;

    public float minWaitTime = 2f;
    public float maxWaitTime = 10f;
    public int sound;
    void Start()
    {
        // Start loop 
        StartCoroutine(PlaySoundsAtRandom());
    }

    IEnumerator PlaySoundsAtRandom()
    {
        while (true)
        {
           
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            sound = Random.Range(1, 3);
            switch (sound)
            {
                case 1:
                    audioSource.PlayOneShot(windAmbient);
                    Debug.Log("Playing sound!");
                        break;
            }
        }
    }
}