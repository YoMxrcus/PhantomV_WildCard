using NUnit.Framework.Constraints;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HeatMeter : MonoBehaviour
{
    public Slider heatBar;
    public GameObject clickArea;
    RectTransform clickAreaPos;
    public float heat;
    bool max;
    public float tolerance = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the RectTransform component so we can actually move the UI element
        clickAreaPos = clickArea.GetComponent<RectTransform>();

        // Apply it to the anchoredPosition (this stays within the UI Canvas)
        clickAreaPos.anchoredPosition = new Vector2(0, Random.Range(-200f, 205f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (heat < 100 && max != true)
        {
            heat++;
            if (heat == 100)
            {
                max = true;
            }
        }

        if (heat > 0 && max == true)
        {
            heat--;
            if (heat == 0)
            {
                max = false;
            }
        }
        if (Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            // 1. Get the local Y position of the click area
            float yPos = clickAreaPos.anchoredPosition.y;

            // 2. Convert the Y Position (-200 to 205) into a 0-100 scale
            // This math "remaps" the pixel range to match your heat bar units
            float targetHeatValue = Mathf.InverseLerp(-200f, 205f, yPos) * 100f;

            // 3. Compare the current heat to the target heat
            if (Mathf.Abs(heat - targetHeatValue) <= tolerance)
            {
                Debug.Log("Success! Perfect Timing!");
                // Add your success logic here (e.g., +points, reset bar)
                clickArea.GetComponent<RawImage>().color = Color.green;


            }
            else
            {
                Debug.Log("Missed! You were at " + heat + " but needed to be near " + targetHeatValue);
            }
        }
        UpdateUI();

    }
    void UpdateUI()
    {
        heatBar.value = heat;
    }
}