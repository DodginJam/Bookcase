using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New FullAuto WeaponBehaviour", menuName = "WeaponBehaviourSO/FullAutoSO")]
public class FullAutoSO : WeaponBehaviourSO
{
    public override void OnTriggerPress(Weapon weapon)
    {
        weapon.FireRoutine = weapon.StartCoroutine(StartAutoFire(weapon));

        weapon.TriggerPullSuccessEventInvoke();
    }

    public override void OnTriggerRelease(Weapon weapon)
    {
        weapon.TriggerReleaseSuccessEventInvoke();
    }

    /// <summary>
    /// For Full Auto fire, ensures firerate is managed every frame when trigger is held down.
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartAutoFire(Weapon weapon)
    {
        while (weapon.IsTriggerHeld)
        {
            if (weapon.WeaponCooldown == false)
            {
                weapon.FireProjectile();
            }

            yield return null;
        }

        weapon.FireRoutine = null;
    }
}
