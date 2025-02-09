using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGrabbable : MonoBehaviour
{

    private Transform prevParent;

    /// <summary>
    /// This function is called when the object is grabbed. Should move the object to the player's hand.
    /// Requirements: The object should have a rigidbody.
    /// </summary>
    /// <param name="position">The position of the player's hand</param>
    public void OnGrab(Transform grabHand)
    {
        // Move the object to the player's hand
        transform.position = grabHand.position;
        // Set it as a child of the player's hand
        prevParent = transform.parent;
        transform.SetParent(grabHand);

        // Enable kinetic
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void OnRelease(Vector3 target) {
        // Set the object's parent back to the previous parent
        transform.SetParent(prevParent);
        // Set the object's position to the target
        // Add the object's size to the target
        transform.position = new Vector3(target.x, target.y + transform.localScale.y / 2, target.z);
        // Disable kinetic
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
}
