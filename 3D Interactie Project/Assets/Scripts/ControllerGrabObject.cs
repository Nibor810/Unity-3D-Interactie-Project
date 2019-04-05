using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerGrabObject : MonoBehaviour
{

    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean grabAction;

    private GameObject collidingObject;
    private GameObject objectInHand;


    void Update()
    {
        if (grabAction.GetLastStateDown(handType))
        {
            Debug.Log("GrabObjectA");
            if (collidingObject)
            {
                Debug.Log("GrabObjectB");
                GrabObject();
            }
        }

        if (grabAction.GetLastStateUp(handType))
        {
            Debug.Log("ReleaseObjectA");
            if (objectInHand)
            {
                Debug.Log("ReleaseObjectB");
                ReleaseObject();
            }
        }

    }

    private void SetCollidingObject(Collider col)
    {
        Debug.Log("SetCollidingA");
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        Debug.Log("SetCollidingB");
        collidingObject = col.gameObject;
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        Debug.Log("DeleteCollidingA");
        if (!collidingObject)
        {
            return;
        }
        Debug.Log("DeleteCollidingB");
        collidingObject = null;
    }

    private void GrabObject()
    {
        objectInHand = collidingObject;
        collidingObject = null;
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            objectInHand.GetComponent<Rigidbody>().velocity = controllerPose.GetVelocity();
            objectInHand.GetComponent<Rigidbody>().angularVelocity = controllerPose.GetAngularVelocity();
        }
        objectInHand = null;
    }
}
