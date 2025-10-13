using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New SemiAuto WeaponBehaviour", menuName = "WeaponBehaviourSO/SemiAutoSO")]
public class SemiAutoSO : WeaponBehaviourSO
{
    public override void OnTriggerPress(Weapon weapon)
    {
        weapon.CanReload = false;

        if (weapon.WeaponCooldown == false)
        {
            if (weapon.TryGetAmmoValueFromClip(1, out _))
            {
                weapon.FireProjectile();
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
    }

    public override void OnTriggerRelease(Weapon weapon)
    {
        weapon.CanReload = true;

        weapon.TriggerReleaseSuccessEventInvoke();
    }
}
