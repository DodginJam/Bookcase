using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponStats", menuName = "Create New WeaponStats")]
public class WeaponStats : ScriptableObject
{
    [field: SerializeField, Min(0)]
    public float DamagerPerProjectile
    { get; private set; }

    [field: SerializeField, Min(0.1f)]
    public float FireRatePerSecond
    { get; private set; } = 0.1f;

    [field: SerializeField, Min(0.1f)]
    public float ProjectileForce
    { get; private set; } = 0.1f;

    [field: SerializeField]
    public float ReloadTime
    { get; private set; } = 1.0f;

    [field: SerializeField]
    public int AmmoClipSize
    { get; private set; } = 30;

    [field: SerializeField]
    public FireMode FireModeState
    { get; private set; }

    public enum FireMode
    {
        FullAuto,
        SemiAuto,
        Burst,
    }
}
