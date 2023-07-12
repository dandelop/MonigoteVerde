using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, ITakeDamage
{
    [Header("Stats")] 
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float attackRange;
    [SerializeField] private float detectionRange;
    [SerializeField] private float movementDuration;
    [SerializeField] private bool hasKey;

    [Header("References")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject lifeDrop;
    [SerializeField] private GameObject keyDrop;

    // Componentes
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D[] _knifeCols;
    
    // Variables
    private float _actualSpeed;
    private Vector2 _direction;
    private Vector2 _launchDirection = Vector2.down;
    private float _distanceToPlayer;
    private float _elapsedTime;
    private bool _isPatrolling;
    private bool _isAttacking;
    private bool _isDying;
    
    private void Awake()
    {
        // Referencia a los componentes
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _knifeCols = weapon.GetComponents<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        // Mueve al enemigo
        _rigidbody.velocity = _direction.normalized * _actualSpeed;

        // Si está atacando o muriendo, no se mueve
        if (_isAttacking || _isDying)
        {
            _rigidbody.velocity = Vector2.zero;
        }
    }

    void Update()
    {
        UpdateGraphics();
        UpdateBehaviour();
    }

    private void UpdateGraphics()
    {
        // Actualización de la animación de parado
        _animator.SetBool("Stop", _direction == Vector2.zero);

        // Actualización de la animación de movimiento
        switch (_direction.x)
        {
            // Coge el valor de X y ajusta animación, sprite y dirección de ataque
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
            default:
                // Coge el valor de Y y ajusta animación, sprite y dirección de ataque
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
                }
                break;
        }
    }

    private void UpdateBehaviour()
    {
        // Comprueba la distancia al jugador y actúa en consecuencia
        _distanceToPlayer = Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position);
        if (_distanceToPlayer <= detectionRange)
        {
            Attack();
        }
        else
        {
            Patrol();
        }
    }

    private void Attack()
    {
        // Aumenta la velocidad y se dirige hacia el jugador
        _actualSpeed = speed;
        _direction = GameManager.Instance.Player.transform.position - transform.position;
        _rigidbody.velocity = _direction.normalized * _actualSpeed;

        // Cuando esta cerca, ataca
        if (_distanceToPlayer <= attackRange && !_isAttacking)
        {
            _isAttacking = true;
            
            // Activa el collider de ataque correspondiente
            switch (_launchDirection)
            {
                case { x: < 0 }:
                    _knifeCols[0].enabled = true;
                    break;
                case { x: > 0 }:
                    _knifeCols[1].enabled = true;
                    break;
            }
            
            // Animación de ataque
            _animator.SetTrigger("Attacking");
 
            // Prepara el siguiente ataque
            StartCoroutine(PrepareNextAttack());
        }
    }
    
    private IEnumerator PrepareNextAttack()
    {
        // Mantiene el ataque activo durante 1.5 segundos
        yield return new WaitForSeconds(1.5f);
        
        // Desactiva los colliders de ataque
        Array.ForEach(_knifeCols, col => col.enabled = false);
        
        // Actualiza el estado de ataque
        _animator.SetBool("Attacking", false);
        _isAttacking = false;
    }

    private void Patrol()
    {
        // Reduce la velocidad a la mitad y activa la patrulla
        _actualSpeed = speed / 2f;
        if (!_isPatrolling)
        {
            _isPatrolling = true;
            _elapsedTime = 0f;
        }

        // Cambia de dirección cada cierto tiempo de forma aleatoria
        if (_elapsedTime >= movementDuration)
        {
            _elapsedTime = 0f;
            switch (Random.Range(0, 4))
            {
                case 0:
                    _direction = Vector2.down;
                    break;
                case 1:
                    _direction = Vector2.up;
                    break;
                case 2:
                    _direction = Vector2.left;
                    break;
                case 3:
                    _direction = Vector2.right;
                    break;
            }
        }
        _elapsedTime += Time.deltaTime;
    }

    // Interfaz de daño
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }
    
    private void Die()
    {
        // Reproduce el sonido de muerte
        _isDying = true;
        GameManager.Instance.SoundManager.PlaySound("EnemyKill");
        
        // Desactiva los colliders y el arma
        GetComponent<BoxCollider2D>().enabled = false;
        weapon.SetActive(false);
        
        // Activa la animación de muerte
        _animator.SetTrigger("Death");
        
        // Genera un drop aleatorio
        RandomDrop();
        
        // Comprueba si es el último enemigo (condición de victoria
        GameManager.Instance.EnemyKilled();
        
        Destroy(gameObject, 1.1f);
    }

    private void RandomDrop()
    {
        // Si el enemigo tiene una llave, la suelta
        if (hasKey)
        {
            Instantiate(keyDrop, transform.position, Quaternion.identity);
        }
        // Si no, tiene un 50% de probabilidad de soltar una vida
        else if (Random.Range(0, 100) < 50 && !hasKey)
        {
            Instantiate(lifeDrop, transform.position, Quaternion.identity);
        }
    }

    // Dibuja el rango de detección
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}