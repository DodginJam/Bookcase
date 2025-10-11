using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New SemiAuto WeaponBehaviour", menuName = "WeaponBehaviourSO/SemiAutoSO")]
public class SemiAutoSO : WeaponBehaviourSO
{
    public override void OnTriggerPress(Weapon weapon)
    {
        if (weapon.WeaponCooldown == false)
        {
            weapon.FireProjectile();
            weapon.TriggerPullSuccessEventInvoke();
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
}
