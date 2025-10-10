using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LocalSoundEmitter : MonoBehaviour
{
    public AudioSource AudioSourceComp
    { get; private set; }

    private void Awake()
    {
        if (TryGetComponent<AudioSource>(out AudioSource audioSource))
        {
            AudioSourceComp = audioSource;
        }
        else
        {
            AudioSourceComp = gameObject.AddComponent<AudioSource>();
        }
    }
}
