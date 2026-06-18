using UnityEngine;

public class ArenaEnemyBuildingBlocker : MonoBehaviour
{
    [SerializeField] private float pushStrength = 4f;
    [SerializeField] private float minimumPushDistance = 0.25f;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Building"))
        {
            return;
        }

        Vector3 closestPoint = other.ClosestPoint(transform.position);
        Vector3 pushDirection = transform.position - closestPoint;
        pushDirection.y = 0f;

        if (pushDirection.sqrMagnitude < 0.001f)
        {
            pushDirection = transform.position - other.transform.position;
            pushDirection.y = 0f;
        }

        if (pushDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        float pushDistance = Mathf.Max(minimumPushDistance, pushStrength * Time.deltaTime);
        transform.position += pushDirection.normalized * pushDistance;
    }
}
