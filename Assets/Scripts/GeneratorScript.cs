using UnityEngine;

public class GeneratorScript : MonoBehaviour
{

    public GameObject lightOne, lightTwo, lightThree;
    public int minigamesComplete = 0;
    public Material completeMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
}
