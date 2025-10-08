using UnityEngine;
using System.Collections.Generic;
using static IInteractable;
using System.Collections;

[DefaultExecutionOrder(-1)]
public class Weapon : MonoBehaviour, IInteractable
{
    [field: SerializeField, Header("Weapon")]
    public WeaponStatsSO Stats
    { get; set; }

    public float FireRatePerSecond
    { get; private set; } = 0.1f;

    public float ReloadTime
    { get; private set; } = 1.0f;

    public int AmmoClipSize
    { get; private set; } = 30;

    public FireMode FireModeState
    { get; private set; }

    [field: SerializeField]
    public Transform FirePosition
    { get; private set; }

    public bool IsTriggerHeld
    { get; set; }

    public bool WeaponCooldown
    { get; set; }

    public float CoolDownTimer
    { get; set; }

    public Coroutine AutoFire
    { get; set; }


    [field: SerializeField, Header("Projectile & Pooling")]
    public GameObject ProjectilePrefab
    { get; set; }

    public List<Projectile> ProjectilePooling
    { get; private set; } = new List<Projectile>();

    [field: SerializeField]
    public Transform ProjectilePoolingParent
    { get; set; }


    [field: SerializeField, Header("Interaction")]
    public float InteractionDistance
    { get; set; }

    [field: SerializeField]
    public bool IsInterationAllowed
    { get; set; }

    [field: SerializeField]
    public InteractionCentrePoint InteractionCentre
    { get; set; }


    void Awake()
    {
        IsInterationAllowed = true;

        if (ProjectilePoolingParent == null)
        {
            Debug.Log("Projectile Pooling parent not assigned so defaulted to the weapon transform itself.");
            ProjectilePoolingParent = transform;
        }

        if (FirePosition == null)
        {
            Debug.Log("Fire Position transform not assigned so defaulted to the weapon transform itself.");
            FirePosition = transform;
        }

        SetWeaponStats();

        Stats.UpdateLinkedWeapons += SetWeaponStats;

        InstantiateProjectilesForPooling(ProjectilePoolingParent, AmmoClipSize * 3);

        CoolDownTimer = FireRatePerSecond;
    }

    private void OnDestroy()
    {
        if (Stats != null)
        {
            Stats.UpdateLinkedWeapons -= SetWeaponStats;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // During weapon cooldown, count down the cooldown timer.
        if (WeaponCooldown == true)
        {
            // Minis the time from last frame to this frame.
            if (CoolDownTimer > 0)
            {
                CoolDownTimer -= Time.deltaTime;
            }

            // If the timer has hit or gone below zero, reset the timer and end cooldown flag.
            if (CoolDownTimer <= 0)
            {
                CoolDownTimer = FireRatePerSecond;

                WeaponCooldown = false;
            }
        }
    }

    public void FirePressed()
    {
        IsTriggerHeld = true;

        if (FireModeState == FireMode.SemiAuto)
        {
            if (WeaponCooldown == false)
            {
                FireProjectile();
            }
        }
        else if (FireModeState == FireMode.FullAuto)
        {
            AutoFire = StartCoroutine(StartAutoFire());
        }
        else if (FireModeState == FireMode.Burst)
        {

        }
        else if (FireModeState == FireMode.Charge)
        {

        }
    }

    public void FireReleased()
    {
        IsTriggerHeld = false;

        if (AutoFire != null)
        {
            StopCoroutine(AutoFire);
            AutoFire = null;
        }
    }

    public IEnumerator StartAutoFire()
    {
        while (IsTriggerHeld)
        {
            if (WeaponCooldown == false)
            {
                FireProjectile();
            }

            yield return null;
        }

    }

    public void FireProjectile()
    {
        for (int i = 0; i < ProjectilePooling.Count; i++)
        {
            if (!ProjectilePooling[i].gameObject.activeSelf)
            {
                ProjectilePooling[i].ActivateLiveProjectile(FirePosition);
                break;
            }
            
            if (i == ProjectilePooling.Count - 1)
            {
                GameObject newProjectile = Instantiate(ProjectilePrefab, FirePosition.position, Quaternion.identity, null);

                if (newProjectile.TryGetComponent<Projectile>(out Projectile projectile))
                {
                    projectile.ObjectPoolingReset += ResetGameObjectForPooling;
                }

                ProjectilePooling.Add(projectile);

                projectile.ActivateLiveProjectile(FirePosition);

                break;
            }
        }

        WeaponCooldown = true;
    }

    public void BindInputs(PlayerInputHandler playerInputs)
    {
        playerInputs.AttackPress += FirePressed;
        playerInputs.AttackRelease += FireReleased;
    }

    public void RemoveInputs(PlayerInputHandler playerInputs)
    {
        playerInputs.AttackPress -= FirePressed;
        playerInputs.AttackRelease -= FireReleased;

        // Helps prevent issues with weapon on being dropped while firing, and being picked up again.
        FireReleased();
    }

    public void SetWeaponStats()
    {
        if (Stats == null)
        {
            Stats = ScriptableObject.CreateInstance<WeaponStatsSO>();
            Debug.LogWarning($"The starting values for {this.name} of type {this.GetType()} were null on awake");
        }

        FireRatePerSecond = Stats.FireRatePerSecond;
        ReloadTime = Stats.ReloadTime;
        AmmoClipSize = Stats.AmmoClipSize;
        FireModeState = Stats.FireModeState;
    }

    public void InstantiateProjectilesForPooling(Transform parentForPooling, int poolingAmount)
    {
        if (ProjectilePrefab == null)
        {
            Debug.LogError("No projectile prefab has been assigned to the weapon.");
            return;
        }

        if (ProjectilePrefab.TryGetComponent<Projectile>(out _))
        {
            for (int i = 0; i < poolingAmount; i++)
            {
                GameObject newProjectile = Instantiate(ProjectilePrefab, parentForPooling.position, Quaternion.identity, parentForPooling);

                if (newProjectile.TryGetComponent<Projectile>(out Projectile projectile))
                {
                    projectile.ObjectPoolingReset += ResetGameObjectForPooling;
                }

                ProjectilePooling.Add(projectile);

                newProjectile.SetActive(false);
            }
        }
        else
        {
            Debug.LogError($"Non-projectile holding object being used as a projectile prefab for this weapon {this.gameObject.name}");
        }
    }

    public void ResetGameObjectForPooling(GameObject projectile)
    {
        projectile.transform.parent = ProjectilePoolingParent.transform;
        projectile.transform.localPosition = Vector3.zero;
    }

    public void Interaction(Interactioner interactioner)
    {
        if (interactioner.Inventory != null)
        {
            interactioner.Inventory.TryAddObjectToHand(this.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (IsInterationAllowed)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        
        Gizmos.DrawWireSphere(transform.position, InteractionDistance);
    }

    private void OnDrawGizmos()
    {
        if (FirePosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(FirePosition.position, 0.025f);
        }
    }
}
