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
    public GameObject targetPointer;

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
    }

    //Needs Work
    private void CrowUpdate()
    {
        Vector3 velocity = controllerPose.GetVelocity();
        if (selectAction.GetState(handType))
        {
            if (CheckForceLimit(velocity, forceLimit))
            {
                CastCrows(velocity);
                hasAccelerated = true;
            }

            if (CheckForceLimit(velocity, 0.2f) && hasAccelerated)
            {
                RaycastHit hit;
                if (Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100))
                {
                    ActivateCrows(hit.point);
                }
                else
                {
                    DeactivateCrows();
                }
                hasAccelerated = false;
            }
        }
    }

   



    //Functional
    private void WindUpdate()
    {
        Vector3 velocity = controllerPose.GetVelocity();
        if (CheckForceLimit(velocity, 1.0f) && !hasAccelerated)
        {
            SelectObjects();
            hasAccelerated = true;
        }
        else if (CheckForceLimit(velocity, 0.2f) && hasAccelerated)
        {
            DeselectObjects();
            hasAccelerated = false;
        }

        if (hasAccelerated)
        {
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

        for (int i = -3; i < 3; i++)//on x axis for left/right, y for up/down
        {
            Vector3 pos = new Vector3(crowSpawnPosition.position.x+(i*0.3f), crowSpawnPosition.position.y, crowSpawnPosition.position.z);
            Quaternion rot = Quaternion.Euler(crowSpawnPosition.eulerAngles + crowRotationOffset);
            GameObject crow = Instantiate(crowPrefab, pos, rot); //Position is fucked up, prefab met startpositie.
            crow.GetComponent<Rigidbody>().AddForce(force);
            spawnedGameObjects.Add(crow);
            //Destroy(crow, crowLifeTime);
        }
    }


    private void ActivateCrows(Vector3 target)
    {
        Debug.Log(target);
        targetPointer.transform.position = target;
        foreach (GameObject crow in spawnedGameObjects)
        {
            crow.GetComponent<CrowScript>().Activate(target);
        }
        spawnedGameObjects.Clear();
    }

    private void DeactivateCrows()
    {
        foreach (GameObject crow in spawnedGameObjects)
        {
            Destroy(crow);
        }
        spawnedGameObjects.Clear();
    }


    private void CastTelekinesis(Vector3 velocity)
    {
        Vector3 force = (velocity * forceVariable);




    }



}
