using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;


public class BanditEnemy : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public GameObject fireBall;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float speed = 3f;
    public bool canShoot = true;
    public bool canMove = true;
    public int activeActionNum;
    public List<string> possibleActions = new List<string>() {"Hide", "Fight", "Heal", "Grenade"};
    public List<GameObject> activeCovers = new List<GameObject>();
    public bool hiding, fighting, healing, grenading;
    bool collectingInitial = true;
    private Rigidbody rb;
    Vector3 movePos;
    private bool moveTime = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        StartCoroutine(IntialScan());
        player = GameObject.Find("MainPlayer");
        firePoint = player.transform; 
        hiding = false;
        fighting = false;
        healing = false;
        grenading = false;
    }

    IEnumerator IntialScan()
    {
        yield return new WaitForSeconds(1f); // wait one frame
        collectingInitial = false;
    }

    private void FixedUpdate()
    {
        if (moveTime)
        {
            Vector3 MoveFunction = Vector3.MoveTowards(enemy.transform.position, movePos, speed * Time.deltaTime);
            enemy.transform.position = MoveFunction;
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                activeActionNum = Random.Range(0, 1);
                string currentAction = possibleActions[activeActionNum];

                switch (currentAction)
                {
                    case "Hide":
                        hiding = true;
                        Invoke("Hide", Random.Range(1f,3f));
                        break;

                    case "Fight":
                        fighting = true;
                        break;

                    case "Heal":
                        healing = true;
                        break;

                    case "Grenade":
                        grenading |= true;
                        break;
                }
                break;

            case "CoverObject":

                activeCovers.RemoveAll(item => item == null);
                
                  if (!activeCovers.Contains(other.gameObject))
                  {
                      activeCovers.Add(other.gameObject);
                  }
                
                break;
        }
    }
    private void OnTriggerStay(Collider other)
    {
       
        switch (other.gameObject.tag)
        {
            case "Player":
                if (canShoot)
                {
                  
                }
                break;

            case "CoverObject":
                activeCovers.RemoveAll(item => item == null);

                if (collectingInitial)
                {
                    if (!activeCovers.Contains(other.gameObject))
                    {
                        activeCovers.Add(other.gameObject);
                    }
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag) 
        {
            case "CoverObject":

                activeCovers.RemoveAll(item => item == null);

                if (activeCovers.Contains(other.gameObject))
                {
                    activeCovers.Remove(other.gameObject);
                }

                break;
        }
    }
   

    void ShootProjectileDelay()
    {
        //ShootProjectile(firePoint);
        

    }


    /*void ShootProjectile(Transform firePoint)
    {
        var projectileObj = Instantiate(fireBall, firePoint.position, Quaternion.identity) as GameObject;
        Vector3 direction = (player.transform.position - firePoint.position).normalized;
        projectileObj.GetComponent<Rigidbody>().linearVelocity = direction * projectileSpeed;
        Destroy(projectileObj, 5f);
    }
    */

    void Hide()
    {
        for (int i = 0; i < activeCovers.Count; i++)
        {
            if (!activeCovers[i].GetComponentInChildren<CoverPointScript>().isCovered)
            {
                activeCovers[i].GetComponentInChildren<CoverPointScript>().isCovered = true;
                movePos = activeCovers[i].GetComponentInChildren<CoverPointScript>().gameObject.transform.position;
                Debug.Log(movePos);
                moveTime = true;
                break;
            }

            else
            {
                Debug.Log("GameBroken");
                continue;
            }
        }

    }


}