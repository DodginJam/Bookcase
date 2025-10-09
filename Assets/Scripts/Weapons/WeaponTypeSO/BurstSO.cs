using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New BurstSO", menuName = "Create New BurstSO")]
public class BurstSO : WeaponTypeSO
{
    public override void OnFirePressed(Weapon weapon)
    {
        if (weapon.WeaponCooldown == false && weapon.FireRoutine == null)
        {
            weapon.FireRoutine = weapon.StartCoroutine(BurstingFire(weapon, this));
        }
    }

    public IEnumerator BurstingFire(Weapon weapon, WeaponTypeSO weaponTypeSO)
    {
        int shotsLeftInBurst = weapon.BurstNumberOfShots;

        float burstTimer = 0;

        while (shotsLeftInBurst > 0)
        {
            if (weapon.WeaponCooldown == false)
            {
                if (burstTimer > 0)
                {
                    burstTimer -= Time.deltaTime;
                }

                if (burstTimer <= 0)
                {
                    weapon.FireProjectile(false);
                    shotsLeftInBurst--;

                    if (shotsLeftInBurst > 0)
                    {
                        burstTimer = weapon.BurstShotFireRate;
                    }
                    else if (shotsLeftInBurst <= 0)
                    {
                        weapon.WeaponCooldown = true;
                        break;
                    }
                }
            }

            yield return null;
        }

        // Ensure the burst fire Coroutine reference is set to clear and reset the cooldown timer for between burst shots.
        weapon.FireRoutine = null;
        // CoolDownTimer = FireRatePerSecond;
    }
}
