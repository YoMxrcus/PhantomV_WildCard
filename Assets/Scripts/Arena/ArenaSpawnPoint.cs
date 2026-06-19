using UnityEngine;

public class ArenaSpawnPoint : MonoBehaviour
{
    public enum SpawnPointType
    {
        Any,
        Snake,
        Zombie
    }

    [SerializeField] private SpawnPointType spawnType = SpawnPointType.Any;
    [SerializeField] private float gizmoRadius = 0.35f;
    [SerializeField] private Color gizmoColor = new Color(1f, 0.35f, 0.1f, 0.8f);

    public SpawnPointType SpawnType => spawnType;

    public bool CanSpawn(ArenaSpawnPoint.SpawnPointType enemySpawnType)
    {
        return spawnType == SpawnPointType.Any || spawnType == enemySpawnType;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GetGizmoColor();
        Gizmos.DrawSphere(transform.position, gizmoRadius);
        Gizmos.DrawWireSphere(transform.position, gizmoRadius * 1.75f);
    }

    private Color GetGizmoColor()
    {
        switch (spawnType)
        {
            case SpawnPointType.Snake:
                return new Color(0.45f, 0.95f, 0.25f, 0.85f);

            case SpawnPointType.Zombie:
                return new Color(0.25f, 0.75f, 1f, 0.85f);

            default:
                return gizmoColor;
        }
    }
}
