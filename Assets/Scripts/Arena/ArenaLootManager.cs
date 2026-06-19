using System.Collections.Generic;
using UnityEngine;

public class ArenaLootManager : MonoBehaviour
{
    [Header("Loot References")]
    [SerializeField] private GameObject lootBoxPrefab;
    [SerializeField] private ArenaLootSpawnPoint[] lootSpawnPoints;
    [SerializeField] private Texture condensorLootImage;

    [Header("Wave Rewards")]
    [SerializeField] private bool spawnLootAfterEveryWave = true;
    [SerializeField] private int boxesPerRewardWave = 1;
    [SerializeField] private int maxActiveLootBoxes = 6;
    [SerializeField] private int firstRewardWave = 1;
    [SerializeField] private int roundsPerExtraLootBox = 5;
    [SerializeField] private int maxBoxesPerRewardWave = 6;
    [SerializeField] private float spawnHeightOffset = 0f;
    [SerializeField] private float despawnDelayAfterLooted = 1.5f;
    [SerializeField] private float occupiedSpawnRadius = 1.25f;

    [Header("Arena Loot Pools")]
    [SerializeField] private List<string> standardArenaLoot = new List<string> { "Stim", "Grenade", "PistolAmmo", "AssaultAmmo", "ShotgunAmmo", "Bandage" };
    [SerializeField] private List<string> preOverheatLoot = new List<string> { "Condensor", "Condensor", "Stim", "PistolAmmo", "AssaultAmmo", "ShotgunAmmo", "Bandage" };

    [Header("Runtime Debug")]
    [SerializeField] private int inspectorActiveLootBoxes;
    [SerializeField] private int inspectorLastBoxesRequested;
    [SerializeField] private int inspectorLastBoxesSpawned;
    [SerializeField] private bool inspectorLastRewardPreparedForOverheat;

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

    public bool SpawnLootForCompletedWave(int completedWave, bool nextWaveIsOverheat = false)
    {
        RemoveMissingOrLootedBoxes();
        inspectorLastRewardPreparedForOverheat = nextWaveIsOverheat;

        if (!spawnLootAfterEveryWave || completedWave < firstRewardWave)
        {
            UpdateInspectorDebug();
            return false;
        }

        int desiredBoxes = GetBoxesForCompletedWave(completedWave);
        int availableSlots = Mathf.Max(0, maxActiveLootBoxes - activeLootBoxes.Count);
        int boxesToSpawn = Mathf.Min(desiredBoxes, availableSlots);
        int spawnedBoxCount = 0;
        bool spawnedLoot = false;

        for (int i = 0; i < boxesToSpawn; i++)
        {
            if (SpawnLootBox(nextWaveIsOverheat))
            {
                spawnedBoxCount++;
                spawnedLoot = true;
            }
        }

        inspectorLastBoxesRequested = desiredBoxes;
        inspectorLastBoxesSpawned = spawnedBoxCount;
        UpdateInspectorDebug();
        return spawnedLoot;
    }

    private int GetBoxesForCompletedWave(int completedWave)
    {
        int extraBoxes = roundsPerExtraLootBox > 0 ? completedWave / roundsPerExtraLootBox : 0;
        int boxesForWave = boxesPerRewardWave + extraBoxes;
        return Mathf.Clamp(boxesForWave, 1, maxBoxesPerRewardWave);
    }

    private bool SpawnLootBox(bool usePreOverheatLoot)
    {
        if (lootBoxPrefab == null || lootSpawnPoints == null || lootSpawnPoints.Length == 0)
        {
            Debug.LogWarning("ArenaLootManager needs a loot box prefab and at least one loot spawn point.");
            return false;
        }

        ArenaLootSpawnPoint spawnPoint = GetRandomOpenSpawnPoint();
        Vector3 spawnPosition = spawnPoint.transform.position + new Vector3(0f, spawnHeightOffset, 0f);
        GameObject lootBox = Instantiate(lootBoxPrefab, spawnPosition, spawnPoint.transform.rotation);
        ConfigureArenaLootBox(lootBox, usePreOverheatLoot);
        activeLootBoxes.Add(lootBox);
        return true;
    }

    private void ConfigureArenaLootBox(GameObject lootBox, bool usePreOverheatLoot)
    {
        LootBox lootBoxScript = lootBox.GetComponent<LootBox>();

        if (lootBoxScript == null)
        {
            return;
        }

        lootBoxScript.SetLootPool(usePreOverheatLoot ? preOverheatLoot : standardArenaLoot);
        lootBoxScript.ForceMaximumLootCount = true;

        if (condensorLootImage != null)
        {
            lootBoxScript.CondensorIMAGE = condensorLootImage;
        }
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
