using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.Linq;
using System;

public enum Power
{
    None, Wind, Crow, Telekinetic
}


public class WindForceScript : MonoBehaviour
{
    #region ControllerInfo
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean selectAction;
    public SteamVR_Action_Boolean deselectAction;
    public SteamVR_Action_Vibration vibration;
    #endregion

    #region MoveObjectinfo
    private Vector3 centerPoint;
    private float directionX;
    private float directionY;
    private float directionZ;
    //public float maxRadius = 0.2f;
    private float xSpeed = 1;
    private float ySpeed = 1;
    private float zSpeed = 1;
    //public float moveSpeed = 0.5f;
    public ObjectAffectorScript objectAffector;
    #endregion

    #region PowerInfo
    public Power startPower = Power.None;
    public Power selectedPower;
    #endregion

    public float forceLimit = 3.0f;
    public float forceVariable = 20.0f;

    #region CrowPowers
    public int crowLifeTime = 5;
    public Vector3 crowPositionOffset;
    public Vector3 crowRotationOffset;
    public Transform crowSpawnPosition;
    public GameObject crowPrefab;
    #endregion

    #region TelekineticPowers
    public SteamVR_Action_Boolean grabItemWithTelikineticPowerAction;
    public Transform affectedObjectDestination;
    public GameObject affectedObject;
    private Rigidbody rbAffectedObject;
    private Vector3 direction;
    //private Vector3 destination;
    public float attractSpeed = 2.0f;
    public float idleSpeed = 0.2f;
    public float idleRadius = 0.1f;
    public float onDestinationTreshhold = 0.01f;

    public GameObject hasObjectParticles;
    public ParticleSystem fireObjectParticles;
    public GameObject attractObjectParticles;

    public LayerMask grabbableObjectMask;

    private bool hasObject = false;
    private bool isGrabbingObject = false;
    #endregion




    //public GameObject targetPointer;
    //public LayerMask mask;

    private List<GameObject> spawnedGameObjects = new List<GameObject>();

    public bool hasAccelerated = false;

    private void Start()
    {
        selectedPower = startPower;
    }

    // Update is called once per frame
    void Update()
    {
        switch (selectedPower)
        {
            case Power.Wind: WindUpdate();  break;
            case Power.Crow: CrowUpdate(); break;
            case Power.Telekinetic: UpdateTelekineticPower(); break;
            case Power.None: NoPowerUpdate(); break;

        }
    }

    private void NoPowerUpdate()
    {
        
    }

    public void SetPower(Power power)
    {
        selectedPower = power;
        hasAccelerated = false;
    }

    //Needs Work
    private void CrowUpdate()
    {
        Vector3 velocity = controllerPose.GetVelocity();
        if (selectAction.GetState(handType))
        {
            if (velocity.magnitude > 3 && !hasAccelerated)
            {
                hasAccelerated = true;
            }
            else if (velocity.magnitude < 1 && hasAccelerated)
            {
                Debug.Log("Crow");
                CastCrows(velocity);
                hasAccelerated = false;
            }
        }
    }

    #region TelekineticPowers
    //For kinetic powers
    private void StartTelekineticPower()
    {
        xSpeed = 1;// UnityEngine.Random.Range(0.5f, 1.5f);
        ySpeed = 1;//UnityEngine.Random.Range(0.5f, 1.5f);
        zSpeed = 1;//UnityEngine.Random.Range(0.5f, 1.5f);
        directionX = xSpeed;
        directionY = ySpeed;
        directionZ = zSpeed;
        //centerPoint = affectedObjectDestination.localPosition;
        //destination = transform.position;
    }

    //For kinetic powers
    private void UpdateTelekineticPower()
    {
        if (hasObject)
        {
            //Idle Animation
            IdleMovementAffectedObject();
            //vibration.Execute(0,Time.deltaTime*3,1.0f/Time.deltaTime,1,handType);


            //Shoot Function
            if (grabItemWithTelikineticPowerAction.GetStateUp(handType))
            {
                ShootObjectAway();
            }
        }
        else if (isGrabbingObject)
        {
            if (grabItemWithTelikineticPowerAction.GetStateUp(handType))
            {
                DropObject();
            }
            else
            {
                if (Vector3.Distance(affectedObjectDestination.position, affectedObject.transform.position) < onDestinationTreshhold)
                {
                    DoOnArrival();
                }
                else
                {
                    //Pull Item closer
                    direction = (affectedObjectDestination.position - affectedObject.transform.position).normalized;
                    rbAffectedObject.MovePosition(affectedObject.transform.position + direction * attractSpeed * Time.deltaTime);
                }
            }

        }
        else
        {
            //CanGrabItem
            if (grabItemWithTelikineticPowerAction.GetStateDown(handType))
            {
                StartGrabbingObjectTelekinetic();
            }
        }
    }

    //For kinetic powers
    private void DropObject()
    {
        Debug.Log("Drop");
        isGrabbingObject = false;
        rbAffectedObject.useGravity = true;
        hasObject = false;
        //affectedObject.transform.SetParent(null);
        affectedObject = null;
        rbAffectedObject = null;
        attractObjectParticles.SetActive(false);
        hasObjectParticles.SetActive(false);
    }

    //For kinetic powers
    private void StartGrabbingObjectTelekinetic()
    {
        Debug.Log("Grab");
        RaycastHit hit;
        if (Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100, grabbableObjectMask))
        {
            affectedObject = hit.collider.gameObject;
            if (affectedObject.GetComponent<Rigidbody>() != null)
            {
                rbAffectedObject = affectedObject.GetComponent<Rigidbody>();
                isGrabbingObject = true;
                rbAffectedObject.useGravity = false;
                attractObjectParticles.SetActive(true);
                rbAffectedObject.velocity = Vector3.zero;
                rbAffectedObject.angularVelocity = Vector3.zero;
            }
            else
            {
                affectedObject = null;
            }
        }
    }

    //For kinetic powers -> Gaat Foutish?
    private void IdleMovementAffectedObject()
    {
        //Debug.Log("Idle: "+ centerPoint + " Loc: "+ affectedObject.transform.localPosition);
        //rbAffectedObject.velocity = Vector3.zero;
        //rbAffectedObject.angularVelocity = Vector3.zero;

        centerPoint = affectedObjectDestination.position;
        
        if (affectedObject.transform.position.x >= centerPoint.x + idleRadius)
        {
            Debug.Log("xBoundry");
            xSpeed = UnityEngine.Random.Range(0.5f, 1.5f);
            directionX = -xSpeed;
        }
        else if (affectedObject.transform.position.x <= centerPoint.x - idleRadius) // centerpoint.x = 0 & maxRadius = 0.5f dus x <= -0.5f;
        {
            Debug.Log("-xBoundry");
            xSpeed = UnityEngine.Random.Range(0.5f, 1.5f); //1
            directionX = xSpeed;
        }

        if (affectedObject.transform.position.y >= centerPoint.y + idleRadius)
        {
            Debug.Log("yBoundry");
            ySpeed = UnityEngine.Random.Range(0.5f, 1.5f);
            directionY = -ySpeed;

        }
        else if (affectedObject.transform.position.y <= centerPoint.y - idleRadius)
        {
            Debug.Log("-yBoundry");
            ySpeed = UnityEngine.Random.Range(0.5f, 1.5f);
            directionY = ySpeed;

        }

        if (affectedObject.transform.position.z >= centerPoint.z + idleRadius)
        {
            Debug.Log("zBoundry");
            zSpeed = UnityEngine.Random.Range(0.5f, 1.5f);
            directionZ = -zSpeed;
        }
        else if (affectedObject.transform.position.z <= centerPoint.z - idleRadius)
        {
            Debug.Log("-zBoundry");
            zSpeed = UnityEngine.Random.Range(0.5f, 1.5f);
            directionZ = zSpeed;
        }

        direction = new Vector3(directionX, directionY, directionZ);
        float distanceSpeedModifier = 1f;
        if(Vector3.Distance(affectedObject.transform.position, centerPoint) > 0.3f)
        {
            //Go Faster.
            distanceSpeedModifier = 10;
        }

        rbAffectedObject.MovePosition(affectedObject.transform.position + (direction * idleSpeed * Time.deltaTime * distanceSpeedModifier));

        //affectedObject.transform.position = affectedObject.transform.position + (direction * idleSpeed * Time.deltaTime);

        Debug.Log("Loc: " + affectedObject.transform.position + "CenterPoint: "+centerPoint+" Direction: " + direction);
    }

    //For kinetic powers
    private void DoOnArrival()
    {
        Debug.Log("Arrived");

        isGrabbingObject = false;
        hasObject = true;
        affectedObject.transform.position = affectedObjectDestination.position;
        centerPoint = affectedObject.transform.localPosition;
        attractObjectParticles.SetActive(false);
        hasObjectParticles.SetActive(true);
        //affectedObject.transform.SetParent(transform);

        rbAffectedObject.velocity = Vector3.zero;
        rbAffectedObject.angularVelocity = Vector3.zero;
    }

    //For kinetic powers
    private void ShootObjectAway()
    {
        Debug.Log("Shoot");
        
        rbAffectedObject.useGravity = true;
        rbAffectedObject.AddForce(transform.forward * 1000);
        //affectedObject.transform.SetParent(null);
        hasObject = false;
        affectedObject = null;
        rbAffectedObject = null;
        attractObjectParticles.SetActive(false);
        hasObjectParticles.SetActive(false);
        fireObjectParticles.Play();
        
    }
    #endregion


    #region WindPowers
    private void WindUpdate()
    {
        Vector3 velocity = controllerPose.GetVelocity();
        //Debug.Log(velocity.magnitude);
        if (/*CheckForceLimit(velocity, 1.0f,false)*/ velocity.magnitude > 3 && !hasAccelerated)//Toggle Werkt nog voor geen reet.
        {
            Debug.Log("Wind - Fast");
            SelectObjects();
            hasAccelerated = true;
        }
        else if (/*CheckForceLimit(velocity, 0.2f,true) */ velocity.magnitude < 1 && hasAccelerated)
        {
            Debug.Log("Wind - Slow");
            DeselectObjects();
            hasAccelerated = false;
        }

        if (hasAccelerated)
        {
            //Debug.Log("WindCast");
            CastWind(velocity);
        }
    }

    private void DeselectObjects()
    {
        objectAffector.selectedObjects.Clear();
    }

    private void SelectObjects()
    {
        objectAffector.selectedObjects = objectAffector.affectedObjects.ToList();
    }

    private bool CheckForceLimit(Vector3 force, float limit, bool lower)
    {
        if(lower)
            return Mathf.Abs(force.x) < limit || Mathf.Abs(force.y) < limit || Mathf.Abs(force.z) < limit;
        else
            return Mathf.Abs(force.x) > limit || Mathf.Abs(force.y) > limit || Mathf.Abs(force.z) > limit;
    }

    private void CastWind(Vector3 velocity)
    {
        if (objectAffector.selectedObjects.Any())
        {
            Vector3 force = velocity * forceVariable;

            foreach (GameObject moveableObject in objectAffector.selectedObjects)
            {
                moveableObject.GetComponent<Rigidbody>().AddForce(force);
            }
        }
    }
    #endregion

    private void CastCrows(Vector3 velocity)
    {
        Vector3 force = (velocity * forceVariable * 10) + (crowSpawnPosition.transform.forward * forceVariable * 10); //+ (controllerPose.transform.forward * forceVariable);

        for (int i = -1; i < 1; i++)//on x axis for left/right, y for up/down
        {
            GameObject crow = Instantiate(crowPrefab, controllerPose.transform); //Position is fucked up, prefab met startpositie.
            crow.transform.localPosition = new Vector3(controllerPose.transform.localPosition.x + (i * 0.3f), controllerPose.transform.localPosition.y, controllerPose.transform.localPosition.z); ;
            crow.transform.SetParent(null);
            crow.GetComponent<Rigidbody>().AddForce(force);
            Destroy(crow, (1 + UnityEngine.Random.value * 3));
        }
    }

    



}
