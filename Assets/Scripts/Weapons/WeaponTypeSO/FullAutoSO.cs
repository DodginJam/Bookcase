using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New FullAuto", menuName = "Create New FullAuto")]
public class FullAutoSO : WeaponTypeSO
{
    public override void OnFirePressed(Weapon weapon)
    {
        weapon.FireRoutine = weapon.StartCoroutine(StartAutoFire(weapon));
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
