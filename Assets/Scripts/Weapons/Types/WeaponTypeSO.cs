using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class WeaponTypeSO : ScriptableObject
{
    public abstract void OnFirePressed(Weapon weapon);
}
