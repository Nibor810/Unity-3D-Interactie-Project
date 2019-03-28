using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PowerSelectorScript : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean gripAction;
    public SteamVR_Action_Vector2 touchPadPosition;

    //public WindForceScript magicScript;

    // Update is called once per frame
    void Update()
    {
        if (gripAction.GetState(handType))
        {
            Debug.Log(touchPadPosition.GetAxis(handType));
            if (touchPadPosition.axis.x < 0 && touchPadPosition.axis.y < 0)
            {
                Debug.Log("1");
            }
            else if (touchPadPosition.axis.x > 0 && touchPadPosition.axis.y < 0)
            {
                Debug.Log("2");
            }
            else if (touchPadPosition.axis.x > 0 && touchPadPosition.axis.y > 0)
            {
                Debug.Log("3");
            }
            else if (touchPadPosition.axis.x < 0 && touchPadPosition.axis.y > 0)
            {
                Debug.Log("4");
            }
            else
            {
                Debug.Log("Else");
            }
        }
    }
}
