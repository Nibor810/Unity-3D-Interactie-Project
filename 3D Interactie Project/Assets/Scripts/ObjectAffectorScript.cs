using System.Collections.Generic;
using UnityEngine;

public class ObjectAffectorScript : MonoBehaviour
{
    public List<GameObject> affectedObjects = new List<GameObject>();
    public List<GameObject> selectedObjects = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Grabable") && !affectedObjects.Contains(other.gameObject))
            affectedObjects.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabable") && affectedObjects.Contains(other.gameObject))
            affectedObjects.Remove(other.gameObject);
    }
}
