using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR;

public class ActionTest : MonoBehaviour
{

    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean teleportAction;
    public SteamVR_Action_Boolean grabAction;

    void Update()
    {
        if (GetTeleportDown())
        {
            Debug.Log("Teleport " + handType);
        }

        if (GetGrab())
        {
            Debug.Log("Grab " + handType);
        }
    }


    public bool GetTeleportDown()
    {
        return teleportAction.GetStateDown(handType);
    }

    public bool GetGrab()
    {
        return grabAction.GetState(handType);
    }
}
