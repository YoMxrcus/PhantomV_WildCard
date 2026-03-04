using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HeatMeter : MonoBehaviour
{
    public Slider heatBar;
    public float heat;
    bool max;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (heat < 100 && max != true)
        { 
            heat++;
            if(heat == 100)
            {
                max = true;
            }
        }

        if (heat > 0 && max == true)
        { 
            heat--;
            if(heat == 0)
            {
                max = false;
            }
        }
        UpdateUI();


    }
    void UpdateUI()
    {
        heatBar.value = heat;
    }
}
