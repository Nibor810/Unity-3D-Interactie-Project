using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class WindForceScript : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean selectAction;
    public SteamVR_Action_Boolean deselectAction;

    public float forceLimit = 3.0f;
    public float forceVariable = 20.0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = controllerPose.GetVelocity();
        if (selectAction.GetStateDown(handType))
        {
            Debug.Log("Pressed");
            SelectObjects();
        }

        if (selectAction.GetStateUp(handType))
        {
            Debug.Log("Released");
            DeselectObjects();
        }

        if (selectAction.GetState(handType))
        {
            if (IsOverForceLimit(velocity))
            {
                Debug.Log("Velocity: " + velocity);
                CastWind(velocity);
            }
        }
        
    }

    private void DeselectObjects()
    {
        AffectableObjectsManager.selectedObjects = new List<GameObject>();
    }

    private void SelectObjects()
    {
        AffectableObjectsManager.selectedObjects = new List<GameObject>(AffectableObjectsManager.affectedObjects); 
    }

    private bool IsOverForceLimit(Vector3 force)
    {
        return Mathf.Abs(force.x) > forceLimit || Mathf.Abs(force.y) > forceLimit || Mathf.Abs(force.z) > forceLimit;
    }

    private void CastWind(Vector3 velocity)
    {
        if (AffectableObjectsManager.selectedObjects.Count > 0)
        {
            Vector3 force = velocity * forceVariable;

            foreach (GameObject moveableObject in AffectableObjectsManager.selectedObjects)
            {
                moveableObject.GetComponent<Rigidbody>().AddForce(force);
            }
        }
    }
}
