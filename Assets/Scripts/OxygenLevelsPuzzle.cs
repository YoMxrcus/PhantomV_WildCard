using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OxygenLevelsPuzzle : MonoBehaviour
{
    public float oxygen, oxygen2, oxygen3;
    public float min, min2, min3, max, max2, max3;
    public Slider oxygenBar, oxygenBar2, oxygenBar3;
    public Image fillImage, fillImage2, fillImage3;

    public GameObject timer, timer2, timer3;
    public float time, time2, time3;

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
        max = min + 30; max2 = min2 + 30; max3 = min3 + 30;
        UpdateUI();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Adding Oxygen

        if (Keyboard.current[Key.Digit1].isPressed && oxygen < 100 && !complete)
        { oxygen++; }

        if(oxygen > min && oxygen < max && CoStarted1 == false)
        { CoStarted1 = true; Cor1 = StartCoroutine(CompleteValues()); }
        else if (oxygen > min && oxygen < max) { }
        else
        {
            CoStarted1 = false;
            if (Cor1 != null)
            {
                StopCoroutine(Cor1);
            }
        }

        if (Keyboard.current[Key.Digit2].isPressed && oxygen2 < 100 && !complete2)
        { oxygen2++; }

        if (oxygen2 > min2 && oxygen2 < max2 && CoStarted2 == false)
        { CoStarted2 = true; Cor2 = StartCoroutine(CompleteValues2()); }
        else if (oxygen2 > min2 && oxygen2 < max2) { }
        else
        {
            CoStarted2 = false;
            if (Cor2 != null)
            {
                StopCoroutine(Cor2);
            }
        }

        if (Keyboard.current[Key.Digit3].isPressed && oxygen3 < 100 && !complete3)
        { oxygen3++; }

        if (oxygen3 > min3 && oxygen3 < max3 && CoStarted3 == false)
        { CoStarted3 = true; Cor3 = StartCoroutine(CompleteValues3()); }
        else if (oxygen3 > min3 && oxygen3 < max3) { }
        else
        {
            CoStarted3 = false;
            if (Cor3 != null)
            {
                StopCoroutine(Cor3);
            }
        }

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


    IEnumerator CompleteValues()
    {
        yield return new WaitForSeconds(3f);
        complete = true;
        fillImage.color = Color.green;
    }
    IEnumerator CompleteValues2()
    {
        yield return new WaitForSeconds(3f);
        complete2 = true;
        fillImage2.color = Color.green;
    }
    IEnumerator CompleteValues3()
    {
        yield return new WaitForSeconds(3f);
        complete3 = true;
        fillImage3.color = Color.green;
    }
}
