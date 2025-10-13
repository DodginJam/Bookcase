using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Charge WeaponBehaviour", menuName = "WeaponBehaviourSO/ChargeSO")]
public class ChargeSO : WeaponBehaviourSO
{
    public override void OnTriggerPress(Weapon weapon)
    {
        if (weapon.WeaponCooldown == false)
        {
            if (weapon.TryGetAmmoValueFromClip(1, out _))
            {
                weapon.FireRoutine = weapon.StartCoroutine(ChargingFire(weapon));
                weapon.TriggerPullSuccessEventInvoke();
            }
            else
            {
                weapon.TriggerPullFailEventInvoke();
            }
        }
        else
        {
            weapon.TriggerPullFailEventInvoke();
        }

        weapon.CanReload = false;
    }

    public override void OnTriggerRelease(Weapon weapon)
    {
        if (weapon.ChargeTimer <= 0)
        {
            weapon.TriggerReleaseSuccessEventInvoke();
        }
        else
        {
            weapon.TriggerReleaseFailEventInvoke();
            weapon.ShootWeaponFailEventInvoke();
        }

        weapon.CanReload = true;
    }

    public IEnumerator ChargingFire(Weapon weapon)
    {
        weapon.ChargeTimer = weapon.ChargeTime;

        while (weapon.IsTriggerHeld)
        {
            if (weapon.ChargeTimer > 0)
            {
                weapon.ChargeTimer -= Time.deltaTime;
            }

            if (weapon.ChargeTimer <= 0)
            {
                weapon.FireProjectile();
                break;
            }

            yield return null;
        }

        weapon.FireRoutine = null;
    }
}
