using UnityEngine;

public class SceneManager : MonoBehaviour
{
    // Botón para jugar
    public void OnClickPlay()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DungeonScene");
    }
    
    // Botón para salir
    public void OnClickExit()
    {
        Application.Quit();
    }
    
    // Botón para volver al menú
    public void OnClickMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    // Método para cargar una escena
    public static void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
