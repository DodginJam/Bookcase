using UnityEngine;

public interface IDamaging
{
    public float Damage
    { get; set; }

    public void PassDamage(IDamagable damagable)
    {
        if (damagable == null)
        {
            Debug.LogWarning("Damagable is null");
            return;
        }

        damagable.TakeDamage(Damage);
    }
}
