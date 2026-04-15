using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PasswordMinigameManager : MonoBehaviour
{

    [SerializeField] string[] comboLetters = new string[4];
    public char PlayerInput;
    public string currentPassword;
    public TMP_Text passwordText;
    public GameObject correctOne, correctTwo, correctThree, correctFour, correctFive;
    [SerializeField] int correctCounter = 0;
    public GameObject PasswordMinigame;
    public GameObject Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
 

    // Update is called once per frame
    void Update()
    {
        if (Player.GetComponent<ThirdPersonController>().InPuzzle == true)
        {

            if (Keyboard.current.qKey.wasPressedThisFrame)
            {

                // Sets PlayerInput to the character "q"
                PlayerInput = 'q';
                CheckPassword();
            }

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {

                // Sets PlayerInput to the character "q"
                PlayerInput = 'e';
                CheckPassword();
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {

                // Sets PlayerInput to the character "q"
                PlayerInput = 'r';
                CheckPassword();
            }

            if (Keyboard.current.fKey.wasPressedThisFrame)
            {

                // Sets PlayerInput to the character "q"
                PlayerInput = 'f';
                CheckPassword();
            }
        }
    }

    public void BeginMinigame()
    {
        PasswordMinigame.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            currentPassword += comboLetters[Random.Range(0, comboLetters.Length)];
            
        }

        passwordText.text = currentPassword.ToUpper();
    }

    void CheckPassword()
    {


        //Creates a variable that holds the first character of the combination (Even after a character has been removed)
        char FirstLetter = currentPassword[0];

        //Checks if the players character input (QERF) is equal to the first letter in the combination (Player must enter the character in order)
        if (PlayerInput == FirstLetter)
        {
            //plays first bell sound

            correctCounter++;

            if (correctCounter == 1)
            {
                correctOne.GetComponent<Image>().color = Color.green;
            }

            else if (correctCounter == 2)
            {
                correctTwo.GetComponent<Image>().color = Color.green;
            }

            else if (correctCounter == 3)
            {
                correctThree.GetComponent<Image>().color = Color.green;
            }

            else if (correctCounter == 4)
            {
                correctFour.GetComponent<Image>().color = Color.green;
            }

            else if (correctCounter == 5)
            {
                correctFive.GetComponent<Image>().color = Color.green;
            }

            Debug.Log("OG:" + currentPassword);

            // If the player entered the correct character, it removes that character from the combination
            currentPassword = currentPassword.Remove(0, 1);

            Debug.Log("After:" + currentPassword);

            // Lights up the corresponding letter that the player pressed, and resets it back to white after .5 seconds
           

            if (currentPassword == "")
            {
                // If the player completes the combination, it tells the player what spell they casted and sets SpellHeld to true, changes CurrentSpell to the spell that the player is holding
                Debug.Log("Password Correct");
               
                Player.GetComponent<ThirdPersonController>().CurrentGenerator.GetComponent<GeneratorScript>().minigamesComplete++;
                Player.GetComponent<ThirdPersonController>().CurrentGenerator.GetComponent<GeneratorScript>().UpdateLights();
                if (Player.GetComponent<ThirdPersonController>().CurrentGenerator.GetComponent<GeneratorScript>().minigamesComplete == 3)
                {
                    Player.GetComponent<ThirdPersonController>().condensors += Random.Range(1,4);
                    Player.GetComponent<ThirdPersonController>().UpdateUI();
                }

                Invoke("ResetInteractNum", 3f);
                Invoke("CloseMinigame", 1f);
            }

        }

    }

    public void CloseMinigame()
    {
        PasswordMinigame.SetActive(false);
        Player.GetComponent<ThirdPersonController>().InPuzzle = false;

        correctOne.GetComponent<Image>().color = Color.red;
        correctTwo.GetComponent<Image>().color = Color.red;
        correctThree.GetComponent<Image>().color = Color.red;
        correctFour.GetComponent<Image>().color = Color.red;
        correctFive.GetComponent<Image>().color = Color.red;

        correctCounter = 0;

        Player.GetComponent<ThirdPersonController>().UIOpaque();
    }

   public void ResetInteractNum()
    {
        Player.GetComponent<ThirdPersonController>().interactTextNum = 0;
    }
}
