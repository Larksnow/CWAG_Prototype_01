using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IGrabbable : MonoBehaviour
{

    private Transform prevParent;
    private Rigidbody rb;

    public float pushForce = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// This function is called when the object is grabbed. Should move the object to the player's hand.
    /// Requirements: The object should have a rigidbody.
    /// </summary>
    /// <param name="position">The position of the player's hand</param>
    public void OnGrab(Transform grabHand)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Move the object to the player's hand
        // transform.position = grabHand.position;
        // Trigger a DOTween to move the object to the player's hand
        Vector3 direction = grabHand.position - transform.position;
        rb.AddForce(-direction.normalized * pushForce, ForceMode.Impulse);

        // Call MoveTo in 1 second using Coroutine
        StartCoroutine(MoveToCoroutine(grabHand));
    }

    private IEnumerator MoveToCoroutine(Transform grabHand)
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Moving to hand");
        rb.velocity = Vector3.zero;
        transform.DOMove(grabHand.position, 0.2f).SetEase(Ease.OutSine);
        prevParent = transform.parent;
        transform.SetParent(grabHand);
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
