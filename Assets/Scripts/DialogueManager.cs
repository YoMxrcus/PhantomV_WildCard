/*
 * This Script is for formating CSV files to text in unity and then dispalying ut on the screen with text boxes
 */


using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    //CSV Finder
    public TextAsset csvDialogue;
    public string[,] dialougeGrid;

    //Diolouge Box
    public GameObject dialougeBox;
    public TMP_Text dialougeTxt, nameTxt;
    public float dialogueType = 0.05f;
    public GameObject SpeakingFace;

    //DialougeVariables
    public bool canTalk, startedPlaying, currentlyPlaying, donePlaying;
    public bool doneTyping, textBoxOpen;
    public string commaCheckTxt;
    public string commaCheckName;
    public float inBetweenNumber = 0.01f ;

    //Indexes
    public int characterID;
    public int currentTextIndex;
    public int lengthOfDialouge;
    public Coroutine dioCor;

    //animator
    public Animator textBoxAnimator;
 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //How to call items in the CSV - grid[x, y] where x and y are numbers starting from 0 (A1 on the excel sheet would be 0,0 and A2 on the excell sheet would be 0,1)
        //How to call certain characters - grid[2 + currentTextIndex, characterID]
        csvDialogue = Resources.Load<TextAsset>("CSVFiles/TestingDio_01");
        dialougeGrid = getCSVGrid(csvDialogue.text);

        dialougeBox.SetActive(false);
        textBoxOpen = false;

        characterID = 1;
        currentTextIndex = 0;

        doneTyping = true;



    }

    // Update is called once per frame
    void Update()
    {



    }

    public string[,] getCSVGrid(string csvText)
    {
        //split the data on split line character
        string[] lines = csvText.Split("\n"[0]);

        // find the max number of columns
        int totalColumns = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            totalColumns = Mathf.Max(totalColumns, row.Length);
        }

        // creates new 2D string grid to output to
        string[,] outputGrid = new string[totalColumns + 1, lines.Length + 1];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = lines[y].Split(',');

            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];
            }
        }

        return outputGrid;
    }

    public IEnumerator Dialogue(string message)
    {
        //pick message to show
        dialougeTxt.text = "";
        string newDialogue = message;

        doneTyping = false;


        //type out message
        foreach (char c in newDialogue)
        {
            dialougeTxt.text += c;
            yield return new WaitForSeconds(dialogueType);
        }

        doneTyping = true;

        yield return new WaitForSeconds(2f);
        ClickForward();
    }


  
    public void OpenTextBox(int characterIndex)
    {
        
        CloseTextBox();
        textBoxOpen = true;
        currentTextIndex = 0;
        characterID = characterIndex;
        doneTyping = true;
        dialougeBox.SetActive(true);
        PlayDio();
    }

    public void ClickForward()
    {
        Debug.Log(currentTextIndex);
        Debug.Log(dialougeGrid[currentTextIndex, characterID]);
        string EndCheck = dialougeGrid[currentTextIndex + 4, characterID];
        Debug.Log(EndCheck);

        if ((EndCheck == "END TEXT" || EndCheck == null || EndCheck == "END TEXT\r\n" || EndCheck == "") & doneTyping)
        {
            Debug.Log("Attempting To Close");
            Invoke("CloseTextBox", 1f);
        }
        else
        {
            if (doneTyping)
            {
                currentTextIndex += 2;
            }

            PlayDio();
        }
        
        

    }


    public void CloseTextBox()
    {
        if (dioCor != null)
        {
            StopCoroutine(dioCor);
        }
        dialougeTxt.text = "";
        nameTxt.text = "";
        dialougeBox.SetActive(false);
        textBoxOpen = false;
       
    }

    void PlayDio() //plays the dialouge inside the text box depending on the character ID and page index
    {
        CommaChecker();

        if (doneTyping && currentTextIndex == 0) //if the player clicks the button after typing, move to the next text dialouge
        {
            nameTxt.text = commaCheckName;
            dioCor = StartCoroutine(Dialogue(commaCheckTxt));
            ChangeCharacter();

        }

        else if (doneTyping)
        {
            nameTxt.text = commaCheckName;
            dioCor = StartCoroutine(Dialogue(commaCheckTxt));
            ChangeCharacter();
        }

        else //if they click forwards before the text types, skip to the end and finish the text
        {
            if (SceneManager.GetActiveScene().name != "OutroLevelThree")
            {
                doneTyping = true;
                StopCoroutine(dioCor);
                nameTxt.text = commaCheckName;
                dialougeTxt.text = commaCheckTxt;
                ChangeCharacter();
            }
            
        }

        
    }

    void CommaChecker()
    {
        commaCheckName = dialougeGrid[currentTextIndex + 1, characterID];
        commaCheckName = commaCheckName.Replace("`", ",");

        commaCheckTxt = dialougeGrid[currentTextIndex + 2, characterID];
        commaCheckTxt = commaCheckTxt.Replace("`", ",");
    }
    void ChangeCharacter()
    {
        switch (nameTxt.text)
        {
            case "Alejandro":
                SpeakingFace.GetComponent<Image>().sprite = Resources.Load<Sprite>("PlaceHolderFace");
                break;
            case "Aidan":
                SpeakingFace.GetComponent<Image>().sprite = Resources.Load<Sprite>("PlaceHolderFace2");
                break;
            case "Test One":
                SpeakingFace.GetComponent<Image>().sprite = Resources.Load<Sprite>("Bandit");
                break;
            case "Test Two":
                SpeakingFace.GetComponent<Image>().sprite = Resources.Load<Sprite>("Lady");
                break;
            case "Test Three":
                SpeakingFace.GetComponent<Image>().sprite = Resources.Load<Sprite>("MC");
                break;
            case "Evil Wizards":
                SpeakingFace.GetComponent<Image>().sprite = Resources.Load<Sprite>("EvilWizardHeadshot");
                break;

            case "Bartholomew":
                SpeakingFace.GetComponent<Image>().sprite = Resources.Load<Sprite>("EvilWizardHeadshot");
                break;



        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DioTrigger")
        {
            int num = int.Parse(other.gameObject.name);
            OpenTextBox(num);
        }
        
    }

    /*Ref For Using Dialouge
     * 
    public IEnumerator Lvl01Dio01()
    {

        OpenTextBox(1);

        textBoxOpen = true;


        while (textBoxOpen)
        {

            yield return new WaitForSeconds(inBetweenNumber);
        }

        StopCoroutine(dioCor);

        OpenTextBox(2);

        while (textBoxOpen)
        {
            yield return new WaitForSeconds(inBetweenNumber);
        }

        StopCoroutine(dioCor);

        OpenTextBox(3);

        while (textBoxOpen)
        {
            yield return new WaitForSeconds(inBetweenNumber);
        }

        StopCoroutine(dioCor);

        OpenTextBox(4);

        while (textBoxOpen)
        {
            yield return new WaitForSeconds(inBetweenNumber);
        }

        StopCoroutine(dioCor);

        OpenTextBox(5);
    }

   */
}

