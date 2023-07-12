using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject pauseMenu;
    
    private GameManager()
    {
        if (Instance != null)
        {
            Instance = this;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(this);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    // Referencias
    public Player _player;
    public Player Player
    {
        get => _player;
        set => _player = value;
    }
    public HUDManager HUDManager { get; set; }
    public SoundManager SoundManager { get; set; }
    
    // Menú pausa
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    // Cambia el tiempo y muestra/oculta el menú de pausa
    private void PauseGame()
    {
        //si estamos en la escena dungeon se puede pausar
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "DungeonScene")
        {
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
            }
            else
            {
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
            }
        }
    }

    // Carga la escena de game over
    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    public void EnemyKilled()
    {
        // Busca en la escena objetos de tipo Enemy
        var enemies = FindObjectsOfType<Enemy>();
        
        //si no hay enemigos, carga la escena de victoria
        if (enemies.Length == 1)
        {
            StartCoroutine("WaitToLoadScene");
        }
    }
    
    // Corrutina para esperar antes de cargar la escena de victoria
    private IEnumerator WaitToLoadScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("VictoryScene");
    }
}
