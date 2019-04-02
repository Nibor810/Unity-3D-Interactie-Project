using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.Linq;

public enum Power
{
    None, Wind, Crow
}


public class WindForceScript : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean selectAction;
    public SteamVR_Action_Boolean deselectAction;
    public GameObject crowPrefab;

    public float forceLimit = 3.0f;
    public float forceVariable = 20.0f;
    public int crowLifeTime = 5;
    public Vector3 crowPositionOffset;
    public Vector3 crowRotationOffset;
    public Transform crowSpawnPosition;

    public Power selectedPower;
    public GameObject targetPointer;
    public LayerMask mask;

    private List<GameObject> spawnedGameObjects = new List<GameObject>();

    public bool hasAccelerated = false;

    // Update is called once per frame
    void Update()
    {
        switch (selectedPower)
        {
            case Power.Wind: WindUpdate();  break;
            case Power.Crow: CrowUpdate(); break;

        }
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
        AffectableObjectsManager.selectedObjects.Clear();
    }

    private void SelectObjects()
    {
        AffectableObjectsManager.selectedObjects = AffectableObjectsManager.affectedObjects.ToList();
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
        if (AffectableObjectsManager.selectedObjects.Any())
        {
            Vector3 force = velocity * forceVariable;

            foreach (GameObject moveableObject in AffectableObjectsManager.selectedObjects)
            {
                moveableObject.GetComponent<Rigidbody>().AddForce(force);
            }
        }
    }

    private void CastCrows(Vector3 velocity)
    {
        Vector3 force = (velocity * forceVariable * 10) + (crowSpawnPosition.transform.forward * forceVariable * 10); //+ (controllerPose.transform.forward * forceVariable);

        for (int i = -1; i < 1; i++)//on x axis for left/right, y for up/down
        {
            GameObject crow = Instantiate(crowPrefab, controllerPose.transform); //Position is fucked up, prefab met startpositie.
            crow.transform.localPosition = new Vector3(controllerPose.transform.localPosition.x + (i * 0.3f), controllerPose.transform.localPosition.y, controllerPose.transform.localPosition.z); ;
            crow.transform.SetParent(null);
            crow.GetComponent<Rigidbody>().AddForce(force);
            Destroy(crow, (1 + Random.value * 3));
        }
    }

    private void CastTelekinesis(Vector3 velocity)
    {
        Vector3 force = (velocity * forceVariable);




    }



}
