using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponStatsSO", menuName = "Create New WeaponStatsSO")]
public class WeaponStatsSO : ScriptableObject
{
    [field: SerializeField, Min(0.001f)]
    public float FireRatePerSecond
    { get; private set; } = 0.1f;

    [field: SerializeField, Min(0.01f)]
    public float ReloadTime
    { get; private set; } = 1.0f;

    [field: SerializeField, Min(1)]
    public int AmmoClipSize
    { get; private set; } = 30;

    [field: SerializeField]
    public FireMode FireModeState
    { get; private set; }

    /// <summary>
    /// For exclusive use of the charge fire mode, time to charge for shot to fire - exposed via Editor.
    /// </summary>
    [field: SerializeField, Min(0.01f), HideInInspector, Tooltip("For exclusive use of the charge fire mode, time to charge for shot to fire - exposed via Editor.")]
    public float ChargeTime
    { get; set; } = 1.0f;

    /// <summary>
    /// For exclusive use of the burst fire mode, number of projectiles in the burst.
    /// </summary>
    [field: SerializeField, Min(1), HideInInspector, Tooltip("For exclusive use of the burst fire mode, number of projectiles in the burst. - exposed via Editor.")]
    public int BurstNumberOfShots
    { get; set; } = 3;

    /// <summary>
    /// For exclusive use of the burst fire mode, time between each shot within the burst. - exposed via Editor.
    /// </summary>
    [field: SerializeField, Min(0.01f), HideInInspector, Tooltip("For exclusive use of the burst fire mode, time between each shot within the burst. - exposed via Editor.")]
    public float BurstShotFireRate
    { get; set; } = 0.1f;

    public event Action UpdateLinkedWeapons;

    public void UpdateLinkedWeaponValues()
    {
        UpdateLinkedWeapons?.Invoke();
    }
}

public enum FireMode
{
    FullAuto,
    SemiAuto,
    Burst,
    Charge
}
