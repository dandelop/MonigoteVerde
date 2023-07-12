using UnityEngine;

public class DungeonAudio : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    void Start()
    {
        _audioSource.Play();
    }
}
