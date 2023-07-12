using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private Dictionary<string, AudioClip> _soundDictionary;
    private AudioSource _audioSource;

    private void Start()
    {
        // Referencia al audio source
        _audioSource = GetComponent<AudioSource>();

        // Registro en el game manager
        if (GameManager.Instance.SoundManager != null)
        {
            Destroy(this);
        }
        else
            GameManager.Instance.SoundManager = this;

        // Inicializa el diccionario
        _soundDictionary = new Dictionary<string, AudioClip>
        {
            { "BombExplosion", Resources.Load<AudioClip>("Audio/Bomb") },
            { "EnemyKill", Resources.Load<AudioClip>("Audio/EnemyKill") },
            { "Death", Resources.Load<AudioClip>("Audio/Death") },
            { "Heart", Resources.Load<AudioClip>("Audio/Heart") },
            { "FloorButton", Resources.Load<AudioClip>("Audio/FloorButton") },
            { "BombFuse", Resources.Load<AudioClip>("Audio/BombFuse") },
            { "Attack", Resources.Load<AudioClip>("Audio/Attack") },
            { "Key", Resources.Load<AudioClip>("Audio/Key") },
            { "Damage", Resources.Load<AudioClip>("Audio/Damage") },
            { "Lever", Resources.Load<AudioClip>("Audio/Lever") },
            { "Door", Resources.Load<AudioClip>("Audio/Door") },
            { "Dungeon", Resources.Load<AudioClip>("Audio/Dungeon") },
            { "Victory", Resources.Load<AudioClip>("Audio/Victory") },
            { "MainMenu", Resources.Load<AudioClip>("Audio/MainMenu") },
            { "GameOver", Resources.Load<AudioClip>("Audio/GameOver") }
        };
    }

    // Reproduce el sonido indicado
    public void PlaySound(string soundName)
    {
        if (_soundDictionary.ContainsKey(soundName))
        {
            _audioSource.PlayOneShot(_soundDictionary[soundName]);
        }
        else
        {
            Debug.LogWarning("No existe: " + soundName);
        }
    }
}