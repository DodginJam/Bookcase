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

    public void PlayOneShot(AudioClip clipToPlay)
    {
        if (AudioSourceComp != null)
        {
            AudioSourceComp.PlayOneShot(clipToPlay);
        }
    }

    public void PlaySoundMain(AudioClip clipToPlay)
    {
        if (AudioSourceComp != null)
        {
            AudioSourceComp.clip = clipToPlay;
            AudioSourceComp.Play();
        }
    }

    public void StopSoundMain()
    {
        if (AudioSourceComp != null)
        {
            AudioSourceComp.clip = null;
        }
    }
}
