using System;
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

    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransformToPass = (collision.rigidbody != null) ? collision.rigidbody.transform : collision.collider.transform;

        if (hitTransformToPass.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.TakeDamage(Damage);
        }

        ResetRigidbodyProjectile();

        ObjectPoolingReset?.Invoke(this.gameObject);

        this.gameObject.SetActive(false);
    }
}
