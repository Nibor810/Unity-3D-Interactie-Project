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

    public WindForceScript magicScript;

    public GameObject particleA;
    public GameObject particleB;

    // Update is called once per frame
    void Update()
    {
        if (gripAction.GetState(handType))
        {
            Debug.Log(touchPadPosition.GetAxis(handType));
            if (touchPadPosition.axis.x < 0 && touchPadPosition.axis.y < 0)
            {
                Debug.Log("1");//Links Onder
                magicScript.SetPower(Power.Crow);
                particleA.SetActive(true);
                particleB.SetActive(false);
            }
            else if (touchPadPosition.axis.x > 0 && touchPadPosition.axis.y < 0)
            {
                Debug.Log("2");//Rechts Onder
                magicScript.SetPower(Power.Wind);
                particleA.SetActive(false);
                particleB.SetActive(true);
            }
            else if (touchPadPosition.axis.x > 0 && touchPadPosition.axis.y > 0)
            {
                Debug.Log("3");//Rechts Boven
            }
            else if (touchPadPosition.axis.x < 0 && touchPadPosition.axis.y > 0)
            {
                Debug.Log("4"); //Links Boven
            }
            else
            {
                Debug.Log("Else");
            }
        }
    }
}
