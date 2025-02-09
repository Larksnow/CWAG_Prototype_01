using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GunController : MonoBehaviour
{
    public static GunController main;

    public RectTransform crosshair;
    public GameObject gunModel;
    public float distance;
    // private Joycon currentJoycon;
    public Transform gunModelParentLeft;
    public Transform gunModelParentRight;
    public Transform gunModelParent;
    public Transform grabHand;
    public int ammo;


    public bool inCooldown;
    public bool isReloading;
    public bool isFull = true;
    private bool isGrabbing = false;
    public float gunTrailTime = 0.1f;

    public int totalShotsFired;

    // MuzzleFlash muzzleFlash;
    Animator animator;
    LineRenderer gunTrail;

    Vector3 rayDirection;

    public LayerMask layerMask;

    // Used as a callback for all menu items.
    [System.NonSerialized] public UnityEvent menuClickEvent;

    // SoundVariationPlayer variationPlayer;
    [SerializeField] AudioSource reloadAudio;

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
    void Update()
    {
        UpdateGunModelRotation();
    }

    public void InitialGun()
    {
    }

    // Triggered anytime the shoot input is pressed down.
    // Ignores when it is held.
    public void OnShootDown()
    {
        // Get the point that the raycast is looking at
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, rayDirection, out hit, Mathf.Infinity, layerMask))
        {
            if (grabHand.childCount > 0)
            {
                // Get the child
                Transform child = grabHand.GetChild(0);
                // Get the IGrabbable component
                IGrabbable grabbable = child.GetComponent<IGrabbable>();
                grabbable.OnRelease(hit.point);

            } else {
                IGrabbable grabbable = hit.collider.GetComponent<IGrabbable>();
                if (grabbable != null)
                {
                    grabbable.OnGrab(grabHand);
                }

                Debug.Log("Hit: " + hit.collider.name);

            }
        }
        else
        {
        }
    }

    // Triggered anytime the shoot input is pressed or held.
    public void Shoot()
    {
        if (inCooldown) return;
        if (isReloading) return;

        if (ammo <= 0)
        {
            return;
        }
        ammo--;

        totalShotsFired++;
        isFull = false;


        StartCoroutine(Cooldown());
    }

    void FireSingleBullet()
    {
        Vector3 principalDirection = rayDirection;
        principalDirection.Normalize();

        float spread = 0; // CWAG.main.currentGun.spread;
        float angle = Random.Range(-spread / 2f, spread / 2f);
        Vector3 randomAxis = Vector3.Cross(principalDirection, Random.onUnitSphere).normalized;

        Quaternion rotation = Quaternion.AngleAxis(angle, randomAxis);
        Vector3 shiftedDirection = rotation * principalDirection;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, shiftedDirection, out hit, Mathf.Infinity, layerMask))
        {
            ShootEffect(hit);
            // ShowGunTrail(hit.point);
            if (hit.collider.tag == "Level")
            {
                // BulletHoleManager.main.CreateBulletHole(hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
            }
            if (hit.collider.tag == "ValidTarget")
            {
                // TimeManager.main.validShot++;
            }
        }
        else
        {
            // if didn't hit anything, show the trail to a point far in the distance
            // ShowGunTrail(Camera.main.transform.position + rayDirection * 1000f);
        }

    }

    private IEnumerator FadeGunTrail()
    {
        float elapsedTime = 0f;
        float duration = gunTrailTime;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            gunTrail.startColor = new Color(gunTrail.startColor.r, gunTrail.startColor.g, gunTrail.startColor.b, alpha);
            gunTrail.endColor = new Color(gunTrail.endColor.r, gunTrail.endColor.g, gunTrail.endColor.b, alpha);
            yield return null;
        }

        gunTrail.positionCount = 0;
    }

    void ShootEffect(RaycastHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null) return;

        Vector3 knockbackForce = 100 * Mathf.Abs(Vector3.Dot(hit.normal, rayDirection)) * -hit.normal;

        rb.AddForce(knockbackForce);

        AudioSource audioSource = rb.GetComponent<AudioSource>();
        
        // SoundVariationPlayer variationPlayer = rb.GetComponent<SoundVariationPlayer>();
        // if (variationPlayer) // If the object has variation SFX, random play
        //     variationPlayer.PlayRandomClip();
        // else if (audioSource) // If the object has only one SFX, directly play
        //     audioSource.Play();

        IShootable shootable = rb.GetComponent<IShootable>();
        if (shootable != null)
            shootable.TakeShot(knockbackForce);
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

    IEnumerator Cooldown()
    {
        inCooldown = true;
        yield return new WaitForSeconds(1f / 0.2f);
        inCooldown = false;
    }
}