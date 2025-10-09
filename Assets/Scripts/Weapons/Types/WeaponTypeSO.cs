using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class WeaponTypeSO : ScriptableObject
{
    public event Action<bool> TriggerPullEvents;

    public event Action<bool> TriggerReleaseEvents;

    public event Action OnWeaponShoot;

    public abstract void OnTriggerPress(Weapon weapon);

    public abstract void OnTriggerRelease(Weapon weapon);

    public void TriggerPullEventInvoke(bool successfulTriggerPull)
    {
        TriggerPullEvents?.Invoke(successfulTriggerPull);
    }

    public void TriggerReleaseEventInvoke(bool successfulTriggerRelease)
    {
        TriggerReleaseEvents?.Invoke(successfulTriggerRelease);
    }
}
