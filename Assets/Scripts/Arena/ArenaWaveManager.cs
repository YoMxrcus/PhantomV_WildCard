using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    [SerializeField] private TMP_Text overheatNoticeText;
    [SerializeField] private TMP_Text lootNoticeText;
    [SerializeField] private float overheatNoticeSeconds = 3f;
    [SerializeField] private float lootNoticeSeconds = 3f;

    [Header("Optional Notification Audio")]
    [SerializeField] private AudioClip overheatNoticeSound;
    [SerializeField] private AudioClip lootNoticeSound;
    [SerializeField] private AudioSource overheatNoticeAudioSource;
    [SerializeField] private AudioSource lootNoticeAudioSource;
    [SerializeField, Range(0f, 1f)] private float notificationVolume = 1f;

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
    private Coroutine overheatNoticeCoroutine;
    private Coroutine lootNoticeCoroutine;

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
        InitializeArenaNotifications();
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

        HideArenaNotifications();
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

                if (overheatManager.IsOverheatActive)
                {
                    ShowOverheatNotice();
                }
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
                bool spawnedLoot = lootManager.SpawnLootForCompletedWave(currentWave, nextWaveIsOverheat);

                if (spawnedLoot)
                {
                    ShowLootNotice();
                }
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

    private void InitializeArenaNotifications()
    {
        if (overheatNoticeText == null)
        {
            overheatNoticeText = CreateNotificationText("ArenaOverheatNoticeText", new Vector2(0f, 130f), new Color(1f, 0.15f, 0.05f, 1f), 42f);
        }

        if (lootNoticeText == null)
        {
            lootNoticeText = CreateNotificationText("ArenaLootNoticeText", new Vector2(0f, 82f), new Color(1f, 0.9f, 0.25f, 1f), 34f);
        }

        HideNotification(overheatNoticeText);
        HideNotification(lootNoticeText);
        InitializeNotificationAudio();
    }

    private void InitializeNotificationAudio()
    {
        overheatNoticeAudioSource = ResolveNotificationAudioSource(overheatNoticeAudioSource, "ArenaOverheatNoticeAudioSource");
        lootNoticeAudioSource = ResolveNotificationAudioSource(lootNoticeAudioSource, "ArenaLootNoticeAudioSource");
    }

    private AudioSource ResolveNotificationAudioSource(AudioSource audioSource, string objectName)
    {
        if (audioSource != null)
        {
            ConfigureNotificationAudioSource(audioSource);
            return audioSource;
        }

        GameObject existingObject = GameObject.Find(objectName);

        if (existingObject != null && existingObject.TryGetComponent(out AudioSource existingSource))
        {
            ConfigureNotificationAudioSource(existingSource);
            return existingSource;
        }

        GameObject audioObject = new GameObject(objectName);
        AudioSource newSource = audioObject.AddComponent<AudioSource>();
        ConfigureNotificationAudioSource(newSource);
        return newSource;
    }

    private void ConfigureNotificationAudioSource(AudioSource audioSource)
    {
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = notificationVolume;
    }

    private TMP_Text CreateNotificationText(string objectName, Vector2 anchoredPosition, Color color, float fontSize)
    {
        GameObject existingObject = GameObject.Find(objectName);

        if (existingObject != null && existingObject.TryGetComponent(out TMP_Text existingText))
        {
            return existingText;
        }

        GameObject canvasObject = GameObject.Find("ArenaNotificationCanvas");
        Canvas canvas = canvasObject != null ? canvasObject.GetComponent<Canvas>() : null;

        if (canvas == null)
        {
            canvasObject = new GameObject("ArenaNotificationCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(canvas.transform, false);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.Center;
        text.color = color;
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.raycastTarget = false;

        RectTransform rectTransform = text.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(900f, 80f);

        return text;
    }

    private void ShowOverheatNotice()
    {
        ShowNotification(overheatNoticeText, "OVERHEAT FIND SHELTER", overheatNoticeSeconds, ref overheatNoticeCoroutine, overheatNoticeSound, overheatNoticeAudioSource);
    }

    private void ShowLootNotice()
    {
        ShowNotification(lootNoticeText, "NEW LOOT AVAILABLE", lootNoticeSeconds, ref lootNoticeCoroutine, lootNoticeSound, lootNoticeAudioSource);
    }

    private void ShowNotification(TMP_Text targetText, string message, float seconds, ref Coroutine activeCoroutine, AudioClip sound, AudioSource audioSource)
    {
        if (targetText == null)
        {
            return;
        }

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(NotificationRoutine(targetText, message, seconds, sound, audioSource));
    }

    private IEnumerator NotificationRoutine(TMP_Text targetText, string message, float seconds, AudioClip sound, AudioSource audioSource)
    {
        targetText.text = message;
        targetText.gameObject.SetActive(true);
        PlayNotificationSound(sound, audioSource);

        yield return new WaitForSeconds(seconds);

        HideNotification(targetText);
        StopNotificationSound(audioSource);
    }

    private void PlayNotificationSound(AudioClip sound, AudioSource audioSource)
    {
        if (sound == null || audioSource == null)
        {
            return;
        }

        audioSource.Stop();
        audioSource.clip = sound;
        audioSource.volume = notificationVolume;
        audioSource.Play();
    }

    private void StopNotificationSound(AudioSource audioSource)
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            return;
        }

        audioSource.Stop();
    }

    private void HideArenaNotifications()
    {
        if (overheatNoticeCoroutine != null)
        {
            StopCoroutine(overheatNoticeCoroutine);
            overheatNoticeCoroutine = null;
        }

        if (lootNoticeCoroutine != null)
        {
            StopCoroutine(lootNoticeCoroutine);
            lootNoticeCoroutine = null;
        }

        HideNotification(overheatNoticeText);
        HideNotification(lootNoticeText);
        StopNotificationSound(overheatNoticeAudioSource);
        StopNotificationSound(lootNoticeAudioSource);
    }

    private void HideNotification(TMP_Text targetText)
    {
        if (targetText == null)
        {
            return;
        }

        targetText.text = string.Empty;
        targetText.gameObject.SetActive(false);
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
