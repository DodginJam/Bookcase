using UnityEngine;

[CreateAssetMenu(fileName = "new BurstSoundSO", menuName = "SoundsSO/BurstSoundSO")]
public class BurstSoundSO : WeaponSoundsSO
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
