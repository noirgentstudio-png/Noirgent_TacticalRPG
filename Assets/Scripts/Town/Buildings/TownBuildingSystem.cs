using UnityEngine;

public class TownBuildingSystem : MonoBehaviour
{
    public static TownBuildingSystem Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void EnterBuilding(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("TownBuildingSystem: No se indicó una escena.");
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PreviousScene = "TownScene";
        }

        Debug.Log("Entrando al edificio: " + sceneName);

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("No existe un SceneLoader.");
            return;
        }

        SceneLoader.Instance.LoadScene(sceneName);
    }
}