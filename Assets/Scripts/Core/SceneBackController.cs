using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneBackController : MonoBehaviour
{
    [Header("Escena de regreso")]
    [SerializeField] private string returnSceneName = "WorldPrototype";

    private void Update()
    {
        bool backPressed = false;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.backspaceKey.wasPressedThisFrame)
            {
                backPressed = true;
            }
        }
#if !ENABLE_INPUT_SYSTEM
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            backPressed = true;
        }
#endif

        if (backPressed)
        {
            ReturnToPreviousScene();
        }
    }

    public void ReturnToPreviousScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        string targetScene = returnSceneName;

        if (currentScene == "TownScene")
        {
            // Salir de la ciudad siempre regresa al mapa del mundo
            targetScene = "WorldPrototype";

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PreviousScene = "WorldPrototype";
            }
        }
        else if (currentScene == "MarketScene")
        {
            // El mercado regresa a la escena de donde vino (TownScene o WorldPrototype)
            if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.PreviousScene) && GameManager.Instance.PreviousScene != "MarketScene")
            {
                targetScene = GameManager.Instance.PreviousScene;
            }
            else
            {
                targetScene = !string.IsNullOrEmpty(returnSceneName) ? returnSceneName : "TownScene";
            }
        }
        else if (currentScene == "CombatScene")
        {
            targetScene = "WorldPrototype";
        }
        else if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.PreviousScene) && GameManager.Instance.PreviousScene != currentScene)
        {
            targetScene = GameManager.Instance.PreviousScene;
        }

        // Seguridad anti-bucle: si targetScene es la misma escena activa, volver al mapa del mundo
        if (string.IsNullOrEmpty(targetScene) || targetScene == currentScene)
        {
            targetScene = "WorldPrototype";
        }

        Debug.Log($"SceneBackController: Saliendo de '{currentScene}' -> Regresando a '{targetScene}'");

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("SceneBackController: No existe SceneLoader. Cargando directamente con SceneManager.");
            SceneManager.LoadScene(targetScene);
        }
    }
}