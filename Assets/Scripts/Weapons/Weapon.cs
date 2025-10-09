using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static IInteractable;

[DefaultExecutionOrder(-1)]
public class Weapon : MonoBehaviour, IInteractable
{
    [field: SerializeField, Header("Weapon Stats Properties")]
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

    /// <summary>
    /// For exclusive use of the charge fire mode, time to charge for shot to fire.
    /// </summary>
    public float ChargeTime
    { get; set; } = 1.0f;

    /// <summary>
    /// The point and direction the projectiles are emitted from.
    /// </summary>
    [field: SerializeField]
    public Transform FirePosition
    { get; private set; }

    /// <summary>
    /// Read from the input system, tracks if the trigger is being held.
    /// </summary>
    public bool IsTriggerHeld
    { get; set; }

    /// <summary>
    /// Flag to track if the weapon is currently waiting until it can fire again.
    /// </summary>
    public bool WeaponCooldown
    { get; set; }

    /// <summary>
    /// The internal timer counting down how long to wait between shots to maintain desired firerate.
    /// </summary>
    public float CoolDownTimer
    { get; set; }

    /// <summary>
    /// Holds reference to the routine for allow full-auto fire.
    /// </summary>
    public Coroutine AutoFire
    { get; set; }

    /// <summary>
    /// Holds reference to the routine for allow charging fire.
    /// </summary>
    public Coroutine ChargeFire
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

        // Null checking for the pooling parent transform.
        if (ProjectilePoolingParent == null)
        {
            Debug.Log("Projectile Pooling parent not assigned so defaulted to the weapon transform itself.");
            ProjectilePoolingParent = transform;
        }

        // Null checking for the FirePosition transform.
        if (FirePosition == null)
        {
            Debug.Log("Fire Position transform not assigned so defaulted to the weapon transform itself.");
            FirePosition = transform;
        }

        // Init of the stats from the SO reference.
        SetWeaponStats();

        // Set up the listeners for the SO to allow changes to be pushed to runtime instances that are using the SO data.
        Stats.UpdateLinkedWeapons += SetWeaponStats;

        // Sets up the object pooling.
        InstantiateProjectilesForPooling(ProjectilePoolingParent, AmmoClipSize * 3);

        // Initialise the interal cooldown time for firerate management.
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

    /// <summary>
    /// Called on input down for the fire input, ensures weapon fire according to it mode of fire.
    /// </summary>
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
            if (WeaponCooldown == false)
            {
                ChargeFire = StartCoroutine(ChargingFire());
            }
        }
    }

    /// <summary>
    /// To be called on input down for the fire input.Cancels the firing status of the weapon.
    /// </summary>
    public void FireReleased()
    {
        IsTriggerHeld = false;

        if (AutoFire != null)
        {
            StopCoroutine(AutoFire);
            AutoFire = null;
        }
    }

    /// <summary>
    /// For Full Auto fire, ensures firerate is managed every frame when trigger is held down.
    /// </summary>
    /// <returns></returns>
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

    public IEnumerator ChargingFire()
    {
        float chargeTimer = ChargeTime;

        while (IsTriggerHeld)
        {
            if (chargeTimer > 0)
            {
                chargeTimer -= Time.deltaTime;
            }

            if (chargeTimer <= 0)
            {
                FireProjectile();
                break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Accesses the object pooling and select an inactive projectile to activate and fire. Failsafe ensures new objects are instantiated if all objects are active in the pool.
    /// </summary>
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

        // After a projectile is fired, ensure the weaponcooldown flag is set to allow countdown for next chance to fire to start.
        WeaponCooldown = true;
    }

    /// <summary>
    /// Binds the firing methods to the attack inputs.
    /// </summary>
    /// <param name="playerInputs"></param>
    public void BindInputs(PlayerInputHandler playerInputs)
    {
        playerInputs.AttackPress += FirePressed;
        playerInputs.AttackRelease += FireReleased;
    }

    /// <summary>
    /// Un-Binds the firing methods from the attack inputs.
    /// </summary>
    /// <param name="playerInputs"></param>
    public void RemoveInputs(PlayerInputHandler playerInputs)
    {
        playerInputs.AttackPress -= FirePressed;
        playerInputs.AttackRelease -= FireReleased;

        // Helps prevent issues with weapon on being dropped while firing, and being picked up again.
        FireReleased();
    }

    /// <summary>
    /// Initialise the stats from the scriptable object reference.
    /// </summary>
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
        ChargeTime = Stats.ChargeTime;
    }

    /// <summary>
    /// Creates and sets up the intial state for the projectiles that are to be used by the weapon.
    /// </summary>
    /// <param name="parentForPooling"></param>
    /// <param name="poolingAmount"></param>
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
