using UnityEngine;

[CreateAssetMenu(fileName = "new ChargeSoundSO", menuName = "SoundsSO/ChargeSoundSO")]
public class ChargeSoundSO : WeaponSoundsSO
{
    [field: SerializeField, Header("Charge Weapon Sounds")]
    public AudioClip Charging
    { get; set; }

    [field: SerializeField]
    public AudioClip FailedCharge
    { get; set; }
}
