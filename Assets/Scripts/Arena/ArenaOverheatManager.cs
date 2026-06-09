using UnityEngine;

public class ArenaOverheatManager : MonoBehaviour
{
    [Header("Wave Timing")]
    [SerializeField] private int firstOverheatWave = 5;
    [SerializeField] private int overheatEveryWaves = 5;
    [SerializeField] private float overheatEnemyCountMultiplier = 0.75f;

    [Header("Arena Heat Rules")]
    [SerializeField] private int startingHeat = 25;
    [SerializeField] private float heatPerSecond = 8f;
    [SerializeField] private float condensorCoolingPerStack = 5f;
    [SerializeField] private float condensorStackDuration = 8f;
    [SerializeField] private float damageThreshold = 90f;
    [SerializeField] private float damagePerSecond = 18f;
    [SerializeField] private bool clearHeatAfterOverheatWave = true;

    [Header("Runtime Debug")]
    [SerializeField] private bool inspectorOverheatActive;
    [SerializeField] private int inspectorActiveOverheatWave;

    private ThirdPersonController player;

    public bool IsOverheatActive => inspectorOverheatActive;

    public void Initialize(ThirdPersonController arenaPlayer)
    {
        player = arenaPlayer;

        if (player == null)
        {
            player = FindFirstObjectByType<ThirdPersonController>();
        }

        if (player != null)
        {
            player.EnableArenaHeatOverride();
        }
    }

    public bool IsOverheatWave(int waveNumber)
    {
        if (overheatEveryWaves <= 0 || waveNumber < firstOverheatWave)
        {
            return false;
        }

        return (waveNumber - firstOverheatWave) % overheatEveryWaves == 0;
    }

    public int GetEnemyCountForWave(int normalEnemyCount, int waveNumber)
    {
        if (!IsOverheatWave(waveNumber))
        {
            return normalEnemyCount;
        }

        return Mathf.Max(1, Mathf.RoundToInt(normalEnemyCount * overheatEnemyCountMultiplier));
    }

    public void BeginWave(int waveNumber)
    {
        if (player == null)
        {
            Initialize(null);
        }

        if (player == null)
        {
            Debug.LogWarning("ArenaOverheatManager could not find a ThirdPersonController.");
            return;
        }

        if (!IsOverheatWave(waveNumber))
        {
            inspectorOverheatActive = false;
            inspectorActiveOverheatWave = 0;
            player.EnableArenaHeatOverride();
            player.EndArenaOverheat(true);
            return;
        }

        inspectorOverheatActive = true;
        inspectorActiveOverheatWave = waveNumber;
        player.BeginArenaOverheat(heatPerSecond, condensorCoolingPerStack, condensorStackDuration, damageThreshold, damagePerSecond, startingHeat);
    }

    public void EndWave(int waveNumber)
    {
        if (player == null)
        {
            return;
        }

        if (IsOverheatWave(waveNumber))
        {
            player.EndArenaOverheat(clearHeatAfterOverheatWave);
        }

        inspectorOverheatActive = false;
        inspectorActiveOverheatWave = 0;
    }
}
