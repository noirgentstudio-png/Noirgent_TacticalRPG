using UnityEngine;
using UnityEngine.InputSystem;

public class SceneBackController : MonoBehaviour
{
    [Header("Escena de regreso")]
    [SerializeField] private string returnSceneName;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ReturnToPreviousScene();
        }
    }
    private void ReturnToPreviousScene()
    {
        if (string.IsNullOrEmpty(returnSceneName))
        {
            Debug.LogWarning("SceneBackController: No se indicó una escena de regreso.");
            return;
        }

        Debug.Log("Regresando a: " + returnSceneName);

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(returnSceneName);
        }
        else
        {
            Debug.LogWarning("SceneBackController: No existe SceneLoader. Cargando directamente la escena.");

            UnityEngine.SceneManagement.SceneManager.LoadScene(returnSceneName);
        }
    }
}