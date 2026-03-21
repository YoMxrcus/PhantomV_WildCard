using UnityEngine;

public class GeneratorScript : MonoBehaviour
{

    public GameObject lightOne, lightTwo, lightThree;
    public int minigamesComplete = 0;
    public Material completeMaterial;
    public GameObject Player;
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
                
                return;
        }
    }

    void UnlockDoors()
    {
        //Checks tag of generator and opens door based on generator number
    }
}
