using UnityEngine;

public class HeatMeterSOUND : MonoBehaviour
{

    public AudioSource HeatSounds;
    public GameObject Player;
    public AudioClip[] audioClips;
    public bool Audio1 = false;
    public bool Audio2 = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("ThirdPersonTEST");
        HeatSounds = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.GetComponent<ThirdPersonController>().HeaterUp > 60 && Player.GetComponent<ThirdPersonController>().HeaterUp < 90 && !Audio1)
        {
            Audio1 = true;
            Audio2 = false;
            HeatSounds.clip = audioClips[0];
            HeatSounds.loop = true;
            HeatSounds.Play();
            
            
        }

        else if (Player.GetComponent<ThirdPersonController>().HeaterUp > 90 && Player.GetComponent<ThirdPersonController>().HeaterUp < 120 && !Audio2)
        {
            Audio2 = true;
            Audio1 = false;
            HeatSounds.clip = audioClips[1];
            HeatSounds.loop = true;
            HeatSounds.Play();
        }

        if (Player.GetComponent<ThirdPersonController>().HeaterUp < 60 || Player.GetComponent<ThirdPersonController>().HeaterUp == 120)
        {
            HeatSounds.Stop();
        }
    }
}
