using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private Sprite _activatedSprite;
    [SerializeField] private Sprite _deactivatedSprite;
    [SerializeField] private GameObject _action;
    
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        // Recupera componentes
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Si el jugador entra en el botón, se activa
        if (col.gameObject.CompareTag("Player"))
        {
            Activated();
        }
    }
    
    // Cuando se quita del botón, vuelve a su sprite original
    /*private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Deactivated();
        }
    } */

    private void Activated()
    {
        // Reproduce el sonido de botón, cambia el sprite y activa la acción
        GameManager.Instance.SoundManager.PlaySound("FloorButton");
        _spriteRenderer.sprite = _activatedSprite;
        _action.SetActive(true);
    }
    
    private void Deactivated()
    {
        _spriteRenderer.sprite = _deactivatedSprite;
    }
}