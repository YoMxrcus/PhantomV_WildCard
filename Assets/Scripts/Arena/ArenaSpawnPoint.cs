using UnityEngine;

public class ArenaSpawnPoint : MonoBehaviour
{
    [SerializeField] private float gizmoRadius = 0.35f;
    [SerializeField] private Color gizmoColor = new Color(1f, 0.35f, 0.1f, 0.8f);

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoRadius);
        Gizmos.DrawWireSphere(transform.position, gizmoRadius * 1.75f);
    }
}
