using UnityEngine;

public abstract class WeaponSoundsSO : ScriptableObject
{
    [field: SerializeField, Header("General Weapon Sounds")]
    public AudioClip TriggerPull
    { get; private set; }

    [field: SerializeField]
    public AudioClip TriggerPullFail
    { get; private set; }

    [field: SerializeField]
    public AudioClip TriggerRelease
    { get; private set; }

    [field: SerializeField]
    public AudioClip TriggerReleaseFail
    { get; private set; }

    [field: SerializeField]
    public AudioClip FireProjectile
    { get; private set; }

    [field: SerializeField]
    public AudioClip FireProjectileFail
    { get; private set; }

    [field: SerializeField]
    public AudioClip Reloading
    { get; private set; }

    public virtual void SetUpWeaponListeners(Weapon weapon)
    {
        weapon.TriggerPullSuccessEvents += PlayTriggerPulledSuccessSound;
        weapon.TriggerPullFailEvents += PlayTriggerPulledFailureSound;
        weapon.TriggerReleaseSuccessEvents += PlayTriggerReleasedSuccessSound;
        weapon.WeaponShootSuccessEvents += PlayWeaponShootSuccess;

        weapon.ReloadPressEvent += PlayWeaponReload;
    }

    public virtual void RemoveWeaponListeners(Weapon weapon)
    {
        weapon.TriggerPullSuccessEvents -= PlayTriggerPulledSuccessSound;
        weapon.TriggerPullFailEvents -= PlayTriggerPulledFailureSound;
        weapon.TriggerReleaseSuccessEvents -= PlayTriggerReleasedSuccessSound;
        weapon.WeaponShootSuccessEvents -= PlayWeaponShootSuccess;

        weapon.ReloadPressEvent -= PlayWeaponReload;
    }

    public void PlayTriggerPulledSuccessSound(Weapon weapon)
    {
        weapon.SoundEmitter.PlayOneShot(TriggerPull);
    }

    public void PlayTriggerPulledFailureSound(Weapon weapon)
    {
        weapon.SoundEmitter.PlayOneShot(TriggerPullFail);
    }

    public void PlayTriggerReleasedSuccessSound(Weapon weapon)
    {
        weapon.SoundEmitter.PlayOneShot(TriggerRelease);
    }

    public void PlayTriggerReleasedFailSound(Weapon weapon)
    {
        weapon.SoundEmitter.PlayOneShot(TriggerReleaseFail);
    }

    public void PlayWeaponShootSuccess(Weapon weapon)
    {
        weapon.SoundEmitter.PlayOneShot(FireProjectile);
    }

    public void PlayWeaponShootFail(Weapon weapon)
    {
        weapon.SoundEmitter.PlayOneShot(FireProjectileFail);
    }

    public void PlayWeaponReload(Weapon weapon)
    {
        weapon.SoundEmitter.PlayOneShot(Reloading);
    }
}
