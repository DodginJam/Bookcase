using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New SemiAutoSO", menuName = "Create New SemiAutoSO")]
public class SemiAutoSO : WeaponTypeSO
{
    public override void OnTriggerPress(Weapon weapon)
    {
        if (weapon.WeaponCooldown == false)
        {
            weapon.FireProjectile();
            TriggerPullEventInvoke(true);
        }
        else
        {
            TriggerPullEventInvoke(false);
        }
    }

    public override void OnTriggerRelease(Weapon weapon)
    {
        TriggerReleaseEventInvoke(true);
    }
}
