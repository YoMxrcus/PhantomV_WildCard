using UnityEngine;

public class GrenadeScript : MonoBehaviour
{

    [SerializeField] Vector3 HitPoint;
    bool HitGround = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
                break;
        }
    }
}
