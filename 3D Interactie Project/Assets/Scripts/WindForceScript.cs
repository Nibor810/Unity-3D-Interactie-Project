using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class WindForceScript : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean teleportAction;

    public float forceLimit = 3.0f;
    public float forceVariable = 20.0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = controllerPose.GetVelocity();

        if(IsOverForceLimit(velocity))
        {
            Debug.Log("Harde LUL"+ velocity);
            CastWind2(velocity);
        }
        
    }

    private bool IsOverForceLimit(Vector3 force)
    {
        return Mathf.Abs(force.x) > forceLimit || Mathf.Abs(force.y) > forceLimit || Mathf.Abs(force.z) > forceLimit;
    }

    private void CastWind()
    {
        Vector3 force = transform.forward * 50;
        
        foreach (GameObject moveableObject in AffectableObjectsManager.affectedObjects)
        {
            moveableObject.GetComponent<Rigidbody>().AddForce(force);
        }
    }

    private void CastWind2(Vector3 velocity)
    {
        Vector3 force = velocity * forceVariable;

        foreach (GameObject moveableObject in AffectableObjectsManager.affectedObjects)
        {
            moveableObject.GetComponent<Rigidbody>().AddForce(force);
        }
    }
}
