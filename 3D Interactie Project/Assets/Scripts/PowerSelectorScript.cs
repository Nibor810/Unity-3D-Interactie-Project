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
    public GameObject particleC;

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
                particleC.SetActive(false);
            }
            else if (touchPadPosition.axis.x > 0 && touchPadPosition.axis.y < 0)
            {
                Debug.Log("2");//Rechts Onder
                magicScript.SetPower(Power.Wind);
                particleA.SetActive(false);
                particleB.SetActive(true);
                particleC.SetActive(false);
            }
            else if (touchPadPosition.axis.x > 0 && touchPadPosition.axis.y > 0)
            {
                Debug.Log("3");//Rechts Boven
                magicScript.SetPower(Power.Telekinetic);
                particleA.SetActive(false);
                particleB.SetActive(false);
                particleC.SetActive(true);
            }
            else if (touchPadPosition.axis.x < 0 && touchPadPosition.axis.y > 0)
            {
                Debug.Log("4"); //Links Boven
                magicScript.SetPower(Power.None);
                particleA.SetActive(false);
                particleB.SetActive(false);
                particleC.SetActive(false);
            }
            else
            {
                Debug.Log("Else");
            }
        }
    }
}
