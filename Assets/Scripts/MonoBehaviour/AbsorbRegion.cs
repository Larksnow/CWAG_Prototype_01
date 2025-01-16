using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbRegion : MonoBehaviour
{
    public Transform absorbTarget; // Position to which objects are sucked
    public float suchForce = 10f; // Strength of the suction force
    public float pushForce = 10f; 
    [SerializeField] private List<Rigidbody> objectsInRegion = new List<Rigidbody>(); // Track objects with Rigidbody
 
    void OnTriggerEnter(Collider other)
    {
        // Check if the object has a Rigidbody
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !objectsInRegion.Contains(rb))
        {
            objectsInRegion.Add(rb); // Add object to the list
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Remove the object when it exits the trigger
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && objectsInRegion.Contains(rb))
        {
            objectsInRegion.Remove(rb);
        }
    }

    public void ApplySuctionForce()
    {
        foreach (Rigidbody rb in objectsInRegion)
        {
            if (rb != null) // Ensure the object still exists
            {
                // Calculate direction and apply force
                Vector3 direction = (absorbTarget.position - rb.position).normalized;
                rb.useGravity = false;
                rb.AddForce(direction * suchForce, ForceMode.Acceleration);
            }
        }
    }

    public void ApplyPushForce()
    {
        foreach (Rigidbody rb in objectsInRegion)
        {
            if (rb != null) // Ensure the object still exists
            {
                rb.useGravity = true;
                rb.AddForce(GunController.main.rayDirection * pushForce, ForceMode.Acceleration);
            }
        }
    }

}
