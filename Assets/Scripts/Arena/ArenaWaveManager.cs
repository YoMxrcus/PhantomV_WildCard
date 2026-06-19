using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private GameObject[] postWaveFiveEnemyPrefabs;
    [SerializeField] private ArenaSpawnPoint[] spawnPoints;
    [SerializeField] private ArenaLootManager lootManager;
    [SerializeField] private ArenaOverheatManager overheatManager;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private bool movePlayerToSpawnOnStart = false;

    [Header("Wave Tuning")]
    [SerializeField] private bool startOnPlay = true;
    [SerializeField] private int firstWaveEnemyCount = 10;
    [SerializeField] private int enemiesAddedEachWave = 6;
    [SerializeField] private int maxAliveEnemies = 12;
    [SerializeField] private float timeBetweenSpawns = 1.25f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float spawnHeightOffset = -0.25f;
    [SerializeField] private int postWaveFiveEnemyStartWave = 1;
    [SerializeField] private float navMeshSpawnSampleRadius = 6f;

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
    public int TotalEnemiesRemaining => aliveEnemies.Count + enemiesRemainingToSpawn;

    private void Awake()
    {
        ResolveSceneReferences();
    }

    private void Start()
    {
        if (movePlayerToSpawnOnStart)
        {
            MovePlayerToArenaSpawn();
        }

        InitializeArenaSystems();
        UpdateUI();

        if (startOnPlay)
        {
            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
            }

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

    public List<Transform> GetAliveEnemyTransforms()
    {
        List<Transform> enemyTransforms = new List<Transform>();

        foreach (ArenaEnemyTracker tracker in aliveEnemies)
        {
            if (tracker != null)
            {
                enemyTransforms.Add(tracker.transform);
            }
        }

        return enemyTransforms;
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
                bool nextWaveIsOverheat = overheatManager != null && overheatManager.IsOverheatWave(currentWave + 1);
                lootManager.SpawnLootForCompletedWave(currentWave, nextWaveIsOverheat);
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

        GameObject prefab = GetEnemyPrefabForCurrentWave();
        ArenaSpawnPoint spawnPoint = GetSpawnPointForPrefab(prefab);
        Vector3 spawnPosition = spawnPoint.transform.position;

        if (player != null)
        {
            spawnPosition.y = player.transform.position.y + spawnHeightOffset;
        }

        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, navMeshSpawnSampleRadius, NavMesh.AllAreas))
        {
            spawnPosition = hit.position;
        }

        GameObject enemyInstance = Instantiate(prefab, spawnPosition, Quaternion.identity);
        ConfigureArenaEnemy(enemyInstance);

        ArenaEnemyTracker tracker = enemyInstance.GetComponent<ArenaEnemyTracker>();

        if (tracker == null)
        {
            tracker = enemyInstance.AddComponent<ArenaEnemyTracker>();
        }

        tracker.Initialize(this);
        aliveEnemies.Add(tracker);
    }

    private void ConfigureArenaEnemy(GameObject enemyInstance)
    {
        ArenaZombieEnemy zombieEnemy = enemyInstance.GetComponent<ArenaZombieEnemy>();

        if (zombieEnemy != null)
        {
            if (enemyInstance.GetComponent<ArenaEnemyTracker>() == null)
            {
                enemyInstance.AddComponent<ArenaEnemyTracker>();
            }

            return;
        }

        Enemy enemy = enemyInstance.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.DestroyOnBuildingContact = false;
        }

        if (enemyInstance.GetComponent<ArenaSnakeNavMeshEnemy>() == null)
        {
            enemyInstance.AddComponent<ArenaSnakeNavMeshEnemy>();
        }

        if (enemyInstance.GetComponent<ArenaEnemyTracker>() == null)
        {
            enemyInstance.AddComponent<ArenaEnemyTracker>();
        }

        return;
    }

    private GameObject GetEnemyPrefabForCurrentWave()
    {
        bool canSpawnPostWaveFiveEnemy = currentWave >= postWaveFiveEnemyStartWave
            && postWaveFiveEnemyPrefabs != null
            && postWaveFiveEnemyPrefabs.Length > 0;

        if (!canSpawnPostWaveFiveEnemy)
        {
            return enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        }

        int baseEnemyCount = enemyPrefabs != null ? enemyPrefabs.Length : 0;
        int postWaveEnemyCount = postWaveFiveEnemyPrefabs.Length;
        int totalEnemyOptions = baseEnemyCount + postWaveEnemyCount;
        int selectedIndex = Random.Range(0, totalEnemyOptions);

        if (selectedIndex < baseEnemyCount)
        {
            return enemyPrefabs[selectedIndex];
        }

        return postWaveFiveEnemyPrefabs[selectedIndex - baseEnemyCount];
    }

    private ArenaSpawnPoint GetSpawnPointForPrefab(GameObject prefab)
    {
        ArenaSpawnPoint.SpawnPointType spawnType = GetSpawnTypeForPrefab(prefab);
        List<ArenaSpawnPoint> matchingSpawnPoints = new List<ArenaSpawnPoint>();
        List<ArenaSpawnPoint> fallbackSpawnPoints = new List<ArenaSpawnPoint>();

        foreach (ArenaSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint == null)
            {
                continue;
            }

            if (spawnPoint.SpawnType == spawnType)
            {
                matchingSpawnPoints.Add(spawnPoint);
                continue;
            }

            if (spawnPoint.SpawnType == ArenaSpawnPoint.SpawnPointType.Any)
            {
                fallbackSpawnPoints.Add(spawnPoint);
            }
        }

        if (matchingSpawnPoints.Count > 0)
        {
            return matchingSpawnPoints[Random.Range(0, matchingSpawnPoints.Count)];
        }

        if (fallbackSpawnPoints.Count > 0)
        {
            Debug.LogWarning($"No exact arena spawn points found for {spawnType}. Falling back to Any arena spawn points.");
            return fallbackSpawnPoints[Random.Range(0, fallbackSpawnPoints.Count)];
        }

        Debug.LogWarning($"No arena spawn points found for {spawnType}. Falling back to any arena spawn point.");
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    private ArenaSpawnPoint.SpawnPointType GetSpawnTypeForPrefab(GameObject prefab)
    {
        if (prefab != null && prefab.GetComponent<ArenaZombieEnemy>() != null)
        {
            return ArenaSpawnPoint.SpawnPointType.Zombie;
        }

        return ArenaSpawnPoint.SpawnPointType.Snake;
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
