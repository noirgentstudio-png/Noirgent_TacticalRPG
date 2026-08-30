using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string CurrentTown { get; private set; }

    // Posición de la compañía en el mundo antes de entrar a Town.
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

        // Guardamos la posición actual de la compañía
        // antes de abandonar WorldPrototype.
        CompanyController company = FindFirstObjectByType<CompanyController>();

        if (company != null)
        {
            CompanyWorldPosition = company.transform.position;
            HasCompanyWorldPosition = true;

            Debug.Log("Posición de compañía guardada: " + CompanyWorldPosition);
        }
        else
        {
            Debug.LogWarning("GameManager: No se encontró la compañía antes de entrar a Town.");
        }

        UIManager.Instance.ShowMessage("Entrando en " + CurrentTown);

        Debug.Log("Entrando en " + CurrentTown);

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("No existe un SceneLoader.");
            return;
        }

        SceneLoader.Instance.LoadScene("TownScene");
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