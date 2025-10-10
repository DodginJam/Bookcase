using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public WeaponTypeSO WeaponType
    { get; set; }

    /// <summary>
    /// For exclusive use of the charge fire mode, time to charge for shot to fire.
    /// </summary>
    public float ChargeTime
    { get; set; } = 1.0f;

    /// <summary>
    /// For exclusive use of the burst fire mode, number of projectiles in the burst.
    /// </summary>
    public int BurstNumberOfShots
    { get; set; } = 3;

    /// <summary>
    /// For exclusive use of the burst fire mode, time between each shot within the burst.
    /// </summary>
    public float BurstShotFireRate
    { get; set; } = 0.1f;

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

    public Coroutine FireRoutine
    { get; set; }

    public event Action<bool> TriggerPullEvents;

    public event Action<bool> TriggerReleaseEvents;

    public event Action<bool> WeaponShootEvents;


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
    
    void SetUpWeaponListeners()
    {
        WeaponShootEvents += TestTriggerLog;
    }

    void RemoveWeaponListeners()
    {
        WeaponShootEvents -= TestTriggerLog;
    }

    void TestTriggerLog(bool successStatus)
    {
        string message;
        if (successStatus)
        {
            message = "Weapon Shoot Event Fired - Success";
        }
        else
        {
            message = "Weapon Shoot Event Fired - Failed";
        }

        Debug.Log(message);
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
    public void TriggerPressed()
    {
        IsTriggerHeld = true;

        WeaponType.OnTriggerPress(this);
    }

    /// <summary>
    /// To be called on input down for the fire input.Cancels the firing status of the weapon.
    /// </summary>
    public void TriggerReleased()
    {
        IsTriggerHeld = false;

        WeaponType.OnTriggerRelease(this);
    }

    /// <summary>
    /// Accesses the object pooling and select an inactive projectile to activate and fire. Failsafe ensures new objects are instantiated if all objects are active in the pool.
    /// </summary>
    public void FireProjectile(bool setCooldown = true)
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

        ShootWeaponEventInvoke(true);

        // After a projectile is fired, ensure the weaponcooldown flag is set to allow countdown for next chance to fire to start.
        WeaponCooldown = setCooldown;
    }

    /// <summary>
    /// Binds the firing methods to the attack inputs.
    /// </summary>
    /// <param name="playerInputs"></param>
    public void BindInputs(PlayerInputHandler playerInputs)
    {
        playerInputs.AttackPress += TriggerPressed;
        playerInputs.AttackRelease += TriggerReleased;
    }

    /// <summary>
    /// Un-Binds the firing methods from the attack inputs.
    /// </summary>
    /// <param name="playerInputs"></param>
    public void RemoveInputs(PlayerInputHandler playerInputs)
    {
        playerInputs.AttackPress -= TriggerPressed;
        playerInputs.AttackRelease -= TriggerReleased;

        // Helps prevent issues with weapon on being dropped while firing, and being picked up again.
        IsTriggerHeld = false;
    }

    public void TriggerPullEventInvoke(bool successfulTriggerPull)
    {
        TriggerPullEvents?.Invoke(successfulTriggerPull);
    }

    public void TriggerReleaseEventInvoke(bool successfulTriggerRelease)
    {
        TriggerReleaseEvents?.Invoke(successfulTriggerRelease);
    }

    public void ShootWeaponEventInvoke(bool successfullShoot)
    {
        WeaponShootEvents?.Invoke(successfullShoot);
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

        RemoveWeaponListeners();
        WeaponType = Stats.WeaponType;
        SetUpWeaponListeners();

        ChargeTime = Stats.ChargeTime;
        BurstNumberOfShots = Stats.BurstNumberOfShots;
        BurstShotFireRate = Stats.BurstShotFireRate;
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
