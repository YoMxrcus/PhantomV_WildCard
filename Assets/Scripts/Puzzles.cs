using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Puzzles : MonoBehaviour
{
    [SerializeField] Sprite[] wires;

    [SerializeField] string currentButton,lastClickedButton;
    [SerializeField] GameObject redWire, blueWire, yellowWire, wireMinigame;
    [SerializeField] bool redConnected, blueConnected, yellowConnected;

    [SerializeField] List<int> positions = new List<int>();
    

    // TEMPORARY
    private void Start()
    {
        BeginMinigame();
    }

    public void BeginMinigame()
    {
        // Wires inactive, buttons active
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
        RandomizeButtonsRight();

        redWire.GetComponent<Image>().sprite = CalculateConnection(GameObject.Find("RedButton1"), GameObject.Find("RedButton2"));
        blueWire.GetComponent<Image>().sprite = CalculateConnection(GameObject.Find("BlueButton1"), GameObject.Find("BlueButton2"));
        yellowWire.GetComponent<Image>().sprite = CalculateConnection(GameObject.Find("YellowButton1"), GameObject.Find("YellowButton2"));

        redConnected = false;
        blueConnected = false;
        yellowConnected = false;
    }

    private void RandomizeButtonsLeft()
    {
        List<int> positionsBackup = positions;

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

        yPosition = UnityEngine.Random.Range(0, positionsBackup.Count);

        GameObject yellowButton = GameObject.Find("YellowButton1");
        yellowButton.transform.position = new Vector3(yellowButton.transform.position.x, positionsBackup[yPosition], yellowButton.transform.position.z);
    }

    private void RandomizeButtonsRight()
    {
        List<int> positionsBackup = positions;

        int yPosition = UnityEngine.Random.Range(0, positionsBackup.Count);

        GameObject redButton = GameObject.Find("RedButton2");
        redButton.transform.position = new Vector3(redButton.transform.position.x, positionsBackup[yPosition], redButton.transform.position.z);

        positionsBackup.RemoveAt(yPosition);

        Debug.Log("Keep going!");

        yPosition = UnityEngine.Random.Range(0, positionsBackup.Count);

        GameObject blueButton = GameObject.Find("BlueButton2");
        blueButton.transform.position = new Vector3(blueButton.transform.position.x, positionsBackup[yPosition], blueButton.transform.position.z);

        positionsBackup.RemoveAt(yPosition);

        yPosition = UnityEngine.Random.Range(0, positionsBackup.Count);

        GameObject yellowButton = GameObject.Find("YellowButton2");
        yellowButton.transform.position = new Vector3(yellowButton.transform.position.x, positionsBackup[yPosition], yellowButton.transform.position.z);
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

    void CloseMinigame()
    {
        wireMinigame.SetActive(false);
    }
}
