using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.SoundManager.PlaySound("GameOver");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
