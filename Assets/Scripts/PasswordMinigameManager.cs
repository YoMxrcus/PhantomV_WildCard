using UnityEngine;
using UnityEngine.InputSystem;

public class PasswordMinigameManager : MonoBehaviour
{

    [SerializeField] string[] comboLetters = new string[4];
    public char PlayerInput;
    public string currentPassword;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BeginMinigame();
        Debug.Log(currentPassword);
    }

    // Update is called once per frame
    void Update()
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

    void BeginMinigame()
    {
        for (int i = 0; i < 5; i++)
        {
            currentPassword += comboLetters[Random.Range(0, comboLetters.Length)];

        }
    }

    void CheckPassword()
    {


        //Creates a variable that holds the first character of the combination (Even after a character has been removed)
        char FirstLetter = currentPassword[0];

        //Checks if the players character input (QERF) is equal to the first letter in the combination (Player must enter the character in order)
        if (PlayerInput == FirstLetter)
        {
            //plays first bell sound


            Debug.Log("OG:" + currentPassword);

            // If the player entered the correct character, it removes that character from the combination
            currentPassword = currentPassword.Remove(0, 1);

            Debug.Log("After:" + currentPassword);

            // Lights up the corresponding letter that the player pressed, and resets it back to white after .5 seconds
           

            if (currentPassword == "")
            {
                // If the player completes the combination, it tells the player what spell they casted and sets SpellHeld to true, changes CurrentSpell to the spell that the player is holding
                Debug.Log("Password Correct");
            }

        }

    }
}
