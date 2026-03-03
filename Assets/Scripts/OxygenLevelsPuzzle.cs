using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OxygenLevelsPuzzle : MonoBehaviour
{
    public float oxygen, oxygen2, oxygen3;
    public float min, min2, min3, max, max2, max3;
    public Slider oxygenBar, oxygenBar2, oxygenBar3;
    public bool complete = false;
    public bool complete2 = false;
    public bool complete3 = false;

    public bool CoStarted1= false;
    public bool CoStarted2 = false;
    public bool CoStarted3 = false;

    public Coroutine Cor1;
    public Coroutine Cor2;
    public Coroutine Cor3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        oxygen = Random.Range(10, 101); oxygen2 = Random.Range(10, 101); oxygen3 = Random.Range(10, 101);
        min = Random.Range(20, 80); min2 = Random.Range(20, 80); min3 = Random.Range(20, 80);
        max = min + 30; max2 = min2 + 10; max3 = min3 + 10;
        UpdateUI();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Adding Oxygen

        if (Keyboard.current[Key.Digit1].isPressed && oxygen < 100 && !complete)
        { oxygen++; }

        if(oxygen > min && oxygen < max && CoStarted1 == false)
        { CoStarted1 = true; Cor1 = StartCoroutine(CompleteValues(1)); }
        else if (oxygen > min && oxygen < max) { }
        else
        {
            CoStarted1 = false;
            if (Cor1 != null)
            {
                StopCoroutine(Cor1);
            }

        }

        if (Keyboard.current[Key.Digit2].isPressed && oxygen2 < 100)
        { oxygen2++; }

        if (Keyboard.current[Key.Digit3].isPressed && oxygen3 < 100)
        { oxygen3++; }

        //Update Bars
        UpdateUI();
        if (oxygen > 0 && !complete)
        { oxygen -= .25f; }
        else {}
        if (oxygen2 > 0 && !complete2)
        { oxygen2 -= .25f; }
        else {}
        if (oxygen3 > 0 && !complete3)
        { oxygen3 -= .25f; }
        else {}
    }
    void UpdateUI()
    {
        oxygenBar.value = oxygen;
        oxygenBar2.value = oxygen2;
        oxygenBar3.value = oxygen3;
    }


    IEnumerator CompleteValues(int num)
    {
        yield return new WaitForSeconds(3f);
        complete = true;
    }
}
