using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class WireMinigameManager : MonoBehaviour
{
    [SerializeField] Sprite[] wires;

    [SerializeField] string currentButton,lastClickedButton;
    public GameObject redWire, blueWire, yellowWire, wireMinigame;
    [SerializeField] bool redConnected, blueConnected, yellowConnected;

    [SerializeField] List<int> positions = new List<int>();
    [SerializeField] List<int> positions2 = new List<int>();

    // TEMPORARY

    public void BeginMinigame()
    {
        // Wires inactive, buttons active
        ResetPositions();

        redConnected = false;
        blueConnected = false;
        yellowConnected = false;

        wireMinigame.SetActive(true);

        redWire.SetActive(false);
        blueWire.SetActive(false);
        yellowWire.SetActive(false);

        GameObject.Find("RedButton1").GetComponent<Button>().interactable = true;
        GameObject.Find("RedButton2").GetComponent<Button>().interactable = true;
        GameObject.Find("BlueButton1").GetComponent<Button>().interactable = true;
        GameObject.Find("BlueButton2").GetComponent<Button>().interactable = true;
        GameObject.Find("YellowButton1").GetComponent<Button>().interactable = true;
        GameObject.Find("YellowButton2").GetComponent<Button>().interactable = true;

        RandomizeButtonsLeft();
       

        redWire.GetComponent<Image>().sprite = CalculateConnection(GameObject.Find("RedButton1"), GameObject.Find("RedButton2"));
        blueWire.GetComponent<Image>().sprite = CalculateConnection(GameObject.Find("BlueButton1"), GameObject.Find("BlueButton2"));
        yellowWire.GetComponent<Image>().sprite = CalculateConnection(GameObject.Find("YellowButton1"), GameObject.Find("YellowButton2"));

       
    }

    private void RandomizeButtonsLeft()
    {
        List<int> positionsBackup = positions;
        List<int> positionsBackup2 = positions2;

        Debug.Log(positionsBackup[0] + " " + positionsBackup[1] + " " + positionsBackup[2]);

        int yPosition = UnityEngine.Random.Range(0, positionsBackup.Count);

        GameObject redButton = GameObject.Find("RedButton1");
        redButton.transform.position = new Vector3(redButton.transform.position.x, positionsBackup[yPosition], redButton.transform.position.z);

        positionsBackup.RemoveAt(yPosition);

        Debug.Log("Keep going!");

        yPosition = UnityEngine.Random.Range(0, positionsBackup.Count);

        GameObject blueButton = GameObject.Find("BlueButton1");
        blueButton.transform.position = new Vector3(blueButton.transform.position.x, positionsBackup[yPosition], blueButton.transform.position.z);

        positionsBackup.RemoveAt(yPosition);

        Debug.Log("Keep going!");

        yPosition = UnityEngine.Random.Range(0, positionsBackup.Count);

        GameObject yellowButton = GameObject.Find("YellowButton1");
        yellowButton.transform.position = new Vector3(yellowButton.transform.position.x, positionsBackup[yPosition], yellowButton.transform.position.z);

        Debug.Log("Finished with First");

        //RESET positionsBackup TO INTIAL positions
      

        Debug.Log(positions[0]);

        Debug.Log( "sec" + positionsBackup2[0] + " " + positionsBackup2[1] + " " + positionsBackup2[2]);

         int yPosition2 = UnityEngine.Random.Range(0, positionsBackup2.Count);

        GameObject redButton2 = GameObject.Find("RedButton2");
        redButton2.transform.position = new Vector3(redButton2.transform.position.x, positionsBackup2[yPosition2], redButton2.transform.position.z);

        positionsBackup2.RemoveAt(yPosition2);

        Debug.Log("Keep going!");

        yPosition2 = UnityEngine.Random.Range(0, positionsBackup2.Count);

        GameObject blueButton2 = GameObject.Find("BlueButton2");
        blueButton2.transform.position = new Vector3(blueButton2.transform.position.x, positionsBackup2[yPosition2], blueButton2.transform.position.z);

        positionsBackup2.RemoveAt(yPosition2);

        Debug.Log("Keep going!");

        yPosition2 = UnityEngine.Random.Range(0, positionsBackup2.Count);

        GameObject yellowButton2 = GameObject.Find("YellowButton2");
        yellowButton2.transform.position = new Vector3(yellowButton2.transform.position.x, positionsBackup2[yPosition2], yellowButton2.transform.position.z);

        Debug.Log("Finished with First");



    }

    

    public void ClickButton(string button)
    {
        lastClickedButton = currentButton;
        currentButton = button;

        if ((currentButton.Contains("1") && lastClickedButton.Contains("2")) || 
            (currentButton.Contains("2") && lastClickedButton.Contains("1")))
        {
            if (currentButton.Contains("Red") && lastClickedButton.Contains("Red"))
            {
                Debug.Log("Reds");
                redWire.SetActive(true);
                redConnected = true;
                DisableButtons();
            }
            if (currentButton.Contains("Blue") && lastClickedButton.Contains("Blue"))
            {
                Debug.Log("Blue");
                blueWire.SetActive(true);
                blueConnected = true;
                DisableButtons();
            }
            if (currentButton.Contains("Yellow") && lastClickedButton.Contains("Yellow"))
            {
                Debug.Log("Yellow");
                yellowWire.SetActive(true);
                yellowConnected = true;
                DisableButtons();
            }
        }
    }

    void DisableButtons()
    {
        GameObject.Find(lastClickedButton).GetComponent<Button>().interactable = false;
        GameObject.Find(currentButton).GetComponent<Button>().interactable = false;

        if (redConnected && yellowConnected && blueConnected)
        {
            Debug.Log("All Connected!");
            redConnected = false;
            blueConnected = false;
            yellowConnected = false;
            Invoke("CloseMinigame", 1f);
        }
    }

    Sprite CalculateConnection(GameObject button1, GameObject button2)
    {
        Debug.Log(button1.transform.position.y + ", " + button2.transform.position.y);
        if(button1.transform.position.y == 840)
        {
            if (button2.transform.position.y == 840) { return wires[0]; }
            else if (button2.transform.position.y == 540) { return wires[1]; }
            else if (button2.transform.position.y == 240) { return wires[2]; }
        }
        else if (button1.transform.position.y == 540)
        {
            if (button2.transform.position.y == 840) { return wires[3]; }
            else if (button2.transform.position.y == 540) { return wires[4]; }
            else if (button2.transform.position.y == 240) { return wires[5]; }
        }
        else if (button1.transform.position.y == 240)
        {
            if (button2.transform.position.y == 840) { return wires[6]; }
            else if (button2.transform.position.y == 540) { return wires[7]; }
            else if (button2.transform.position.y == 240) { return wires[8]; }
        }
        Debug.Log("Error!");
        return wires[0];
    }

    public void CloseMinigame()
    {
        wireMinigame.SetActive(false);
    }

    public void ResetPositions()
    {
        positions.Clear();
        positions2.Clear();

        lastClickedButton = null;
        currentButton = null;
        
        

        for (int i = 0; i < 3; i++)
        {
            positions.Add(0);
            positions2.Add(0);
        }

        positions[0] = 540;
        positions[1] = 840;
        positions[2] = 240;
        positions2[0] = 540;
        positions2[1] = 840;
        positions2[2] = 240;

        
    }


}
