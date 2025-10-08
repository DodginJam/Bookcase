using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponStatsSO", menuName = "Create New WeaponStatsSO")]
public class WeaponStatsSO : ScriptableObject
{
    [field: SerializeField, Min(0.001f)]
    public float FireRatePerSecond
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
}

public enum FireMode
{
    FullAuto,
    SemiAuto,
    Burst,
    Charge
}
