using System.Collections.Generic;
using UnityEngine;

public class PlayerGrenadeDetect : MonoBehaviour
{
    [SerializeField] bool PlayerInField = false;
    public GameObject Player;
    [SerializeField] GameObject ParentGrenade;
    public List<GameObject> activeEnemies = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("ThirdPersonTEST");
        Invoke("BoomCountdown", 4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Enemy":

                if (!activeEnemies.Contains(other.gameObject))
                {
                    activeEnemies.Add(other.gameObject);
                }

                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Enemy":
                activeEnemies.RemoveAll(item => item == null);

                if (activeEnemies.Contains(other.gameObject))
                {
                    activeEnemies.Remove(other.gameObject);
                }
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Enemy":
                activeEnemies.RemoveAll(item => item == null);

                if (!activeEnemies.Contains(other.gameObject))
                {
                    activeEnemies.Add(other.gameObject);
                }
                break;
        }
    }

    void BoomCountdown()
    {

        for (int i = 0; i < activeEnemies.Count; i++) 
        {
            activeEnemies[i].GetComponentInChildren<BanditHealth>().banditHealth -= 75;
        }

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
