using UnityEngine;

public class Knife : MonoBehaviour
{
    [SerializeField] private float damage;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Si el collider del cuchillo colisiona con el jugador y no está parpadeando, recibe daño
        if (col.gameObject.CompareTag("Player") && !col.gameObject.GetComponent<Player>().IsBlinking)
        {
            col.gameObject.GetComponent<Player>().TakeDamage(damage);           
        }
    }
}
