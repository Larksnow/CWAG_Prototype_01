using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GunController : MonoBehaviour
{
    public static GunController main;

    public RectTransform crosshair;
    public GameObject gunModel;
    public float distance;
    public Vector3 rayDirection;
    public bool multimode = false;
    [SerializeField] private float force;

    public LayerMask layerMask;
   

    // Used as a callback for all menu items.
    [System.NonSerialized] public UnityEvent menuClickEvent;
    public GameObject holded;
    public GameObject selected;
    public Transform fixedPosition; // Reference to the fixed position child
    public float absorbSpeed = 5f; // Speed at which objects are sucked in

    void Awake()
    {
        if (main) Destroy(gameObject);
        else main = this;

        // Needs to be initialized before UIButton.Start() is called
        if (menuClickEvent == null)
        {
            menuClickEvent = new UnityEvent();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateGunModelRotation();
        SelectTarget();
    }

    public void UpdateCrosshairPostiton(Vector2 screenPosition)
    {
        crosshair.anchoredPosition = screenPosition;
    }


    public Vector2 GetCrosshairPosition()
    {
        return crosshair.anchoredPosition;
    }

    void UpdateGunModelRotation()
    {
        float scale = Screen.width / 1920f;

        Vector2 scaledCrosshairCoords =
            new Vector2(crosshair.anchoredPosition.x * scale + Screen.width / 2,
                        crosshair.anchoredPosition.y * scale + Screen.height / 2);

        Ray ray = Camera.main.ScreenPointToRay(scaledCrosshairCoords);
        rayDirection = ray.direction;

        Vector3 target = ray.direction * distance + Camera.main.transform.position;

        Vector3 targetDirection = gunModel.transform.position - target;
        Vector3 newDirection = Vector3.RotateTowards(gunModel.transform.forward, targetDirection, Mathf.PI, 0);
        gunModel.transform.rotation = Quaternion.LookRotation(newDirection);
    }

    void SelectTarget()
    {
        if (holded != null || multimode) return;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, rayDirection, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.tag == "Moveable")
            {
                // Check if the hit object implements ISelectable
                GameObject selectable = hit.collider.gameObject;
                if (selectable != null)
                {
                    // If it's a new object, disable the outline of the previous one
                    if (selected != selectable)
                    {
                        DeselectCurrent();
                        selected = selectable;
                        selected.GetComponent<ISelectable>().BeingSelect(); // Highlight the new object
                    }
                }
                else
                {
                    // If the raycast doesn't hit a selectable object
                    DeselectCurrent();
                }
            }
            else
            {
                // If the raycast doesn't hit anything
                DeselectCurrent();
            } 
        }
    }

    private void DeselectCurrent()
    {
        if (selected != null)
        {
            // Turn off the outline of the previously selected object
            Sphere sphere = selected.GetComponent<ISelectable>() as Sphere;
            if (sphere != null)
            {
                sphere.outline.enabled = false;
                sphere.GetComponent<Rigidbody>().useGravity = true;
            }
            selected = null;
        }
    }

    public void ShootHoldedObj()
    {
        if (holded == null) return;
        holded.GetComponent<Rigidbody>().isKinematic = false;
        holded.transform.SetParent(null);
        holded.GetComponent<Rigidbody>().AddForce(rayDirection * force, ForceMode.Force);
        holded = null;
    }


    public void StartAbsorbObjectCoroutine()
    {
        if (holded != null) return;
        if (selected == null) return;
        StartCoroutine(AbsorbObject());
    }
    private IEnumerator AbsorbObject()
    {
        Rigidbody obj = selected.GetComponent<Rigidbody>();
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
        obj.transform.SetParent(gunModel.transform, true);
        holded = obj.gameObject;
    }
}
