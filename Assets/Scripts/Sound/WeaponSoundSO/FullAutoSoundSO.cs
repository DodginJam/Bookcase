using UnityEngine;

[CreateAssetMenu(fileName = "new FullAutoSoundSO", menuName = "SoundsSO/FullAutoSoundSO")]
public class FullAutoSoundSO : WeaponSoundsSO
{
    public override void SetUpWeaponListeners(Weapon weapon)
    {
        weapon.TriggerPullSuccessEvents += PlayTriggerPulledSuccessSound;

        weapon.TriggerPullFailEvents += PlayTriggerPulledFailureSound;

        weapon.TriggerReleaseSuccessEvents += PlayTriggerReleasedSuccessSound;

        weapon.WeaponShootSuccessEvents += PlayWeaponShootSuccess;
    }

    public override void RemoveWeaponListeners(Weapon weapon)
    {
        weapon.TriggerPullSuccessEvents -= PlayTriggerPulledSuccessSound;

        weapon.TriggerPullFailEvents -= PlayTriggerPulledFailureSound;

        weapon.TriggerReleaseSuccessEvents -= PlayTriggerReleasedSuccessSound;

        weapon.WeaponShootSuccessEvents -= PlayWeaponShootSuccess;
    }
}
