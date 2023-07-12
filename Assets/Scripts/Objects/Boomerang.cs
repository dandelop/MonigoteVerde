using UnityEngine;

public class Boomerang : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float maxDistance;
    
    // Variables
    private bool _back;
    private float _distance;
    private Vector2 _direction;

    private void OnEnable()
    {
        // Se lanza en la dirección en la que mira el jugador
        _direction = GameManager.Instance.Player.PlayerFaceDirection;
    }

    private void Update()
    {
        // Distancia recorrida
        float travel = Time.deltaTime * speed;

        if (!_back)
        {
            // Va sumando la distancia recorrida
            gameObject.transform.Translate(_direction * travel);
            _distance += travel;
            
            // Si llega a la distancia máxima, vuelve
            if (_distance >= maxDistance)
            {
                _back = true;
            }
        }
        else // Vuelve al jugador
        {
            // Establece el objetivo en el player y se mueve hacia él
            Vector2 target = GameManager.Instance.Player.transform.position;
            gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, target, travel);
            _distance -= travel;
            
            // Cuando llega al jugador, se destruye y puede lanzarlo de nuevo
            if ((Vector2)gameObject.transform.position == target)
            {
                GameManager.Instance.Player.CanLaunchBoomerang = true;
                GameManager.Instance.HUDManager.CantUseItem("Boomerang");
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // Si choca con una palanca, vuelve
        if (col.gameObject.CompareTag("Lever"))
        {
            _back = true;
        }
        // Si choca con un enemigo, le hace daño y vuelve
        else if (col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            _back = true;
        }
    }
}