using UnityEngine;

public class Stencil : MonoBehaviour
{

    public Animator sphereAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sphereAnimator = GameObject.Find("StencilSphere").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Building":
                sphereAnimator.SetTrigger("INC");
                return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Building":
                sphereAnimator.SetTrigger("DEC");
                return;
        }
    }
}
