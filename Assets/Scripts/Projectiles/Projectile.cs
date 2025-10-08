using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IDamaging
{
    [field: SerializeField]
    public ProjectileSO ProjectileValues
    { get; private set; }

    public float Damage
    { get; set; }

    public float Force
    { get; set; }

    public float ActiveLifeTime
    { get; set; }

    public Coroutine LifeTimer
    { get; set; }

    public Rigidbody Rigidbody
    { get; set; }

    public event Action<GameObject> ObjectPoolingReset;

    void Awake()
    {
        if (Rigidbody == null)
        {
            if (TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
            {
                Rigidbody = rigidBody;
            }
            else
            {
                this.gameObject.AddComponent<Rigidbody>();
            }
        }

        InitialiseProjectile();

        ProjectileValues.UpdateLinkedProjectiles += InitialiseProjectile;
    }

    private void OnDestroy()
    {
        if (ProjectileValues != null)
        {
            ProjectileValues.UpdateLinkedProjectiles -= InitialiseProjectile;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitialiseProjectile()
    {
        if (ProjectileValues == null)
        {
            ProjectileValues = ScriptableObject.CreateInstance<ProjectileSO>();
            Debug.LogWarning($"The starting values for {this.name} of type {this.GetType()} were null on awake");
        }

        Damage = ProjectileValues.Damage;
        Force = ProjectileValues.Force;
        ActiveLifeTime = ProjectileValues.ActiveLifeTime;

        if (Rigidbody != null) Rigidbody.mass = ProjectileValues.Mass;
        
    }

    public void ResetRigidbodyProjectile()
    {
        if (Rigidbody != null)
        {
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.linearVelocity = Vector3.zero;
        }
    }

    public void ActivateLiveProjectile(Transform positionAndDirection)
    {
        this.gameObject.transform.position = positionAndDirection.position;

        this.gameObject.transform.parent = null;

        ResetRigidbodyProjectile();

        this.gameObject.SetActive(true);

        LifeTimer = StartCoroutine(LifeTimeCounter());

        Rigidbody.AddForce(positionAndDirection.forward * Force, ForceMode.Impulse);
    }

    public void DeactivateLiveProjectile()
    {
        ResetRigidbodyProjectile();

        ObjectPoolingReset?.Invoke(this.gameObject);

        if (LifeTimer != null)
        {
            StopCoroutine(LifeTimer);
            LifeTimer = null;
        }

        this.gameObject.SetActive(false);
    }

    public IEnumerator LifeTimeCounter()
    {
        float currentTimer = ActiveLifeTime;

        while (this.gameObject.activeSelf)
        {
            currentTimer -= Time.deltaTime;

            if (currentTimer <= 0)
            {
                DeactivateLiveProjectile();

                break;
            }

            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransformToPass = (collision.rigidbody != null) ? collision.rigidbody.transform : collision.collider.transform;

        if (hitTransformToPass.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.TakeDamage(Damage);
        }

        DeactivateLiveProjectile();
    }
}
