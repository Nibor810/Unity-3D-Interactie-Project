using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectToPoint : MonoBehaviour
{
    public GameObject hasObjectParticles;
    public ParticleSystem fireObjectParticles;
    public GameObject attractObjectParticles;


    public GameObject orb;
    public Vector3 direction;
    public Vector3 destination;
    public float moveSpeed = 0.5f;
    public float attractSpeed = 2.0f;
    public float onDestinationTreshhold = 0.01f;
    private Rigidbody rb;

    public bool hasObject = false;
    public bool isGrabbingObject = false;


    private Vector3 centerPoint;
    public Vector3 directionX;
    public Vector3 directionY;
    public Vector3 directionZ;
    public float maxRadius = 0.2f;
    private float xSpeed = 1;
    private float ySpeed = 1;
    private float zSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        xSpeed = Random.Range(0.5f, 1.5f);
        ySpeed = Random.Range(0.5f, 1.5f);
        zSpeed = Random.Range(0.5f, 1.5f);
        directionX = Vector3.right * xSpeed;
        directionY = Vector3.up * ySpeed;
        directionZ = Vector3.forward * zSpeed;
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (hasObject)
        {
            IdleMovement();
            if (Input.GetKeyDown(KeyCode.F))
            {
                ShootObjectAway();
                hasObject = false;
            }
        }
        else if (isGrabbingObject)
        {
            if (Vector3.Distance(destination, orb.transform.position) < onDestinationTreshhold)
            {
                DoOnArrival();
            }
            else
            {
                //if(Vector3.Distance(destination, orb.transform.position) < onDestinationTreshhold)
                direction = (destination - orb.transform.localPosition).normalized;
                rb.MovePosition(orb.transform.position + direction * attractSpeed * Time.deltaTime);
            }
        }
        else
        {
            //CanGrabItem
            if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG");
                isGrabbingObject = true;
                rb = orb.GetComponent<Rigidbody>();
                rb.useGravity = false;
                attractObjectParticles.SetActive(true);
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

    }

    private void IdleMovement()
    {
        if (orb.transform.localPosition.x >= centerPoint.x + maxRadius)
        {
            xSpeed = Random.Range(0.5f, 1.5f);
            directionX = -Vector3.right * xSpeed;
        }
        else if (orb.transform.localPosition.x <= centerPoint.x - maxRadius)
        {
            xSpeed = Random.Range(0.5f, 1.5f);
            directionX = Vector3.right * xSpeed;
        }


        if (orb.transform.localPosition.y >= centerPoint.y + maxRadius)
        {
            ySpeed = Random.Range(0.5f, 1.5f);
            directionY = -Vector3.up * ySpeed;

        }
        else if (orb.transform.localPosition.y <= centerPoint.y - maxRadius)
        {
            ySpeed = Random.Range(0.5f, 1.5f);
            directionY = Vector3.up * ySpeed;

        }

        if (orb.transform.localPosition.z >= centerPoint.z + maxRadius)
        {
            zSpeed = Random.Range(0.5f, 1.5f);
            directionZ = -Vector3.forward * zSpeed;
        }
        else if (orb.transform.localPosition.z <= centerPoint.z - maxRadius)
        {
            zSpeed = Random.Range(0.5f, 1.5f);
            directionZ = Vector3.forward * zSpeed;
        }

        direction = (directionX + directionY + directionZ).normalized;

        rb.MovePosition(orb.transform.position + direction * moveSpeed * Time.deltaTime);
    }

    private void DoOnArrival()
    {
        Debug.Log("Arrived");
        isGrabbingObject = false;
        hasObject = true;
        orb.transform.position = transform.position;
        centerPoint = orb.transform.localPosition;
        attractObjectParticles.SetActive(false);
        hasObjectParticles.SetActive(true);
    }

    private void ShootObjectAway()
    {
        Debug.Log("Shoot");
        rb.useGravity = true;
        rb.AddForce(Vector3.forward * 1000);
        hasObject = false;
        rb = null;
        attractObjectParticles.SetActive(false);
        hasObjectParticles.SetActive(false);
        fireObjectParticles.Play();
    }
}
