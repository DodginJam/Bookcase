using UnityEngine;

[CreateAssetMenu(fileName = "new SemiAutoSoundSO", menuName = "SoundsSO/SemiAutoSoundSO")]
public class SemiAutoSoundSO : WeaponSoundsSO
{
    public override void SetUpWeaponListeners(Weapon weapon)
    {
        base.SetUpWeaponListeners(weapon);
    }

    public override void RemoveWeaponListeners(Weapon weapon)
    {
        base.RemoveWeaponListeners(weapon);
    }
}
