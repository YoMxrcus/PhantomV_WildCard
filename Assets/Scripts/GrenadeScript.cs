using UnityEngine;

public class GrenadeScript : MonoBehaviour
{

    [SerializeField] Vector3 HitPoint;
    bool HitGround = false;
    [SerializeField]GameObject EffectArea;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EffectArea = transform.Find("Sphere").GetComponent<GrenadeDetectScript>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (HitGround)
        {
            transform.position = HitPoint;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Ground":
                HitPoint = transform.position;
                HitGround = true;
                EffectArea.SetActive(true);
                Debug.Log("Set True!");
                break;
        }
    }
}
