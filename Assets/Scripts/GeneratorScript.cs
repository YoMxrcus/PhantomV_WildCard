using UnityEngine;
using UnityEngine.SceneManagement;
public class GeneratorScript : MonoBehaviour
{

    public GameObject lightOne, lightTwo, lightThree;
    public int minigamesComplete = 0;
    public Material completeMaterial;
    public GameObject Player;

    public GameObject doorOne;
    public GameObject doorTwo;
    public GameObject doorThree;

    public AudioSource generatorSounds;
    public AudioClip generatorCorrect, openDoor; //where do I put this?


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("MainPlayer");
    }

    // Update is called once per frame
   

    public void UpdateLights()
    {
        switch (minigamesComplete) 
        
        {
            case 1:
                generatorSounds.PlayOneShot(generatorCorrect);
                lightOne.GetComponent<Renderer>().material = completeMaterial;
                return;

            case 2:
                generatorSounds.PlayOneShot(generatorCorrect);
                lightTwo.GetComponent<Renderer>().material = completeMaterial;
                return;

            case 3:
                generatorSounds.PlayOneShot(generatorCorrect);
                lightThree.GetComponent<Renderer>().material = completeMaterial;
                switch (gameObject.name)
                {
                    case "Generator03":
                        doorOne = GameObject.Find("Door_01");
                        doorTwo = GameObject.Find("Door_02");

                        doorOne.SetActive(false);
                        doorTwo.SetActive(false);

                        SceneManager.LoadScene("LevelTwo_F");
                        break;




                    case "Generator01_02":
                        doorOne = GameObject.Find("Door_01");
                        doorTwo = GameObject.Find("Door_02");

                        doorOne.SetActive(false);
                        doorTwo.SetActive(false);
                        break;
                    case "Generator02_02":
                        doorOne = GameObject.Find("Door_03");
                        doorOne.SetActive(false);
                        break;
                    case "Generator03_02":
                        doorOne = GameObject.Find("Door_04");
                        doorOne.SetActive(false);

                        SceneManager.LoadScene("LevelThree_F");
                        break;


                    case "Generator01_03":
                        doorOne = GameObject.Find("Door_01");
                        doorOne.SetActive(false);
                        break;
                    case "Generator02_03":
                        doorOne = GameObject.Find("Door_02");
                        doorOne.SetActive(false);
                        break;
                    case "Generator03_03":
                        doorOne = GameObject.Find("Door_03");
                        doorOne.SetActive(false);
                        break;
                    case "Generator04_03":
                        doorOne = GameObject.Find("Door_04");
                        doorOne.SetActive(false);
                        break;
                    case "Generator05_03":
                        SceneManager.LoadScene("WIN");
                        doorOne = GameObject.Find("Door_05");
                        doorOne.SetActive(false);
                        break;
                    case "Generator06_03":
                        doorOne = GameObject.Find("Door_06");
                        doorOne.SetActive(false);

                        
                        break;

                    default:
                        break;
                }
                return;
        }
    }

    void UnlockDoors()
    {
        //Checks tag of generator and opens door based on generator number
    }
}
