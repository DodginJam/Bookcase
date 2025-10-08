using UnityEngine;
using System.Collections.Generic;
using static IInteractable;

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
    public GameObject ProjectilePrefab
    { get; set; }

    public List<GameObject> ProjectilePooling
    { get; private set; } = new List<GameObject>();

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

        SetWeaponStats();

        InstantiateProjectilesForPooling(ProjectilePoolingParent, AmmoClipSize * 3);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
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

        for (int i = 0; i < poolingAmount; i++)
        {
            GameObject newProjectile = Instantiate(ProjectilePrefab);

            newProjectile.transform.parent = parentForPooling;
            newProjectile.transform.localPosition = Vector3.zero;

            if (newProjectile.TryGetComponent<Projectile>(out Projectile projectile))
            {
                projectile.ObjectPoolingReset += ResetGameObjectForPooling;
            }

            ProjectilePooling.Add(newProjectile);

            newProjectile.SetActive(false);
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
}
