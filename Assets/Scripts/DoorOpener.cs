using System.Collections;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{

    public Vector3 StartPos;
    public Vector3 CurrentPos;
    public Vector3 TargetPos;

    public float Speed;

    public int DebugInt = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CloseDoor()
    {
        while (transform.position != TargetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPos, Speed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void OpenDoor()
    {
        StartCoroutine(CloseDoor());
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && DebugInt == 0)
        {
            DebugInt++;
            StartCoroutine(CloseDoor());
        }
    }

    */



}
