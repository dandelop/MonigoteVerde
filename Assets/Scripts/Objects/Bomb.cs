using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float damage; 
    [SerializeField] private float radius;
    
    // Variables
    private Animator _animator;
    private float _countdown = 3f;
    
    void OnEnable()
    {
        // Recupera componentes
        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Cuenta atrás
        _countdown -= Time.deltaTime;

        if (_countdown <= 0f)
        {
            _animator.SetTrigger("Explode");
        }
    }

    private void ExplosionDamage()
    {
        // Reproduce sonido de explosión
        GameManager.Instance.SoundManager.PlaySound("BombExplosion");

        // Crea un círculo de colisión y comprueba si hay enemigos/jugador dentro o si hay objetos rompibles
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<ITakeDamage>().TakeDamage(damage);
            }
            if (col.gameObject.CompareTag("Breakable"))
            {
                Destroy(col.gameObject);
            }
        }
        
        // Puede volver a lanzar bombas
        GameManager.Instance.Player.CanLaunchBomb = true;
        GameManager.Instance.HUDManager.CantUseItem("Bomb");
        
        // Destruye el objeto cuando termina la animación
        Destroy(gameObject, 0.9f);
    }

    // Muestra el radio de explosión
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);   
    }
}
