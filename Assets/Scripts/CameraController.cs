
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Player _player;
    
    private void Start()
    {
        // Recuperamos el jugador
        _player = GameManager.Instance.Player;
    }
    
    void LateUpdate()
    {
        // Hacemos que la cámara siga al jugador
        var position = _player.transform.position;
        transform.position = new Vector3(position.x, position.y, -10f);
    }
}
