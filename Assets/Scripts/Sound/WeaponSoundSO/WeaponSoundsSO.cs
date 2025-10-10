using UnityEngine;

public abstract class WeaponSoundsSO : ScriptableObject
{
    [field: SerializeField, Header("General Weapon Sounds")]
    public AudioClip TriggerPull
    { get; private set; }

    [field: SerializeField]
    public AudioClip TriggerRelease
    { get; private set; }

    [field: SerializeField]
    public AudioClip FireProjectile
    { get; private set; }
}
