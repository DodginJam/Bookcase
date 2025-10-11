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

    public abstract void SetUpWeaponListeners(Weapon weapon);

    public abstract void RemoveWeaponListeners(Weapon weapon);

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
}
