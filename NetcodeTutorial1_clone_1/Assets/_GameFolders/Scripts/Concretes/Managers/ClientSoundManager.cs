using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ClientSoundManager : Singleton<ClientSoundManager>
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _nomAudioClip;

    void OnValidate()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayNomAudioClip()
    {
        _audioSource.clip = _nomAudioClip;
        _audioSource.Play();
    }
}
