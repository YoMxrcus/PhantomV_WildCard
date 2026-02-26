using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class RangedEnemyDetect : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public GameObject fireBall;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public bool canShoot = true;
    public bool canMove = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Animator WizardAnim;
    void Start()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                if (canShoot)
                {
                    canMove = false;
                    WizardAnim.SetBool("AttackBool", true);
                    Invoke("ShootProjectileDelay", 1.5f);
                    canShoot = false;
                    Invoke("AllowMovement", 3.5f);
                    Invoke("AllowEnemyShooting", 6.5f);
                   
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WizardAnim.SetBool("WalkingBool", false);
            WizardAnim.SetBool("AttackBool", false);
            //StopMovement();
        }
    }
    void AllowEnemyShooting()
    {
        canShoot = true;
    }
    
    void AllowMovement()
    {
        canMove = true;
    }

    void ShootProjectileDelay()
    {
        ShootProjectile(firePoint);
        WizardAnim.SetBool("AttackBool", false);
        WizardAnim.SetBool("WalkingBool", false);

    }


    void ShootProjectile(Transform firePoint)
    {
        var projectileObj = Instantiate(fireBall, firePoint.position, Quaternion.identity) as GameObject;
        Vector3 direction = (player.transform.position - firePoint.position).normalized;
        projectileObj.GetComponent<Rigidbody>().linearVelocity = direction * projectileSpeed;
        Destroy(projectileObj, 5f);
    }
}
