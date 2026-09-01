using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string CurrentTown { get; private set; }
    public string PreviousScene { get; set; } = "WorldPrototype";

    // Posición de la compañía en el mundo antes de entrar a Town o Market.
    public Vector3 CompanyWorldPosition { get; private set; }

    public bool HasCompanyWorldPosition { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void EnterTown(string townName)
    {
        CurrentTown = townName;
        PreviousScene = "WorldPrototype";

        SaveCompanyPosition();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMessage("Entrando en " + CurrentTown);
        }

        Debug.Log("Entrando en " + CurrentTown);

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("No existe un SceneLoader.");
            return;
        }

        SceneLoader.Instance.LoadScene("TownScene");
    }

    public void EnterMarket(string merchantName = "Caravana Mercante", string sceneName = "MarketScene")
    {
        PreviousScene = "WorldPrototype";

        SaveCompanyPosition();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMessage("Comerciando con " + merchantName);
        }

        Debug.Log("Comerciando con " + merchantName);

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("No existe un SceneLoader.");
            return;
        }

        SceneLoader.Instance.LoadScene(sceneName);
    }

    public void EnterCombat(string encounterName = "Banda de Bandidos", string sceneName = "CombatScene")
    {
        PreviousScene = "WorldPrototype";

        SaveCompanyPosition();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMessage("¡Iniciando combate contra " + encounterName + "!");
        }

        Debug.Log("Iniciando combate contra " + encounterName);

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("No existe un SceneLoader.");
            return;
        }

        SceneLoader.Instance.LoadScene(sceneName);
    }

    private void SaveCompanyPosition()
    {
        CompanyController company = FindFirstObjectByType<CompanyController>();

        if (company != null)
        {
            CompanyWorldPosition = company.transform.position;
            HasCompanyWorldPosition = true;

            Debug.Log("Posición de compañía guardada: " + CompanyWorldPosition);
        }
        else
        {
            Debug.LogWarning("GameManager: No se encontró la compañía antes de entrar.");
        }
    }

    public void ExitTown()
    {
        UIManager.Instance.ShowMessage("Explorando el mundo");

        Debug.Log("Saliendo de " + CurrentTown);

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("No existe un SceneLoader.");
            return;
        }

        SceneLoader.Instance.LoadScene("WorldPrototype");
    }
}