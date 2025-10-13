using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Burst WeaponBehaviour", menuName = "WeaponBehaviourSO/BurstSO")]
public class BurstSO : WeaponBehaviourSO
{
    public override void OnTriggerPress(Weapon weapon)
    {
        if (weapon.WeaponCooldown == false && weapon.FireRoutine == null)
        {
            weapon.FireRoutine = weapon.StartCoroutine(BurstingFire(weapon));
        }
        else
        {
            weapon.TriggerPullFailEventInvoke();
        }
    }

    public override void OnTriggerRelease(Weapon weapon)
    {
        weapon.TriggerReleaseSuccessEventInvoke();
    }

    public IEnumerator BurstingFire(Weapon weapon)
    {
        if (weapon.TryGetAmmoValueFromClip(weapon.BurstNumberOfShots, out int shotsLeftInBurst))
        {
            float burstTimer = 0;

            weapon.CanReload = false;

            weapon.TriggerPullSuccessEventInvoke();

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
        }
        else
        {
            weapon.TriggerPullFailEventInvoke();
        }

        // Ensure the burst fire Coroutine reference is set to clear and reset the cooldown timer for between burst shots.
        weapon.FireRoutine = null;
        weapon.CanReload = true;
        // CoolDownTimer = FireRatePerSecond;
    }
}
