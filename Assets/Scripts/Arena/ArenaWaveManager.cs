using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArenaWaveManager : MonoBehaviour
{
    private enum ArenaState
    {
        WaitingToStart,
        Intermission,
        Spawning,
        ClearingWave,
        GameOver
    }

    [Header("Arena References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private ArenaSpawnPoint[] spawnPoints;
    [SerializeField] private ArenaLootManager lootManager;
    [SerializeField] private ArenaOverheatManager overheatManager;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject player;

    [Header("Wave Tuning")]
    [SerializeField] private bool startOnPlay = true;
    [SerializeField] private int firstWaveEnemyCount = 5;
    [SerializeField] private int enemiesAddedEachWave = 3;
    [SerializeField] private int maxAliveEnemies = 12;
    [SerializeField] private float timeBetweenSpawns = 1.25f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float spawnHeightOffset = -0.25f;

    [Header("Optional UI")]
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text enemiesText;
    [SerializeField] private TMP_Text stateText;

    [Header("Runtime Debug")]
    [SerializeField] private int inspectorCurrentWave;
    [SerializeField] private int inspectorAliveEnemies;
    [SerializeField] private int inspectorEnemiesRemainingToSpawn;
    [SerializeField] private int inspectorTotalEnemiesRemaining;
    [SerializeField] private bool inspectorIsOverheatWave;
    [SerializeField] private string inspectorState;

    private readonly HashSet<ArenaEnemyTracker> aliveEnemies = new HashSet<ArenaEnemyTracker>();
    private Coroutine arenaCoroutine;
    private ArenaState state = ArenaState.WaitingToStart;
    private int currentWave;
    private int enemiesRemainingToSpawn;
    private bool runActive;

    public int CurrentWave => currentWave;
    public int AliveEnemyCount => aliveEnemies.Count;
    public int EnemiesRemainingToSpawn => enemiesRemainingToSpawn;

    private void Awake()
    {
        ResolveSceneReferences();
    }

    private void Start()
    {
        MovePlayerToArenaSpawn();
        InitializeArenaSystems();
        UpdateUI();

        if (startOnPlay)
        {
            StartArena();
        }
    }

    public void StartArena()
    {
        if (arenaCoroutine != null)
        {
            StopCoroutine(arenaCoroutine);
        }

        ClearTrackedEnemies();
        runActive = true;
        currentWave = 0;
        arenaCoroutine = StartCoroutine(ArenaLoop());
    }

    public void StopArena()
    {
        runActive = false;
        state = ArenaState.GameOver;

        if (arenaCoroutine != null)
        {
            StopCoroutine(arenaCoroutine);
            arenaCoroutine = null;
        }

        UpdateUI();
    }

    public void NotifyEnemyRemoved(ArenaEnemyTracker tracker)
    {
        aliveEnemies.Remove(tracker);
        UpdateUI();
    }

    private IEnumerator ArenaLoop()
    {
        while (runActive)
        {
            currentWave++;
            enemiesRemainingToSpawn = GetEnemyCountForWave(currentWave);

            state = ArenaState.Intermission;
            UpdateUI();
            yield return new WaitForSeconds(timeBetweenWaves);

            state = ArenaState.Spawning;
            if (overheatManager != null)
            {
                overheatManager.BeginWave(currentWave);
            }
            UpdateUI();
            yield return StartCoroutine(SpawnCurrentWave());

            state = ArenaState.ClearingWave;
            UpdateUI();

            while (aliveEnemies.Count > 0)
            {
                yield return null;
            }

            if (overheatManager != null)
            {
                overheatManager.EndWave(currentWave);
            }

            if (lootManager != null)
            {
                lootManager.SpawnLootForCompletedWave(currentWave);
            }
        }
    }

    private IEnumerator SpawnCurrentWave()
    {
        while (enemiesRemainingToSpawn > 0)
        {
            while (aliveEnemies.Count >= maxAliveEnemies)
            {
                yield return null;
            }

            SpawnEnemy();
            enemiesRemainingToSpawn--;
            UpdateUI();

            if (enemiesRemainingToSpawn > 0)
            {
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("ArenaWaveManager needs at least one enemy prefab and one spawn point.");
            StopArena();
            return;
        }

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        ArenaSpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 spawnPosition = spawnPoint.transform.position;

        if (player != null)
        {
            spawnPosition.y = player.transform.position.y + spawnHeightOffset;
        }

        GameObject enemyInstance = Instantiate(prefab, spawnPosition, Quaternion.identity);
        ArenaEnemyTracker tracker = enemyInstance.GetComponent<ArenaEnemyTracker>();

        if (tracker == null)
        {
            tracker = enemyInstance.AddComponent<ArenaEnemyTracker>();
        }

        tracker.Initialize(this);
        aliveEnemies.Add(tracker);
    }

    private int GetEnemyCountForWave(int waveNumber)
    {
        int enemyCount = firstWaveEnemyCount + ((waveNumber - 1) * enemiesAddedEachWave);

        if (overheatManager != null)
        {
            enemyCount = overheatManager.GetEnemyCountForWave(enemyCount, waveNumber);
        }

        return Mathf.Max(1, enemyCount);
    }

    private void ResolveSceneReferences()
    {
        if (player == null)
        {
            player = GameObject.Find("ThirdPersonTEST");
        }

        if (player == null)
        {
            ThirdPersonController playerController = FindFirstObjectByType<ThirdPersonController>();
            player = playerController != null ? playerController.gameObject : null;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = FindObjectsByType<ArenaSpawnPoint>(FindObjectsSortMode.None);
        }

        if (lootManager == null)
        {
            lootManager = FindFirstObjectByType<ArenaLootManager>();
        }

        if (overheatManager == null)
        {
            overheatManager = FindFirstObjectByType<ArenaOverheatManager>();
        }
    }

    private void InitializeArenaSystems()
    {
        if (overheatManager == null)
        {
            return;
        }

        ThirdPersonController playerController = player != null ? player.GetComponent<ThirdPersonController>() : null;
        overheatManager.Initialize(playerController);
    }

    private void MovePlayerToArenaSpawn()
    {
        if (player == null || playerSpawnPoint == null)
        {
            return;
        }

        CharacterController characterController = player.GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        player.transform.SetPositionAndRotation(playerSpawnPoint.position, playerSpawnPoint.rotation);

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }

    private void ClearTrackedEnemies()
    {
        aliveEnemies.Clear();
    }

    private void UpdateUI()
    {
        inspectorCurrentWave = currentWave;
        inspectorAliveEnemies = aliveEnemies.Count;
        inspectorEnemiesRemainingToSpawn = enemiesRemainingToSpawn;
        inspectorTotalEnemiesRemaining = aliveEnemies.Count + enemiesRemainingToSpawn;
        inspectorIsOverheatWave = overheatManager != null && overheatManager.IsOverheatWave(currentWave);
        inspectorState = state.ToString();

        if (waveText != null)
        {
            waveText.text = $"Wave {currentWave}";
        }

        if (enemiesText != null)
        {
            enemiesText.text = $"Enemies {inspectorTotalEnemiesRemaining}";
        }

        if (stateText != null)
        {
            stateText.text = state.ToString();
        }
    }
}
