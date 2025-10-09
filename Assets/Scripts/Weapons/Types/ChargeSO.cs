using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New ChargeSO", menuName = "Create New ChargeSO")]
public class ChargeSO : WeaponTypeSO
{
    public override void OnFirePressed(Weapon weapon)
    {
        if (weapon.WeaponCooldown == false)
        {
            weapon.FireRoutine = weapon.StartCoroutine(ChargingFire(weapon));
        }
    }

    public IEnumerator ChargingFire(Weapon weapon)
    {
        float chargeTimer = weapon.ChargeTime;

        while (weapon.IsTriggerHeld)
        {
            if (chargeTimer > 0)
            {
                chargeTimer -= Time.deltaTime;
            }

            if (chargeTimer <= 0)
            {
                weapon.FireProjectile();
                break;
            }

            yield return null;
        }

        weapon.FireRoutine = null;
    }
}
