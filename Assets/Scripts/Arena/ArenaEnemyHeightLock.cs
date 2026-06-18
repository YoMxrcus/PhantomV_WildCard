using UnityEngine;

public class ArenaEnemyHeightLock : MonoBehaviour
{
    private Transform heightReference;
    private float heightOffset;

    public void Initialize(Transform reference, float offset)
    {
        heightReference = reference;
        heightOffset = offset;
        ApplyHeight();
    }

    private void LateUpdate()
    {
        ApplyHeight();
    }

    private void ApplyHeight()
    {
        if (heightReference == null)
        {
            return;
        }

        Vector3 position = transform.position;
        position.y = heightReference.position.y + heightOffset;
        transform.position = position;
    }
}
