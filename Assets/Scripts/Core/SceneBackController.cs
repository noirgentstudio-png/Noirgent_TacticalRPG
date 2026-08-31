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
        string targetScene = returnSceneName;

        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.PreviousScene))
        {
            targetScene = GameManager.Instance.PreviousScene;
        }

        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning("SceneBackController: No se indicó una escena de regreso.");
            return;
        }

        Debug.Log("Regresando a: " + targetScene);

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("SceneBackController: No existe SceneLoader. Cargando directamente la escena.");

            UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene);
        }
    }
}