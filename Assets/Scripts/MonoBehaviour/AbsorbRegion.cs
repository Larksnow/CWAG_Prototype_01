using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbRegion : MonoBehaviour
{
    public Transform fixedPosition; // Reference to the fixed position child
    public float absorbSpeed = 5f; // Speed at which objects are sucked in

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has a Rigidbody
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Start absorbing the object
            StartCoroutine(AbsorbObject(rb));
        }
    }
    
    private IEnumerator AbsorbObject(Rigidbody obj)
    {
        // Temporarily disable physics
        obj.isKinematic = true;

        // Gradually move the object toward the fixed position
        while (Vector3.Distance(obj.transform.position, fixedPosition.position) > 0.1f)
        {
            obj.transform.position = Vector3.Lerp(obj.transform.position, fixedPosition.position, absorbSpeed * Time.deltaTime);
            yield return null;
        }

        // Snap to the fixed position
        obj.transform.position = fixedPosition.position;

        // Parent the object to the fixed position
        obj.transform.SetParent(transform.parent, true);
        GunController.main.holded = obj.gameObject;
    }
}
