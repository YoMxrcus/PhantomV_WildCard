using UnityEngine;

public class GeneratorScript : MonoBehaviour
{

    public GameObject lightOne, lightTwo, lightThree;
    public int minigamesComplete = 0;
    public Material completeMaterial;
    public GameObject Player;

    public GameObject doorOne;
    public GameObject doorTwo;
    public GameObject doorThree;

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
                lightOne.GetComponent<Renderer>().material = completeMaterial;
                return;

            case 2:
                lightTwo.GetComponent<Renderer>().material = completeMaterial;
                return;

            case 3:
                lightThree.GetComponent<Renderer>().material = completeMaterial;
                switch (gameObject.name)
                {
                    case "Generator01":
                        doorOne = GameObject.Find("Door_01");
                        doorTwo = GameObject.Find("Door_02");

                        doorOne.SetActive(false);
                        doorTwo.SetActive(false);
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
