using System;
using UnityEngine;

public interface IDamagable
{
    public float Health
    { get; set; }

    public float MaxHealth
    { get; set; }

    public void TakeDamage(float damageToTake)
    {
        float newHealth = Health - damageToTake;

        Health = (newHealth < 0) ? 0 : newHealth;
    }

    public void RestoreHealth(float healthToHeal)
    {
        float newHealth = Health + healthToHeal;

        Health = (newHealth > MaxHealth) ? MaxHealth : newHealth;
    }
}
