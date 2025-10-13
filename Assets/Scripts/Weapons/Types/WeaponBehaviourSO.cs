using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class WeaponBehaviourSO : ScriptableObject
{
    public abstract void OnTriggerPress(Weapon weapon);

    public abstract void OnTriggerRelease(Weapon weapon);

    public void OnReloadPress(Weapon weapon)
    {
        weapon.ReloadCoroutine = weapon.StartCoroutine(ReloadingRoutine(weapon));

        CancelFiringRoutine(weapon);

        weapon.ReloadWeaponEventInvoke();
    }

    public IEnumerator ReloadingRoutine(Weapon weapon)
    {
        weapon.ReloadTimer = weapon.ReloadTime;

        while (weapon.ReloadTimer > 0)
        {
            weapon.ReloadTimer -= Time.deltaTime;

            yield return null;
        }

        weapon.IsReloading = false;

        weapon.CanReload = true;

        weapon.ReloadCoroutine = null;

        Debug.Log("Reload Timer Finished");
    }

    public void CancelFiringRoutine(Weapon weapon)
    {
        if (weapon.FireRoutine != null)
        {
            weapon.StopCoroutine(weapon.FireRoutine);
            weapon.FireRoutine = null;
        }
    }
}
