using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class OxygenLevelsPuzzle : MonoBehaviour
{
    public float oxygen, oxygen2, oxygen3;
    public float min, min2, min3, max, max2, max3;
    public Slider oxygenBar, oxygenBar2, oxygenBar3;
    public Image fillImage, fillImage2, fillImage3;

    public TMP_Text timer, timer2, timer3;
    public float time = 3, time2 = 3, time3 = 3;

    public bool complete = false;
    public bool complete2 = false;
    public bool complete3 = false;

    RectTransform clickAreaPos, clickAreaPos2, clickAreaPos3;
    public GameObject clickArea, clickArea2, clickArea3;
    public GameObject OxygenMinigame;
    public GameObject Player;
    public int oxygenResetNum;
    public void BeginMinigame()
    {
        OxygenMinigame.SetActive(true);

        oxygenResetNum = 0;

        oxygen = Random.Range(10, 101); oxygen2 = Random.Range(10, 101); oxygen3 = Random.Range(10, 101);
        min = Random.Range(20, 80); min2 = Random.Range(20, 80); min3 = Random.Range(20, 80);
        max = min + 30; max2 = min2 + 30; max3 = min3 + 30;

        // 1. FIXED: Get the correct component for each
        clickAreaPos = clickArea.GetComponent<RectTransform>();
        clickAreaPos2 = clickArea2.GetComponent<RectTransform>(); // Fixed this
        clickAreaPos3 = clickArea3.GetComponent<RectTransform>(); // Fixed this

        // Apply it to the anchoredPosition (this stays within the UI Canvas)
        clickAreaPos.anchoredPosition = new Vector2(0, (min+max)*.5f-50);
        clickAreaPos2.anchoredPosition = new Vector2(0, (min2 + max2)*.5f-50);
        clickAreaPos3.anchoredPosition = new Vector2(0, (min3 + max3)*.5f-50);

        UpdateUI();
    }

    void FixedUpdate()
    {
        HandleOxygenInput();
        HandleOxygenDecay();
        if(oxygenResetNum == 0)
        {
        HandleTimerLogic();
        }
        UpdateUI();
    }

    void HandleOxygenInput()
    {
        if (Keyboard.current[Key.Digit1].isPressed && oxygen < 100 && !complete) oxygen++;
        if (Keyboard.current[Key.Digit2].isPressed && oxygen2 < 100 && !complete2) oxygen2++;
        if (Keyboard.current[Key.Digit3].isPressed && oxygen3 < 100 && !complete3) oxygen3++;
    }

    void HandleOxygenDecay()
    {
        if (oxygen > 0 && !complete) oxygen -= 0.25f;
        if (oxygen2 > 0 && !complete2) oxygen2 -= 0.25f;
        if (oxygen3 > 0 && !complete3) oxygen3 -= 0.25f;
    }

    void HandleTimerLogic()
    {
        // Oxygen 1
        if (oxygen > min && oxygen < max && !complete)
        {
            time -= Time.deltaTime;
            if (time <= 0) { complete = true; time = 0; fillImage.color = Color.green; CheckWin(); }
        }
        else if (!complete) { time = 3; }

        // Oxygen 2
        if (oxygen2 > min2 && oxygen2 < max2 && !complete2)
        {
            time2 -= Time.deltaTime;
            if (time2 <= 0) { complete2 = true; time2 = 0; fillImage2.color = Color.green; CheckWin(); }
        }
        else if (!complete2) { time2 = 3; }

        // Oxygen 3
        if (oxygen3 > min3 && oxygen3 < max3 && !complete3)
        {
            time3 -= Time.deltaTime;
            if (time3 <= 0) { complete3 = true; time3 = 0; fillImage3.color = Color.green; CheckWin(); }
        }
        else if (!complete3) { time3 = 3; }
    }

    void UpdateUI()
    {
        oxygenBar.value = oxygen;
        oxygenBar2.value = oxygen2;
        oxygenBar3.value = oxygen3;

        timer.text = time.ToString("F1");
        timer2.text = time2.ToString("F1");
        timer3.text = time3.ToString("F1");
    }

    void CheckWin()
    {
        if (complete & complete2 & complete3)
        {
            Debug.Log("GameWon");
            Player.GetComponent<ThirdPersonController>().HeaterUp -= 20;

            if (Player.GetComponent<ThirdPersonController>().HeaterUp < 0)
            {
                Player.GetComponent<ThirdPersonController>().HeaterUp = 0;
            }
            Player.GetComponent<ThirdPersonController>().CurrentGenerator.GetComponent<GeneratorScript>().minigamesComplete++;
            Player.GetComponent<ThirdPersonController>().CurrentGenerator.GetComponent<GeneratorScript>().UpdateLights();
            fillImage.color = Color.white;
            fillImage2.color = Color.white;
            fillImage3.color = Color.white;
            oxygenResetNum = 1;
            Invoke("ResetInteractNum", 3f);
            Invoke("CloseMinigame", 1f);
        }
    }

   public void CloseMinigame()
    {
        Player.GetComponent<ThirdPersonController>().InPuzzle = false;

        complete = false;
        complete2 = false;
        complete3 = false;

       
        OxygenMinigame.SetActive(false);


    }

   public void ResetInteractNum()
    {
        Player.GetComponent<ThirdPersonController>().interactTextNum = 0;
    }
}