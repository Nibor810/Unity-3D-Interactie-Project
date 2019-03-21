using System.Collections.Generic;
using UnityEngine;

public class ObjectAffectorScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Grabable"))
            if(!AffectableObjectsManager.affectedObjects.Contains(other.gameObject))
                AffectableObjectsManager.affectedObjects.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabable"))
            if (AffectableObjectsManager.affectedObjects.Contains(other.gameObject))
                AffectableObjectsManager.affectedObjects.Remove(other.gameObject);
    }
}
