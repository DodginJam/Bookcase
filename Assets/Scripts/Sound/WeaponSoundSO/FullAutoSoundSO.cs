using UnityEngine;

[CreateAssetMenu(fileName = "new FullAutoSoundSO", menuName = "SoundsSO/FullAutoSoundSO")]
public class FullAutoSoundSO : WeaponSoundsSO
{
    [field: SerializeField, Header("FullAuto Weapon Sounds")]
    public AudioClip BackingFire
    { get; set; }
}
