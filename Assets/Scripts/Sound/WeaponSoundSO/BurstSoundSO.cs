using UnityEngine;

[CreateAssetMenu(fileName = "new BurstSoundSO", menuName = "SoundsSO/BurstSoundSO")]
public class BurstSoundSO : WeaponSoundsSO
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
