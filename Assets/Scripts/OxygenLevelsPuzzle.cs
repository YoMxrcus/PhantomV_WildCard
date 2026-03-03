using UnityEngine;
using UnityEngine.UI;

public class OxygenLevelsPuzzle : MonoBehaviour
{
    public float oxygen;
    public Slider bar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        oxygen = Random.Range(10, 101);
        UpdateUI();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateUI();
        oxygen -= .05f;
    }
    void UpdateUI()
    {
        bar.value = oxygen;
    }
    public void RaiseOxygen()
    {
        oxygen++;
        UpdateUI();
    }
}
