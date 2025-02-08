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
    public int ammo;


    public bool inCooldown;
    public bool isReloading;
    public bool isFull = true;
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

    void Start()
    {
        // currentJoycon = JoyconController.main.GetJoycon();
        // if(currentJoycon != null)
        //     gunModelParent = currentJoycon.isLeft ? gunModelParentLeft : gunModelParentRight;
        // InitialGun();
        // variationPlayer = GetComponent<SoundVariationPlayer>();
        // if (gunTrail != null)
        // {
        //     gunTrail.positionCount = 0;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGunModelRotation();
    }

    public void InitialGun()
    {
        // ammo = CWAG.main.currentGun.ammoCapacity;

        // if (gunModel) Destroy(gunModel.gameObject);

        // GameObject gunObj = Instantiate(CWAG.main.currentGun.prefab, gunModelParent);
        // gunModel = gunObj.GetComponent<GunModel>();

        // animator = gunModel.animator;
        // muzzleFlash = gunModel.muzzleFlash;
        // gunTrail = gunModel.lineRenderer;
    }

    // Triggered anytime the shoot input is pressed down.
    // Ignores when it is held.
    public void OnShootDown()
    {
        // if (CWAG.main != null && CWAG.main.isInMenu())
        // {
        //     ShootSFX();
        //     menuClickEvent.Invoke();
        // }
        // else
        // {

        //     // This call is for the End of day continue button, do not remove unless you can cover that case!
        //     menuClickEvent.Invoke();

        //     Shoot();
        // }
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
        // CameraController.main.Shake(
        //     CWAG.main.currentGun.screenShakeDuration,
        //     CWAG.main.currentGun.screenShakeMagnitude);
        // ShootSFX();
        // animator.SetTrigger("PlayShoot");
        // muzzleFlash.Flash();

        // // Call rumble for joycon
        // if (JoyconController.main != null)
        // {
        //     JoyconController.main.GetJoycon().SetRumble(160, 320, CWAG.main.currentGun.rumbleMagnitude, CWAG.main.currentGun.rumbleTime);
        // }

        // // Most of the time this is 1, but for shotguns it will fire multiple per shot
        // for (int i = 0; i < CWAG.main.currentGun.bulletsPerShot; ++i)
        //     FireSingleBullet();

        StartCoroutine(Cooldown());

        // Level two Alert veggies
        // if (RecipeManager.main && RecipeManager.main is SoupManager)
        // {
        //     ((SoupManager)RecipeManager.main).SetAlert();
        // }
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

    // private void ShowGunTrail(Vector3 hitPoint)
    // {
    //     if (gunTrail == null) return;

    //     StartCoroutine(AnimateGunTrail(hitPoint));
    // }

    // private IEnumerator AnimateGunTrail(Vector3 hitPoint)
    // {
    //     float trailDuration = 0.1f;
    //     float elapsedTime = 0f;

    //     Vector3 startPosition = muzzleFlash.transform.position;

    //     Color startColor = gunTrail.startColor;
    //     Color endColor = gunTrail.endColor;

    //     while (elapsedTime < trailDuration)
    //     {
    //         elapsedTime += Time.deltaTime;
    //         float progress = elapsedTime / trailDuration;

    //         gunTrail.positionCount = 2;

    //         // Set the bullet trail from muzzle to hit point over time
    //         gunTrail.SetPosition(0, startPosition);
    //         gunTrail.SetPosition(1, Vector3.Lerp(startPosition, hitPoint, progress));

    //         float alpha = Mathf.Lerp(1f, 0f, progress);
    //         gunTrail.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
    //         gunTrail.endColor = new Color(endColor.r, endColor.g, endColor.b, alpha);

    //         yield return null;
    //     }

    //     gunTrail.positionCount = 0;
    // }

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

    // IEnumerator ReloadCoroutine()
    // {
    //     isReloading = true;
    //     // reloadAudio.clip = CWAG.main.currentGun.reloadSfx;
    //     reloadAudio.Play();
    //     animator.SetTrigger("PlayReload");
    //     // yield return new WaitForSeconds(CWAG.main.currentGun.reloadTime);
    //     // ammo = CWAG.main.currentGun.ammoCapacity;
    //     isReloading = false;
    //     isFull = true;
    // }

    // public void Reload()
    // {
    //     if (isReloading) return;
    //     // if (ammo == CWAG.main.currentGun.ammoCapacity) return;

    //     StopCoroutine(ReloadCoroutine());
    //     StartCoroutine(ReloadCoroutine());
    // }

    // void ShootSFX()
    // {
    //     // if (!variationPlayer) return;

    //     // variationPlayer.sfxList = CWAG.main.currentGun.shootSfxList;
    //     // variationPlayer.PlayRandomClip();
    // }

    // public void ResetShotCount()
    // {
    //     totalShotsFired = 0;
    // }
}