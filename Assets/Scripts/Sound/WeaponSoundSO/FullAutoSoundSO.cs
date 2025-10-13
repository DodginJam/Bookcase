using UnityEngine;

[CreateAssetMenu(fileName = "new FullAutoSoundSO", menuName = "SoundsSO/FullAutoSoundSO")]
public class FullAutoSoundSO : WeaponSoundsSO
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
