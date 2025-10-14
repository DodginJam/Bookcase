using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IInteractable;

[DefaultExecutionOrder(-1)]
public class Weapon : MonoBehaviour, IInteractable
{
    [field: SerializeField, Header("Weapon Properties")]
    public WeaponDataSO WeaponData
    { get; set; }

    public float FireRatePerSecond
    { get; private set; } = 0.1f;

    public float ReloadTime
    { get; private set; } = 1.0f;

    public int AmmoClipSize
    { get; private set; } = 30;

    public WeaponBehaviourSO WeaponBehaviour
    { get; private set; }

    public WeaponSoundsSO SoundsSO
    { get; private set; }

    public int CurrentAmmoInClip
    { get; private set; } = 0;

    /// <summary>
    /// For exclusive use of the charge fire mode, time to charge for shot to fire.
    /// </summary>
    public float ChargeTime
    { get; set; } = 1.0f;

    public float ChargeTimer
    { get; set; } = float.MaxValue;

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
    { get; private set; }

    /// <summary>
    /// Reference to the timer that countdown whether the weapon is eligable to fire the next projectile - assigned via the associated weapon behaviour provided.
    /// </summary>
    public Coroutine FireRoutine
    { get; set; }


    #region Reloading
    /// <summary>
    /// Flag to track if the weapon is currently reloading.
    /// </summary>
    public bool IsReloading
    { get; set; } = false;

    /// <summary>
    /// Falg to track is the weapon is able to be reloaded or is considered busy.
    /// </summary>
    public bool CanReload
    { get; set; }

    /// <summary>
    /// Reference to the coroutine that tracks the reload timing.
    /// </summary>
    public Coroutine ReloadCoroutine
    { get; set; }

    /// <summary>
    /// The interal timer counting down the reloading time where the weapon is unable to fire.
    /// </summary>
    public float ReloadTimer
    { get; set; }
    #endregion


    // Events that allow additonal effects to be triggered via plugging in and out at given moments in the usage of the gun, such as firing off sound effects and VFX.
    #region Events
    public event Action<Weapon> TriggerPullSuccessEvents;
    public event Action<Weapon> TriggerPullFailEvents;

    public event Action<Weapon> TriggerReleaseSuccessEvents;
    public event Action<Weapon> TriggerReleaseFailEvents;

    public event Action<Weapon> WeaponShootSuccessEvents;
    public event Action<Weapon> WeaponShootFailEvents;

    public event Action<Weapon> ReloadPressEvent;
    #endregion


    [field: SerializeField, Header("Projectile & Pooling")]
    public GameObject ProjectilePrefab
    { get; set; }

    public List<Projectile> ProjectilePooling
    { get; private set; } = new List<Projectile>();

    [field: SerializeField]
    public Transform ProjectilePoolingParent
    { get; set; }


    [field: SerializeField, Header("Weapon Look At")]
    public float DistanceMin
    { get; private set; } = 10;

    [field: SerializeField]
    public float DistanceMax
    { get; private set; } = 200;

    public Vector3 LookAtPoint 
    { get; private set; } = Vector3.zero;

    [field: SerializeField]
    public float RotationSpeed 
    { get; private set; } = 100f;

    public bool IsHeld
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

    public WeaponSoundEmitter SoundEmitter
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

        if (TryGetComponent<WeaponSoundEmitter>(out WeaponSoundEmitter soundEmitter))
        {
            SoundEmitter = soundEmitter;
        }
        else
        {
            gameObject.AddComponent<WeaponSoundEmitter>();
        }

        // Init of the stats from the SO reference.
        SetWeaponData();

        // Set up the listeners for the SO to allow changes to be pushed to runtime instances that are using the SO data.
        WeaponData.UpdateLinkedWeapons += SetWeaponData;

        // Sets up the object pooling.
        InstantiateProjectilesForPooling(ProjectilePoolingParent, AmmoClipSize * 3);

        // Initialise the interal cooldown time for firerate management.
        CoolDownTimer = FireRatePerSecond;

        CanReload = true;
    }

    private void OnDestroy()
    {
        if (WeaponData != null)
        {
            WeaponData.UpdateLinkedWeapons -= SetWeaponData;
        }
    }
    
    void SetUpWeaponListeners()
    {
        if (SoundsSO != null && SoundEmitter != null)
        {
            SoundsSO.SetUpWeaponListeners(this);
        }
    }

    void RemoveWeaponListeners()
    {
        if (SoundsSO != null && SoundEmitter != null)
        {
            SoundsSO.RemoveWeaponListeners(this);
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

        // This only works the the object has been flagged as held by the player.
        if (IsHeld && transform.parent != null)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo, DistanceMax))
            {
                float distanceToPoint = Vector3.Distance(transform.position, hitInfo.point);

                if (distanceToPoint <= DistanceMin)
                {
                    LookAtPoint = Camera.main.transform.position + (Camera.main.transform.forward * DistanceMin);
                }
                else if (distanceToPoint >= DistanceMax)
                {
                    LookAtPoint = Camera.main.transform.position + (Camera.main.transform.forward * DistanceMin);
                }
                else
                {
                    LookAtPoint = hitInfo.point;
                }
            }
            else
            {
                LookAtPoint = Camera.main.transform.position + (Camera.main.transform.forward * DistanceMin);
            }

            Quaternion targetRotation = Quaternion.LookRotation(LookAtPoint - transform.position, transform.parent.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }

        /*
                if (IsHeld && transform.parent != null)
                {
                    LookAtPoint = Camera.main.transform.position + (Camera.main.transform.forward * DistanceMax);

                    Quaternion targetRotation = Quaternion.LookRotation(LookAtPoint - transform.position, transform.parent != null ? transform.parent.up : transform.up);

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
                }
        */
    }

    /// <summary>
    /// Called on input down for the fire input, ensures weapon fire according to it mode of fire.
    /// </summary>
    public void TriggerPressed()
    {
        if (IsReloading == false)
        {
            IsTriggerHeld = true;

            WeaponBehaviour.OnTriggerPress(this);
        }
    }

    /// <summary>
    /// To be called on input down for the fire input.Cancels the firing status of the weapon.
    /// </summary>
    public void TriggerReleased()
    {
        IsTriggerHeld = false;

        if (IsReloading == false)
        {
            WeaponBehaviour.OnTriggerRelease(this);
        }
    }

    public void ReloadPressd()
    {
        if (IsReloading == false && CanReload == true)
        {
            IsReloading = true;

            CanReload = false;

            WeaponBehaviour.OnReloadPress(this);
        }
    }

    public void FillAmmoClip()
    {
        CurrentAmmoInClip = AmmoClipSize;
    }

    public bool TryGetAmmoValueFromClip(int ammoNeeded, out int ammoAvailable)
    {
        bool isAmmoAvailable = CurrentAmmoInClip > 0;

        if (isAmmoAvailable)
        {
            ammoAvailable = Mathf.Clamp(ammoNeeded, 1, CurrentAmmoInClip);
        }
        else
        {
            ammoAvailable = 0;
        }
        
        return isAmmoAvailable;
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

        ShootWeaponSuccessEventInvoke();

        // After a projectile is fired, ensure the weaponcooldown flag is set to allow countdown for next chance to fire to start.
        WeaponCooldown = setCooldown;

        CurrentAmmoInClip = Mathf.Clamp(CurrentAmmoInClip - 1, 0, AmmoClipSize);
    }

    /// <summary>
    /// Binds the firing methods to the attack inputs.
    /// </summary>
    /// <param name="playerInputs"></param>
    public void BindInputs(PlayerInputHandler playerInputs)
    {
        playerInputs.AttackPress += TriggerPressed;
        playerInputs.AttackRelease += TriggerReleased;
        playerInputs.ReloadPress += ReloadPressd;
    }

    /// <summary>
    /// Un-Binds the firing methods from the attack inputs.
    /// </summary>
    /// <param name="playerInputs"></param>
    public void RemoveInputs(PlayerInputHandler playerInputs)
    {
        playerInputs.AttackPress -= TriggerPressed;
        playerInputs.AttackRelease -= TriggerReleased;
        playerInputs.ReloadPress -= ReloadPressd;

        // Helps prevent issues with weapon on being dropped while firing, and being picked up again.
        IsTriggerHeld = false;
    }

    public void TriggerPullSuccessEventInvoke()
    {
        TriggerPullSuccessEvents?.Invoke(this);
    }

    public void TriggerPullFailEventInvoke()
    {
        TriggerPullFailEvents?.Invoke(this);
    }

    public void TriggerReleaseSuccessEventInvoke()
    {
        TriggerReleaseSuccessEvents?.Invoke(this);
    }

    public void TriggerReleaseFailEventInvoke()
    {
        TriggerReleaseFailEvents?.Invoke(this);
    }

    public void ShootWeaponSuccessEventInvoke()
    {
        WeaponShootSuccessEvents?.Invoke(this);
    }

    public void ShootWeaponFailEventInvoke()
    {
        WeaponShootFailEvents?.Invoke(this);
    }

    public void ReloadWeaponEventInvoke()
    {
        ReloadPressEvent?.Invoke(this);
    }


    /// <summary>
    /// Initialise the stats from the scriptable object reference.
    /// </summary>
    public void SetWeaponData()
    {
        if (WeaponData == null)
        {
            WeaponData = ScriptableObject.CreateInstance<WeaponDataSO>();
            Debug.LogWarning($"The starting values for {this.name} of type {this.GetType()} were null on awake");
        }

        FireRatePerSecond = WeaponData.FireRatePerSecond;
        ReloadTime = WeaponData.ReloadTime;
        AmmoClipSize = WeaponData.AmmoClipSize;

        RemoveWeaponListeners();
        WeaponBehaviour = WeaponData.WeaponBehaviour;
        SoundsSO = WeaponData.SoundsSO;
        SetUpWeaponListeners();

        ChargeTime = WeaponData.ChargeTime;
        BurstNumberOfShots = WeaponData.BurstNumberOfShots;
        BurstShotFireRate = WeaponData.BurstShotFireRate;

        CurrentAmmoInClip = AmmoClipSize;
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

    private void OnValidate()
    {
        WeaponData.ClearLinkedWeaponValues();

        SetWeaponData();

        WeaponData.UpdateLinkedWeapons += SetWeaponData;
    }
}
