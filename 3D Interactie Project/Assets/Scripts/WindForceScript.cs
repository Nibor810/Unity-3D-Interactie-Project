using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.Linq;

public enum Power
{
    Wind, Crow
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

    private void CrowUpdate()
    {
        Vector3 velocity = controllerPose.GetVelocity();
        if (selectAction.GetStateDown(handType))
        {
            //Debug.Log("Pressed");
            SelectObjects();
        }

        if (selectAction.GetStateUp(handType))
        {
            //Debug.Log("Released");
            DeselectObjects();
        }

        if (selectAction.GetState(handType))
        {
            if (CheckForceLimit(velocity, forceLimit))
            {
                //Debug.Log("Velocity: " + velocity);
                CastWind(velocity);
                //CastCrows(velocity);
            }
        }
    }

    private void WindUpdate()
    {
        Vector3 velocity = controllerPose.GetVelocity();
        if (CheckForceLimit(velocity, 1.0f) && !hasAccelerated)
        {
            //Debug.Log("Start");
            SelectObjects();
            hasAccelerated = true;
        }
        else if (CheckForceLimit(velocity, 0.2f) && hasAccelerated)
        {
            //Debug.Log("Slow");
            DeselectObjects();
            hasAccelerated = false;
        }

        if (hasAccelerated)
        {
            //Debug.Log("accelerated");
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

    private bool CheckForceLimit(Vector3 force, float limit)
    {
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
        Vector3 force = (velocity * forceVariable); // + (controllerPose.transform.forward * forceVariable);

        for (int i = 0; i < 1; i++)
        {
            GameObject crow = Instantiate(crowPrefab, crowSpawnPosition.position + crowPositionOffset, Quaternion.Euler(crowSpawnPosition.eulerAngles + crowRotationOffset)); //Position is fucked up, prefab met startpositie.
            crow.GetComponent<Rigidbody>().AddForce(force);
            Destroy(crow, crowLifeTime);
        }
    }



}
