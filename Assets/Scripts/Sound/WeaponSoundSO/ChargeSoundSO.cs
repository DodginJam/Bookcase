using UnityEngine;

[CreateAssetMenu(fileName = "new ChargeSoundSO", menuName = "SoundsSO/ChargeSoundSO")]
public class ChargeSoundSO : WeaponSoundsSO
{
    [field: SerializeField, Header("Charge Weapon Sounds")]
    public AudioClip Charging
    { get; set; }

    public override void SetUpWeaponListeners(Weapon weapon)
    {
        weapon.TriggerPullSuccessEvents += PlayTriggerPulledSuccessSound;
        weapon.TriggerPullSuccessEvents += PlayWeaponCharge;

        weapon.TriggerPullFailEvents += PlayTriggerPulledFailureSound;

        weapon.TriggerReleaseSuccessEvents += PlayTriggerReleasedSuccessSound;
        weapon.TriggerReleaseSuccessEvents += EndWeaponCharge;

        weapon.TriggerReleaseFailEvents += PlayTriggerReleasedFailSound;
        weapon.TriggerReleaseFailEvents += EndWeaponCharge;

        weapon.WeaponShootSuccessEvents += PlayWeaponShootSuccess;
        weapon.WeaponShootFailEvents += PlayWeaponShootFail;
    }

    public override void RemoveWeaponListeners(Weapon weapon)
    {
        weapon.TriggerPullSuccessEvents -= PlayTriggerPulledSuccessSound;
        weapon.TriggerPullSuccessEvents -= PlayWeaponCharge;

        weapon.TriggerPullFailEvents -= PlayTriggerPulledFailureSound;

        weapon.TriggerReleaseSuccessEvents -= PlayTriggerReleasedSuccessSound;
        weapon.TriggerReleaseSuccessEvents -= EndWeaponCharge;

        weapon.TriggerReleaseFailEvents -= PlayTriggerReleasedFailSound;
        weapon.TriggerReleaseFailEvents -= EndWeaponCharge;

        weapon.WeaponShootSuccessEvents -= PlayWeaponShootSuccess;
        weapon.WeaponShootFailEvents -= PlayWeaponShootFail;
    }

    public void PlayWeaponCharge(Weapon weapon)
    {
        weapon.SoundEmitter.PlaySoundMain(Charging);
    }

    public void EndWeaponCharge(Weapon weapon)
    {
        weapon.SoundEmitter.StopSoundMain();
    }
}
