using System.Collections.Generic;
using UnityEngine;

public class ArenaLootManager : MonoBehaviour
{
    [Header("Loot References")]
    [SerializeField] private GameObject lootBoxPrefab;
    [SerializeField] private ArenaLootSpawnPoint[] lootSpawnPoints;

    [Header("Wave Rewards")]
    [SerializeField] private bool spawnLootAfterEveryWave = true;
    [SerializeField] private int boxesPerRewardWave = 1;
    [SerializeField] private int maxActiveLootBoxes = 3;
    [SerializeField] private int firstRewardWave = 1;
    [SerializeField] private float spawnHeightOffset = 0f;
    [SerializeField] private float despawnDelayAfterLooted = 1.5f;
    [SerializeField] private float occupiedSpawnRadius = 1.25f;

    [Header("Runtime Debug")]
    [SerializeField] private int inspectorActiveLootBoxes;

    private readonly List<GameObject> activeLootBoxes = new List<GameObject>();

    private void Update()
    {
        UpdateInspectorDebug();
    }

    private void Awake()
    {
        if (lootSpawnPoints == null || lootSpawnPoints.Length == 0)
        {
            lootSpawnPoints = FindObjectsByType<ArenaLootSpawnPoint>(FindObjectsSortMode.None);
        }
    }

    public void SpawnLootForCompletedWave(int completedWave)
    {
        RemoveMissingOrLootedBoxes();

        if (!spawnLootAfterEveryWave || completedWave < firstRewardWave)
        {
            UpdateInspectorDebug();
            return;
        }

        int availableSlots = Mathf.Max(0, maxActiveLootBoxes - activeLootBoxes.Count);
        int boxesToSpawn = Mathf.Min(boxesPerRewardWave, availableSlots);

        for (int i = 0; i < boxesToSpawn; i++)
        {
            SpawnLootBox();
        }

        UpdateInspectorDebug();
    }

    private void SpawnLootBox()
    {
        if (lootBoxPrefab == null || lootSpawnPoints == null || lootSpawnPoints.Length == 0)
        {
            Debug.LogWarning("ArenaLootManager needs a loot box prefab and at least one loot spawn point.");
            return;
        }

        ArenaLootSpawnPoint spawnPoint = GetRandomOpenSpawnPoint();
        Vector3 spawnPosition = spawnPoint.transform.position + new Vector3(0f, spawnHeightOffset, 0f);
        GameObject lootBox = Instantiate(lootBoxPrefab, spawnPosition, spawnPoint.transform.rotation);
        activeLootBoxes.Add(lootBox);
    }

    private ArenaLootSpawnPoint GetRandomOpenSpawnPoint()
    {
        List<ArenaLootSpawnPoint> openSpawnPoints = new List<ArenaLootSpawnPoint>();

        foreach (ArenaLootSpawnPoint spawnPoint in lootSpawnPoints)
        {
            if (!IsSpawnPointOccupied(spawnPoint))
            {
                openSpawnPoints.Add(spawnPoint);
            }
        }

        if (openSpawnPoints.Count > 0)
        {
            return openSpawnPoints[Random.Range(0, openSpawnPoints.Count)];
        }

        return lootSpawnPoints[Random.Range(0, lootSpawnPoints.Length)];
    }

    private bool IsSpawnPointOccupied(ArenaLootSpawnPoint spawnPoint)
    {
        foreach (GameObject lootBox in activeLootBoxes)
        {
            if (lootBox == null)
            {
                continue;
            }

            float distance = Vector3.Distance(spawnPoint.transform.position, lootBox.transform.position);

            if (distance <= occupiedSpawnRadius)
            {
                return true;
            }
        }

        return false;
    }

    private void RemoveMissingOrLootedBoxes()
    {
        for (int i = activeLootBoxes.Count - 1; i >= 0; i--)
        {
            GameObject lootBox = activeLootBoxes[i];

            if (lootBox == null)
            {
                activeLootBoxes.RemoveAt(i);
                continue;
            }

            LootBox lootBoxScript = lootBox.GetComponent<LootBox>();

            if (lootBoxScript != null && lootBoxScript.IsLooted)
            {
                Destroy(lootBox, despawnDelayAfterLooted);
                activeLootBoxes.RemoveAt(i);
            }
        }
    }

    private void UpdateInspectorDebug()
    {
        RemoveMissingOrLootedBoxes();
        inspectorActiveLootBoxes = activeLootBoxes.Count;
    }
}
