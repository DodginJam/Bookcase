using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New ChargeSO", menuName = "Create New ChargeSO")]
public class ChargeSO : WeaponTypeSO
{
    public float ChargeTimer
    {  get; set; }

    public override void OnTriggerPress(Weapon weapon)
    {
        if (weapon.WeaponCooldown == false)
        {
            weapon.FireRoutine = weapon.StartCoroutine(ChargingFire(weapon));
            weapon.TriggerPullEventInvoke(true);
        }
        else
        {
            weapon.TriggerPullEventInvoke(false);
        }
    }

    public override void OnTriggerRelease(Weapon weapon)
    {
        if (ChargeTimer <= 0)
        {
            weapon.TriggerReleaseEventInvoke(true);
        }
        else
        {
            weapon.TriggerReleaseEventInvoke(false);
        }
    }

    public IEnumerator ChargingFire(Weapon weapon)
    {
        ChargeTimer = weapon.ChargeTime;

        while (weapon.IsTriggerHeld)
        {
            if (ChargeTimer > 0)
            {
                ChargeTimer -= Time.deltaTime;
            }

            if (ChargeTimer <= 0)
            {
                weapon.FireProjectile();
                break;
            }

            yield return null;
        }

        weapon.FireRoutine = null;
    }
}
