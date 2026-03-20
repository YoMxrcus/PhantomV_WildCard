using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HeatMeter : MonoBehaviour
{
    public Slider heatBar;
    public Slider heatBar2;
    public Slider heatBar3;
    public GameObject clickArea, clickArea2, clickArea3;
    RectTransform clickAreaPos, clickAreaPos2, clickAreaPos3;
    public float heat, heat2, heat3;
    bool max, complete, max2, complete2, max3, complete3;
    public float tolerance = 5f;
    public GameObject HeatMinigame;
    public GameObject Player;

   public void BeginMinigame()
    {
        HeatMinigame.SetActive(true);
        // 1. FIXED: Get the correct component for each
        clickAreaPos = clickArea.GetComponent<RectTransform>();
        clickAreaPos2 = clickArea2.GetComponent<RectTransform>(); // Fixed this
        clickAreaPos3 = clickArea3.GetComponent<RectTransform>(); // Fixed this

        // Apply it to the anchoredPosition (this stays within the UI Canvas)
        clickAreaPos.anchoredPosition = new Vector2(0, Random.Range(-200f, 205f));
        clickAreaPos2.anchoredPosition = new Vector2(0, Random.Range(-200f, 205f));
        clickAreaPos3.anchoredPosition = new Vector2(0, Random.Range(-200f, 205f));
    }

    void FixedUpdate()
    {
        // --- Heat 1 ---
        if (heat < 100 && max != true && !complete)
        {
            heat += 1.5f;
            if (heat >= 100) { max = true; } // Used >= for safety
        }
        if (heat > 0 && max == true && !complete)
        {
            heat -= 1.5f;
            if (heat <= 0) { max = false; }
        }

        // --- Heat 2 ---
        if (heat2 < 100 && max2 != true && !complete2)
        {
            heat2 += 2;
            if (heat2 >= 100) { max2 = true; }
        }
        if (heat2 > 0 && max2 == true && !complete2)
        {
            heat2 -= 2;
            if (heat2 <= 0) { max2 = false; }
        }

        // --- Heat 3 ---
        if (heat3 < 100 && max3 != true && !complete3)
        {
            heat3 += 1.75f;
            if (heat3 >= 100) { max3 = true; }
        }
        if (heat3 > 0 && max3 == true && !complete3)
        {
            heat3 -= 1.75f;
            if (heat3 <= 0) { max3 = false; }
        }

        UpdateUI();
    }

    private void Update()
    {
        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            // 1. Get the local Y position of each individual click area
            float yPos = clickAreaPos.anchoredPosition.y;
            float yPos2 = clickAreaPos2.anchoredPosition.y;
            float yPos3 = clickAreaPos3.anchoredPosition.y;

            // 2. Convert the Y Position (-200 to 205) into a 0-100 scale for each
            float targetHeatValue = Mathf.InverseLerp(-200f, 205f, yPos) * 100f;
            float targetHeatValue2 = Mathf.InverseLerp(-200f, 205f, yPos2) * 100f; // Fixed: Used yPos2
            float targetHeatValue3 = Mathf.InverseLerp(-200f, 205f, yPos3) * 100f; // Fixed: Used yPos3

            // 3. Compare the current heat to the target heat individually

            // Bar 1
            if (!complete && Mathf.Abs(heat - targetHeatValue) <= tolerance)
            {
                Debug.Log("Bar 1 Success!");
                clickArea.GetComponent<RawImage>().color = Color.green;
                complete = true;
                CheckWin();
            }

            // Bar 2
            if (!complete2 && Mathf.Abs(heat2 - targetHeatValue2) <= tolerance) // Fixed: Used targetHeatValue2
            {
                Debug.Log("Bar 2 Success!");
                clickArea2.GetComponent<RawImage>().color = Color.green;
                complete2 = true;
                CheckWin();
            }

            // Bar 3
            if (!complete3 && Mathf.Abs(heat3 - targetHeatValue3) <= tolerance) // Fixed: Used targetHeatValue3
            {
                Debug.Log("Bar 3 Success!");
                clickArea3.GetComponent<RawImage>().color = Color.green;
                complete3 = true;
                CheckWin();
            }
        }
    }

    void UpdateUI()
    {
        heatBar.value = heat;
        heatBar2.value = heat2;
        heatBar3.value = heat3;
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
            if (Player.GetComponent<ThirdPersonController>().CurrentGenerator.GetComponent<GeneratorScript>().minigamesComplete == 3)
            {
                Player.GetComponent<ThirdPersonController>().condensors++;
            }
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

        clickArea.GetComponent<RawImage>().color = Color.yellow;
        clickArea2.GetComponent<RawImage>().color = Color.yellow;
        clickArea3.GetComponent<RawImage>().color = Color.yellow;

        HeatMinigame.SetActive(false);


    }

    public void ResetInteractNum()
    {
        Player.GetComponent<ThirdPersonController>().interactTextNum = 0;
    }

    
}
