using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("HUD Images")] 
    [SerializeField] private GameObject Health;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Image HUDBomb;
    [SerializeField] private Image HUDBoomerang;
    [SerializeField] private Image HUDKey;

    // Variables
    private bool _isKeyVisible;
    private GameObject[] _hearts;
    private int _lastIndex;
    
    private void Start()
    {
        // Registro en el GameManager
        GameManager.Instance.HUDManager = this;
        
        // Asigna los corazones a un array
        _hearts = new GameObject[Health.transform.childCount];
        for (int i = 0; i < Health.transform.childCount; i++)
        {
            _hearts[i] = Health.transform.GetChild(i).gameObject;
        }
    }

    // Cambia la opacidad de los objetos que no se pueden usar
    public void CantUseItem(string item)
    {
        switch (item)
        {
            case "Bomb":
                var hudBombColor = HUDBomb.color;
                // Cambia la opacidad entre 0.4 y 1
                hudBombColor.a = Math.Abs(hudBombColor.a - 0.4f) < 0.1 ? 1f : 0.4f;
                HUDBomb.color = hudBombColor;
                break;

            case "Boomerang":
                var hudBoomerangColor = HUDBoomerang.color;
                // Cambia la opacidad entre 0.4 y 1
                hudBoomerangColor.a = Math.Abs(hudBoomerangColor.a - 0.4f) < 0.1 ? 1f : 0.4f;
                HUDBoomerang.color = hudBoomerangColor;
                break;
        }
    }

    // Muestra/Oculta la llave en el HUD
    public void ShowKey()
    {
        _isKeyVisible = !_isKeyVisible;
        HUDKey.gameObject.SetActive(_isKeyVisible);
    }

    // Vacía un corazón del HUD
    public void LostHealth()
    {
        if (_hearts.Length > 0)
        {
            // Recorre el array de corazones al revés y vacía el primero lleno
            for (int i = _hearts.Length - 1; i >= 0; i--)
            {
                if (_hearts[i].GetComponent<Image>().sprite.Equals(fullHeart))
                {
                    _hearts[i].GetComponent<Image>().sprite = emptyHeart;
                    break;
                }
            }
        }
    }
    
    // Llena un corazón del HUD
    public void AddHealth()
    {
        // Recorre el array de corazones y llena el primero vacío
        for (int i = 0; i < _hearts.Length; i++)
        {
            if (_hearts[i].GetComponent<Image>().sprite.Equals(emptyHeart))
            {
                _hearts[i].GetComponent<Image>().sprite = fullHeart;
                break;
            }
        }
    }
}