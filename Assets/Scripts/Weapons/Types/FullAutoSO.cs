using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New FullAuto WeaponBehaviour", menuName = "WeaponBehaviourSO/FullAutoSO")]
public class FullAutoSO : WeaponBehaviourSO
{
    public override void OnTriggerPress(Weapon weapon)
    {
        weapon.CanReload = false;

        weapon.FireRoutine = weapon.StartCoroutine(StartAutoFire(weapon));
    }

    public override void OnTriggerRelease(Weapon weapon)
    {
        weapon.TriggerReleaseSuccessEventInvoke();

        weapon.CanReload = true;
    }

    /// <summary>
    /// For Full Auto fire, ensures firerate is managed every frame when trigger is held down.
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartAutoFire(Weapon weapon)
    {
        bool isAmmoAvailable = weapon.TryGetAmmoValueFromClip(1, out _);

        if (isAmmoAvailable)
        {
            weapon.TriggerPullSuccessEventInvoke();
        }
        else
        {
            weapon.TriggerPullFailEventInvoke();
        }

        while (weapon.IsTriggerHeld)
        {
            if (weapon.WeaponCooldown == false)
            {
                if (weapon.TryGetAmmoValueFromClip(1, out _))
                {
                    weapon.FireProjectile();
                }
            }

            yield return null;
        }

        weapon.FireRoutine = null;
    }
}
