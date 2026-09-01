using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyCompanyController : MonoBehaviour
{
    [Header("Movimiento Hostil")]
    public float speed = 5f;
    public float waypointArrivalDistance = 4f;
    public float rotationSpeed = 8f;

    [Header("Área de Patrullaje")]
    public float patrolRadius = 25f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 6f;

    [Header("Interacción y Combate")]
    public string enemyCompanyName = "Banda de Forajidos";
    public string combatSceneName = "CombatScene";
    public float encounterDistance = 4f;

    private Vector3 currentTargetPoint;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private Vector3 spawnOrigin;

    private Transform playerCompany;
    private bool playerInRange = false;
    private bool isEnteringCombat = false;

    private void Start()
    {
        spawnOrigin = transform.position;
        SelectNewPatrolPoint();
        FindPlayerCompany();
    }

    private void Update()
    {
        HandlePlayerProximity();
        HandlePlayerInput();

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

        if (dist <= encounterDistance)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                isEnteringCombat = false;

                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowTownPanel($"{enemyCompanyName}\n[E] Iniciar Batalla");
                }
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                isEnteringCombat = false;

                if (UIManager.Instance != null)
                {
                    UIManager.Instance.HideTownPanel();
                }
            }
        }
    }

    private void HandlePlayerInput()
    {
        if (!playerInRange || isEnteringCombat)
            return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            isEnteringCombat = true;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideTownPanel();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.EnterCombat(enemyCompanyName, combatSceneName);
            }
            else if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene(combatSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(combatSceneName);
            }
        }
    }

    private void HandleMovement()
    {
        Vector3 myPos = transform.position;
        Vector3 targetPos = currentTargetPoint;
        targetPos.y = myPos.y;

        Vector3 direction = targetPos - myPos;
        float distance = direction.magnitude;

        if (distance <= waypointArrivalDistance)
        {
            isWaiting = true;
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
            return;
        }

        Vector3 moveDir = direction.normalized;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        transform.position += moveDir * speed * Time.deltaTime;
    }

    private void HandleWaiting()
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
        {
            isWaiting = false;
            SelectNewPatrolPoint();
        }
    }

    private void SelectNewPatrolPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        currentTargetPoint = new Vector3(spawnOrigin.x + randomCircle.x, spawnOrigin.y, spawnOrigin.z + randomCircle.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, encounterDistance);
        Gizmos.DrawWireSphere(spawnOrigin, patrolRadius);
        Gizmos.DrawLine(transform.position, currentTargetPoint);
    }
}

