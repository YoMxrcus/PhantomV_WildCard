using UnityEngine;

public class ArenaLootSpawnPoint : MonoBehaviour
{
    [SerializeField] private float gizmoRadius = 0.3f;
    [SerializeField] private Color gizmoColor = new Color(0.1f, 0.75f, 1f, 0.8f);

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, Vector3.one * gizmoRadius);
        Gizmos.DrawWireCube(transform.position, Vector3.one * gizmoRadius * 1.8f);
    }
}
