using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] private Sprite leverOn;
    [SerializeField] private GameObject action;
    [SerializeField] private GameObject action2;
    
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        // Recupera componentes
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Se puede activar con la espada o el bumerán
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Sword"))
        {
            Action();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Boomerang"))
        {
            Action();
        }
    }
    
    private void Action()
    {
        // Reproduce sonido, cambia sprite y activa acción
        GameManager.Instance.SoundManager.PlaySound("Lever");
        _spriteRenderer.sprite = leverOn;
        action.SetActive(true);
        
        // Si hay una segunda acción, la activa o desactiva
        if (action2 != null)
        {
            action2.SetActive(false);
            //action2.SetActive(true);
        }
    }
}