using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MerchantCompanyController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 8f;
    public float arrivalDistance = 5f;
    public float rotationSpeed = 10f;

    [Header("Tiempo de espera en ciudades")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;

    [Header("Interacción con el jugador")]
    public string merchantName = "Caravana Mercante";
    public string marketSceneName = "MarketScene";
    public float interactionDistance = 4f;

    [Header("Ciudades destino (Opcional - se detectan automáticamente si está vacío)")]
    public List<Transform> townTargets = new List<Transform>();

    private Transform currentTarget;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private Transform playerCompany;
    private bool playerInRange = false;
    private bool isEnteringMarket = false;

    private void Start()
    {
        InitializeTowns();
        SelectNewTargetTown();
        FindPlayerCompany();
    }

    private void Update()
    {
        HandlePlayerProximity();
        HandlePlayerInput();

        if (townTargets == null || townTargets.Count == 0)
        {
            InitializeTowns();
            if (townTargets == null || townTargets.Count == 0)
                return;
        }

        if (isWaiting)
        {
            HandleWaiting();
        }
        else
        {
            HandleMovement();
        }
    }

    private void FindPlayerCompany()
    {
        if (playerCompany == null)
        {
            CompanyController player = FindFirstObjectByType<CompanyController>();
            if (player != null)
            {
                playerCompany = player.transform;
            }
        }
    }

    private void HandlePlayerProximity()
    {
        if (playerCompany == null)
        {
            FindPlayerCompany();
            if (playerCompany == null)
                return;
        }

        Vector3 myPos = transform.position;
        Vector3 playerPos = playerCompany.position;
        myPos.y = 0;
        playerPos.y = 0;

        float dist = Vector3.Distance(myPos, playerPos);

        if (dist <= interactionDistance)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                isEnteringMarket = false;

                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowTownPanel(merchantName);
                }
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                isEnteringMarket = false;

                if (UIManager.Instance != null)
                {
                    UIManager.Instance.HideTownPanel();
                }
            }
        }
    }

    private void HandlePlayerInput()
    {
        if (!playerInRange || isEnteringMarket)
            return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            isEnteringMarket = true;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideTownPanel();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.EnterMarket(merchantName, marketSceneName);
            }
            else if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene(marketSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(marketSceneName);
            }
        }
    }

    private void InitializeTowns()
    {
        if (townTargets == null)
        {
            townTargets = new List<Transform>();
        }

        if (townTargets.Count == 0)
        {
            TownController[] towns = FindObjectsByType<TownController>(FindObjectsSortMode.None);
            if (towns == null || towns.Length == 0)
            {
#pragma warning disable CS0618
                towns = FindObjectsOfType<TownController>();
#pragma warning restore CS0618
            }

            if (towns != null)
            {
                foreach (TownController town in towns)
                {
                    if (town != null && !townTargets.Contains(town.transform))
                    {
                        townTargets.Add(town.transform);
                    }
                }
            }

            Debug.Log($"MerchantCompanyController: {townTargets.Count} ciudades detectadas para comerciar.");
        }
    }

    private void HandleMovement()
    {
        if (currentTarget == null)
        {
            SelectNewTargetTown();
            if (currentTarget == null)
                return;
        }

        Vector3 targetPos = currentTarget.position;
        Vector3 currentPos = transform.position;

        // Mantener altura Y fija
        targetPos.y = currentPos.y;

        Vector3 direction = targetPos - currentPos;
        float distance = direction.magnitude;

        if (distance <= arrivalDistance)
        {
            // Llegó a la ciudad
            isWaiting = true;
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
            Debug.Log($"MerchantCompany: Llegó a {currentTarget.name}. Comerciando durante {waitTimer:F1}s...");
            return;
        }

        Vector3 moveDir = direction.normalized;

        // Rotación suave hacia el objetivo
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Movimiento
        transform.position += moveDir * speed * Time.deltaTime;
    }

    private void HandleWaiting()
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
        {
            isWaiting = false;
            SelectNewTargetTown();
        }
    }

    private void SelectNewTargetTown()
    {
        if (townTargets == null || townTargets.Count == 0)
        {
            InitializeTowns();
            if (townTargets == null || townTargets.Count == 0)
                return;
        }

        if (townTargets.Count == 1)
        {
            currentTarget = townTargets[0];
            return;
        }

        List<Transform> availableTargets = new List<Transform>();
        foreach (Transform t in townTargets)
        {
            if (t != null && t != currentTarget)
            {
                availableTargets.Add(t);
            }
        }

        if (availableTargets.Count > 0)
        {
            int randomIndex = Random.Range(0, availableTargets.Count);
            currentTarget = availableTargets[randomIndex];
            Debug.Log($"MerchantCompany: Nuevo destino comercial seleccionado -> {currentTarget.name}");
        }
        else if (townTargets.Count > 0 && townTargets[0] != null)
        {
            currentTarget = townTargets[0];
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        if (currentTarget != null)
        {
            Gizmos.DrawLine(transform.position, currentTarget.position);
            Gizmos.DrawWireSphere(currentTarget.position, arrivalDistance);
        }
    }
}
