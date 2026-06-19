using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static class ArenaZombiePrefabBuilder
{
    private const string SourceZombiePath = "Assets/Prefabs/Zombie(NOSCRIPTS).prefab";
    private const string WalkerPrefabPath = "Assets/Prefabs/ArenaZombieWalker.prefab";
    private const string RunnerPrefabPath = "Assets/Prefabs/ArenaZombieRunner.prefab";

    [MenuItem("Tools/PhantomV/Arena/Create Arena Zombie Prefabs")]
    public static void CreateArenaZombiePrefabs()
    {
        GameObject sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SourceZombiePath);

        if (sourcePrefab == null)
        {
            Debug.LogError($"Could not find zombie source prefab at {SourceZombiePath}.");
            return;
        }

        CreateZombiePrefab(sourcePrefab, WalkerPrefabPath, ArenaZombieEnemy.ZombieKind.Walker, "ArenaZombieWalker", 80, 7);
        CreateZombiePrefab(sourcePrefab, RunnerPrefabPath, ArenaZombieEnemy.ZombieKind.Runner, "ArenaZombieRunner", 60, 5);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Created arena zombie prefabs. Assign ArenaZombieWalker and ArenaZombieRunner to ArenaWaveManager Post Wave Five Enemy Prefabs.");
    }

    private static void CreateZombiePrefab(GameObject sourcePrefab, string prefabPath, ArenaZombieEnemy.ZombieKind kind, string prefabName, int health, int damage)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(sourcePrefab);
        instance.name = prefabName;

        TrySetTag(instance, "BanditHitbox");
        ConfigureCapsuleCollider(instance);
        ConfigureRigidbody(instance);
        ConfigureNavMeshAgent(instance, kind);
        ConfigureZombieEnemy(instance, kind, health, damage);

        if (instance.GetComponent<ArenaEnemyTracker>() == null)
        {
            instance.AddComponent<ArenaEnemyTracker>();
        }

        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Object.DestroyImmediate(instance);
    }

    private static void ConfigureCapsuleCollider(GameObject instance)
    {
        CapsuleCollider collider = instance.GetComponent<CapsuleCollider>();

        if (collider == null)
        {
            collider = instance.AddComponent<CapsuleCollider>();
        }

        collider.isTrigger = true;
        collider.center = new Vector3(0f, 0.04f, 0f);
        collider.radius = 0.02f;
        collider.height = 0.08f;
    }

    private static void ConfigureRigidbody(GameObject instance)
    {
        Rigidbody rigidbody = instance.GetComponent<Rigidbody>();

        if (rigidbody == null)
        {
            rigidbody = instance.AddComponent<Rigidbody>();
        }

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }

    private static void ConfigureNavMeshAgent(GameObject instance, ArenaZombieEnemy.ZombieKind kind)
    {
        NavMeshAgent agent = instance.GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            agent = instance.AddComponent<NavMeshAgent>();
        }

        agent.speed = kind == ArenaZombieEnemy.ZombieKind.Runner ? 4.4f : 2.2f;
        agent.acceleration = kind == ArenaZombieEnemy.ZombieKind.Runner ? 18f : 10f;
        agent.angularSpeed = 720f;
        agent.stoppingDistance = 1.25f;
        agent.radius = 0.02f;
        agent.height = 0.08f;
        agent.autoBraking = false;
    }

    private static void ConfigureZombieEnemy(GameObject instance, ArenaZombieEnemy.ZombieKind kind, int health, int damage)
    {
        ArenaZombieEnemy zombie = instance.GetComponent<ArenaZombieEnemy>();

        if (zombie == null)
        {
            zombie = instance.AddComponent<ArenaZombieEnemy>();
        }

        SerializedObject serializedZombie = new SerializedObject(zombie);
        SetEnum(serializedZombie, "kind", (int)kind);
        SetInt(serializedZombie, "maxHealth", health);
        SetInt(serializedZombie, "attackDamage", damage);
        serializedZombie.ApplyModifiedProperties();
        EditorUtility.SetDirty(zombie);
    }

    private static void TrySetTag(GameObject instance, string tag)
    {
        try
        {
            instance.tag = tag;
        }
        catch
        {
            Debug.LogWarning($"Could not set {instance.name} tag to {tag}. Add the tag manually if grenade detection does not pick up zombies.");
        }
    }

    private static void SetEnum(SerializedObject serializedObject, string propertyName, int enumValueIndex)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property != null)
        {
            property.enumValueIndex = enumValueIndex;
        }
    }

    private static void SetInt(SerializedObject serializedObject, string propertyName, int value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property != null)
        {
            property.intValue = value;
        }
    }
}
