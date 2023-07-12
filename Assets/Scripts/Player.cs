using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, ITakeDamage
{
    [Header("Stats")] 
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;
    [SerializeField] private bool hasKey = false;
    [SerializeField] private float speed;
    [SerializeField] private float blinkDuration = 1.75f;
    
    [Header("Objects")] 
    [SerializeField] private GameObject boomerang;
    [SerializeField] private GameObject bomb;

    [Header("References")] 
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject shadow;

    // Componentes
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D[] _swordCols;

    // Variables
    private float _originalSpeed;
    private bool _slowed;
    private Vector2 _direction;
    private Vector2 _launchDirection = Vector2.down;
    private float _blinkTimer = 0f;
    private bool _playerAttacking;
    private bool _isBlinking = false;
    private bool _canLaunchBoomerang = true;
    private bool _canLaunchBomb = true;


    // Límites de uso de objetos
    public bool CanLaunchBoomerang
    {
        set => _canLaunchBoomerang = value;
    }

    public bool CanLaunchBomb
    {
        set => _canLaunchBomb = value;
    }

    // Dirección del jugador
    public Vector2 PlayerFaceDirection => _launchDirection;

    // Invencibilidad
    public bool IsBlinking
    {
        get => _isBlinking;
    }

    private void Start()
    {
        // Registro en el game manager
        GameManager.Instance.Player = this;

        // Referencia a componentes
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _swordCols = sword.GetComponents<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void Update()
    {
        IsInvencible();
        UpdateGraphics();
        UpdateActions();
    }

    private void UpdateMovement()
    {
        // Controles de movimiento
        
        // Si el jugador está atacando, no se puede mover
        if (_playerAttacking)
        {
            _rigidbody.velocity = Vector2.zero;
            _direction = Vector2.zero;
            return;
        }

        // Obtenemos la dirección del movimiento y se la pasamos al rigidbody
        _direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _rigidbody.velocity = _direction.normalized * speed;
    }

    private void UpdateActions()
    {
        // Ataque básico
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_playerAttacking)
            {
                StartCoroutine(nameof(Attack));
            }
        }

        // Lanzar bumerán
        if (Input.GetKeyDown(KeyCode.Z) && !_playerAttacking)
        {
            if (!_canLaunchBoomerang)
            {
                return;
            }

            // No puede lanzar bumerán hasta que no vuelva
            _canLaunchBoomerang = false;
            GameManager.Instance.HUDManager.CantUseItem("Boomerang");

            // Instancia el bumerán
            Instantiate(boomerang, transform.position, Quaternion.identity);
        }

        // Soltar bomba
        if (Input.GetKeyDown(KeyCode.X) && !_playerAttacking)
        {
            if (!_canLaunchBomb)
            {
                return;
            }

            // No puede lanzar bomba hasta que no explote
            _canLaunchBomb = false;
            GameManager.Instance.HUDManager.CantUseItem("Bomb");

            // Instancia la bomba
            Instantiate(bomb, transform.position, Quaternion.identity);

            // Sonido de mecha encendida
            GameManager.Instance.SoundManager.PlaySound("BombFuse");
        }
    }

    private IEnumerator Attack()
    {
        _playerAttacking = true;

        // Activamos el collider correspondiente a la dirección del ataque
        switch (_launchDirection)
        {
            case { y: < 0 }:
                _swordCols[0].enabled = true; // Down
                break;
            case { y: > 0 }:
                _swordCols[1].enabled = true; // Up
                break;
            case { x: < 0 }:
                _swordCols[2].enabled = true; // Left
                break;
            case { x: > 0 }:
                _swordCols[3].enabled = true; // Right
                break;
        }

        // Sonido de ataque
        GameManager.Instance.SoundManager.PlaySound("Attack");
        
        // Activa la aniamción de ataque y espera 0.3 segundos
        // Al principio usaba trigger y GetLength para la duración pero no era consistente
        _animator.SetBool("Attacking", true);
        yield return new WaitForSeconds(0.3f);
        _animator.SetBool("Attacking", false);

        // Desactiva todos los colliders
        Array.ForEach(_swordCols, col => col.enabled = false);

        _playerAttacking = false;
    }

    private void UpdateGraphics()
    {
        // Controles para el animator
        _animator.SetBool("Stop", _direction == Vector2.zero);

        // Coge el valor de Y y ajusta animación, sprite y dirección de lanzamiento
        switch (_direction.y)
        {
            case > 0:
                _spriteRenderer.flipX = false;
                _animator.SetTrigger("WalkingUp");
                _launchDirection = Vector2.up;
                break;
            case < 0:
                _spriteRenderer.flipX = false;
                _animator.SetTrigger("WalkingDown");
                _launchDirection = Vector2.down;
                break;
            default:
            {
                // Coge el valor de X y ajusta animación, sprite y dirección de ataque
                switch (_direction.x)
                {
                    case < 0:
                        _spriteRenderer.flipX = false;
                        _animator.SetTrigger("WalkingSide");
                        _launchDirection = Vector2.left;
                        break;
                    case > 0:
                        _spriteRenderer.flipX = true;
                        _animator.SetTrigger("WalkingSide");
                        _launchDirection = Vector2.right;
                        break;
                }

                break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si toca la puerta y tiene llave, la abre
        if (collision.gameObject.CompareTag("Door") && hasKey)
        {
            hasKey = false;

            // Sonido de puerta
            GameManager.Instance.SoundManager.PlaySound("Door");

            // Quita la llave de la interfaz
            GameManager.Instance.HUDManager.ShowKey();
            Destroy(collision.gameObject);
        }

        // Si toca la llave, la coge
        if (collision.gameObject.CompareTag("Key"))
        {
            hasKey = true;

            // Sonido de llave
            GameManager.Instance.SoundManager.PlaySound("Key");

            // Muestra la llave en la interfaz
            GameManager.Instance.HUDManager.ShowKey();
            Destroy(collision.gameObject);
        }
    }

    // Interfaz de daño
    public void TakeDamage(float damage)
    {
        // Si está parpadeando, no recibe daño
        if (_isBlinking)
        {
            return;
        }

        // Quita una vida, reproduce sonido de daño y actualiza la interfaz
        health -= 1;
        GameManager.Instance.SoundManager.PlaySound("Damage");
        GameManager.Instance.HUDManager.LostHealth();
        
        // Si se queda sin vida, muere
        if (health <= 0)
        {
            Die();
        }
        // Si no, se vuelve invencible durante un tiempo
        else
        {
            _isBlinking = true;
            _blinkTimer = blinkDuration;
        }
    }
    
    public void PickUpHeart()
    {
        // Reproduce sonido
        GameManager.Instance.SoundManager.PlaySound("Heart");

        // Si tiene la vida al máximo, no hace nada
        if (health >= maxHealth)
        {
            return;
        }
        
        // Aumenta la vida y actualiza la interfaz
        health += 1;
        GameManager.Instance.HUDManager.AddHealth();
    }

    private void IsInvencible()
    {
        if (_isBlinking)
        {
            // Disminuye el temporizador
            _blinkTimer -= Time.deltaTime;

            // Si el temporizador llega a cero, detiene el parpadeo
            if (_blinkTimer <= 0f)
            {
                _isBlinking = false;
                Color color = _spriteRenderer.color;
                color.a = 1f;
                _spriteRenderer.color = color;
            }
            else
            {
                // Cambia la opacidad del sprite varias veces por segundo
                Color color = _spriteRenderer.color;
                color.a = Mathf.PingPong(Time.time * 20f, 1f);
                _spriteRenderer.color = color;
            }
        }
    }

    private void Die()
    {
        // Ejecuta la animación de muerte y desactiva la sombra
        _animator.SetTrigger("Death");
        shadow.SetActive(false);

        // Sonido de muerte
        GameManager.Instance.SoundManager.PlaySound("Death");

        // Espera medio segundo y llama a GameOver
        Invoke(nameof(GameOver), 0.5f);
    }

    private void GameOver()
    {
        GameManager.Instance.GameOver();
    }
}