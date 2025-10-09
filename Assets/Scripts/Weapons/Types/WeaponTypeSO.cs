using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class WeaponTypeSO : ScriptableObject
{
    public abstract void OnTriggerPress(Weapon weapon);

    public abstract void OnTriggerRelease(Weapon weapon);
}
