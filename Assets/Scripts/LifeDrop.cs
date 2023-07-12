using UnityEngine;

public class LifeDrop : MonoBehaviour
{
    [SerializeField] private float timeToDestroy;
    
    private SpriteRenderer _spriteRenderer;
    private bool _isBlinking = false;
    private float _blinkInterval = 0.2f;

    private void Awake()
    {
        // Referencia al componentes
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Tiempo hasta desaparecer
        timeToDestroy -= Time.deltaTime;

        // Cuando queda la mitad de tiempo, empieza a parpadear
        if (timeToDestroy <= timeToDestroy/2 && !_isBlinking)
        {
            _isBlinking = true;
        }

        // Parpadeo cada 0.2 segundos
        if (_isBlinking)
        {
            _blinkInterval -= Time.deltaTime;

            if (_blinkInterval <= 0f)
            {
                _blinkInterval = 0.2f;
                ToggleSpriteVisibility();
            }
        }

        // Cuando se acaba el tiempo, se destruye
        if (timeToDestroy <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    private void ToggleSpriteVisibility()
    {
        // Activa o desactiva el sprite
        _spriteRenderer.enabled = !_spriteRenderer.enabled;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el jugador lo coge, gana una vida y se destruye
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.Player.PickUpHeart();
            Destroy(gameObject);
        }
    }
}
