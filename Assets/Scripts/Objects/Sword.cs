using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private float damage;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Si la espada entra en contacto con un enemigo, le hace daño
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
}
