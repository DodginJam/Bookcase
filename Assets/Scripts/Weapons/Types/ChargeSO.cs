using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Charge WeaponBehaviour", menuName = "WeaponBehaviourSO/ChargeSO")]
public class ChargeSO : WeaponBehaviourSO
{
    public float ChargeTimer
    {  get; set; }

    public override void OnTriggerPress(Weapon weapon)
    {
        if (weapon.WeaponCooldown == false)
        {
            weapon.FireRoutine = weapon.StartCoroutine(ChargingFire(weapon));
            weapon.TriggerPullSuccessEventInvoke();
        }
        else
        {
            weapon.TriggerPullFailEventInvoke();
        }
    }

    public override void OnTriggerRelease(Weapon weapon)
    {
        if (ChargeTimer <= 0)
        {
            weapon.TriggerReleaseSuccessEventInvoke();
        }
        else
        {
            weapon.TriggerReleaseFailEventInvoke();
            weapon.ShootWeaponFailEventInvoke();
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
