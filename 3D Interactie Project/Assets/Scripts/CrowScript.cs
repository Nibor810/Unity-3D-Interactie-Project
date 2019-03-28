using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowScript : MonoBehaviour
{
    private bool activated = false;
    private Rigidbody rb;
    private Vector3 setTarget;    

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            rb.MovePosition(transform.position + setTarget * Time.deltaTime);
        }
    } 

    public void Activate(Vector3 target)
    {
        transform.LookAt(target);
        setTarget = target;
        activated = true;
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, (1 + Random.value * 3));
    }
}
