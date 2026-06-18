using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArenaEnemyDirectionIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ArenaWaveManager waveManager;
    [SerializeField] private Transform player;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private RectTransform arrow;

    [Header("Indicator Rules")]
    [SerializeField] private int showWhenEnemiesAtOrBelow = 5;
    [SerializeField] private int hideWhenEnemiesAtOrBelow = 0;
    [SerializeField] private float screenEdgePadding = 70f;
    [SerializeField] private bool pointToNearestEnemy = true;

    private Canvas canvas;

    private void Awake()
    {
        ResolveReferences();
        CreateDefaultArrowIfNeeded();
        SetArrowVisible(false);
    }

    private void LateUpdate()
    {
        ResolveReferences();

        if (waveManager == null || player == null || targetCamera == null || arrow == null)
        {
            SetArrowVisible(false);
            return;
        }

        int remainingEnemies = waveManager.TotalEnemiesRemaining;

        if (remainingEnemies <= hideWhenEnemiesAtOrBelow || remainingEnemies > showWhenEnemiesAtOrBelow || waveManager.EnemiesRemainingToSpawn > 0)
        {
            SetArrowVisible(false);
            return;
        }

        Transform target = GetIndicatorTarget();

        if (target == null)
        {
            SetArrowVisible(false);
            return;
        }

        UpdateArrow(target.position);
        SetArrowVisible(true);
    }

    private void ResolveReferences()
    {
        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<ArenaWaveManager>();
        }

        if (player == null)
        {
            ThirdPersonController playerController = FindFirstObjectByType<ThirdPersonController>();
            player = playerController != null ? playerController.transform : null;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
    }

    private void CreateDefaultArrowIfNeeded()
    {
        if (arrow != null)
        {
            return;
        }

        Canvas parentCanvas = canvas;

        if (parentCanvas == null)
        {
            GameObject canvasObject = new GameObject("ArenaEnemyIndicatorCanvas");
            parentCanvas = canvasObject.AddComponent<Canvas>();
            parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            canvas = parentCanvas;
        }

        GameObject arrowObject = new GameObject("ArenaEnemyDirectionArrow");
        arrowObject.transform.SetParent(parentCanvas.transform, false);
        arrow = arrowObject.AddComponent<RectTransform>();
        arrow.sizeDelta = new Vector2(54f, 54f);

        TextMeshProUGUI arrowText = arrowObject.AddComponent<TextMeshProUGUI>();
        arrowText.text = ">";
        arrowText.fontSize = 54f;
        arrowText.alignment = TextAlignmentOptions.Center;
        arrowText.color = new Color(1f, 0.2f, 0.1f, 0.9f);
        arrowText.raycastTarget = false;
    }

    private Transform GetIndicatorTarget()
    {
        List<Transform> enemies = waveManager.GetAliveEnemyTransforms();

        if (enemies.Count == 0)
        {
            return null;
        }

        if (!pointToNearestEnemy)
        {
            return enemies[0];
        }

        Transform nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            float distance = (enemy.position - player.position).sqrMagnitude;

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private void UpdateArrow(Vector3 targetWorldPosition)
    {
        Vector3 screenPosition = targetCamera.WorldToScreenPoint(targetWorldPosition);

        if (screenPosition.z < 0f)
        {
            screenPosition *= -1f;
        }

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 fromCenter = new Vector2(screenPosition.x, screenPosition.y) - screenCenter;

        if (fromCenter.sqrMagnitude < 0.001f)
        {
            fromCenter = Vector2.up;
        }

        Vector2 clampedPosition = screenCenter + fromCenter.normalized * GetEdgeDistance(fromCenter.normalized);
        arrow.position = clampedPosition;

        float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
        arrow.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private float GetEdgeDistance(Vector2 direction)
    {
        float horizontalDistance = (Screen.width * 0.5f - screenEdgePadding) / Mathf.Max(Mathf.Abs(direction.x), 0.001f);
        float verticalDistance = (Screen.height * 0.5f - screenEdgePadding) / Mathf.Max(Mathf.Abs(direction.y), 0.001f);
        return Mathf.Min(horizontalDistance, verticalDistance);
    }

    private void SetArrowVisible(bool visible)
    {
        if (arrow != null && arrow.gameObject.activeSelf != visible)
        {
            arrow.gameObject.SetActive(visible);
        }
    }
}
