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

    public GameObject crowParticles;
    public GameObject windParticles;
    public GameObject kineticParticles;

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
                crowParticles.SetActive(true);
                windParticles.SetActive(false);
                kineticParticles.SetActive(false);
            }
            else if (touchPadPosition.axis.x > 0 && touchPadPosition.axis.y < 0)
            {
                Debug.Log("2");//Rechts Onder
                magicScript.SetPower(Power.Wind);
                crowParticles.SetActive(false);
                windParticles.SetActive(true);
                kineticParticles.SetActive(false);
            }
            else if (touchPadPosition.axis.x > 0 && touchPadPosition.axis.y > 0)
            {
                Debug.Log("3");//Rechts Boven
                magicScript.SetPower(Power.Telekinetic);
                crowParticles.SetActive(false);
                windParticles.SetActive(false);
                kineticParticles.SetActive(true);
            }
            else if (touchPadPosition.axis.x < 0 && touchPadPosition.axis.y > 0)
            {
                Debug.Log("4"); //Links Boven
                magicScript.SetPower(Power.None);
                crowParticles.SetActive(false);
                windParticles.SetActive(false);
                kineticParticles.SetActive(false);
            }
            else
            {
                Debug.Log("Else");
            }
        }
    }
}
