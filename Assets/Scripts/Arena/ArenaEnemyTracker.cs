using UnityEngine;

public class ArenaEnemyTracker : MonoBehaviour
{
    private ArenaWaveManager arena;
    private bool isRegistered;

    public void Initialize(ArenaWaveManager owningArena)
    {
        arena = owningArena;
        isRegistered = arena != null;
    }

    private void OnDestroy()
    {
        if (!isRegistered || arena == null)
        {
            return;
        }

        arena.NotifyEnemyRemoved(this);
        isRegistered = false;
    }
}
