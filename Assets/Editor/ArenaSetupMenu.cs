using UnityEditor;
using UnityEngine;

public static class ArenaSetupMenu
{
    private const string SnakeEnemyPath = "Assets/Prefabs/EXTENDEDREACHSnakeEnemy.prefab";
    private const string LootBoxPath = "Assets/Prefabs/LOOTBOX 1.prefab";

    [MenuItem("Tools/PhantomV/Arena/Create Basic Arena Setup")]
    public static void CreateBasicArenaSetup()
    {
        GameObject managerObject = GameObject.Find("ArenaWaveManager");

        if (managerObject == null)
        {
            managerObject = new GameObject("ArenaWaveManager");
            Undo.RegisterCreatedObjectUndo(managerObject, "Create Arena Wave Manager");
        }

        ArenaWaveManager manager = managerObject.GetComponent<ArenaWaveManager>();

        if (manager == null)
        {
            manager = Undo.AddComponent<ArenaWaveManager>(managerObject);
        }

        Transform player = FindPlayer();
        Transform playerSpawn = CreateOrFindMarker("ArenaPlayerSpawn", player != null ? player.position : Vector3.zero);
        ArenaSpawnPoint[] spawnPoints = CreateSpawnPoints(playerSpawn.position);
        ArenaLootManager lootManager = CreateOrFindLootManager(playerSpawn.position);
        GameObject snakeEnemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SnakeEnemyPath);

        SerializedObject serializedManager = new SerializedObject(manager);
        SetObjectReference(serializedManager, "playerSpawnPoint", playerSpawn);
        SetObjectReference(serializedManager, "player", player != null ? player.gameObject : null);
        SetObjectReference(serializedManager, "lootManager", lootManager);
        SetObjectArray(serializedManager, "spawnPoints", spawnPoints);

        if (snakeEnemyPrefab != null)
        {
            SetObjectArray(serializedManager, "enemyPrefabs", new[] { snakeEnemyPrefab });
        }
        else
        {
            Debug.LogWarning($"Could not find default enemy prefab at {SnakeEnemyPath}. Assign an enemy prefab to ArenaWaveManager manually.");
        }

        serializedManager.ApplyModifiedProperties();
        EditorUtility.SetDirty(manager);
        Selection.activeObject = managerObject;

        Debug.Log("Created basic arena setup. Assign optional UI text fields on ArenaWaveManager if you want wave/enemy counters.");
    }

    private static Transform FindPlayer()
    {
        GameObject player = GameObject.Find("ThirdPersonTEST");

        if (player == null)
        {
            ThirdPersonController playerController = Object.FindFirstObjectByType<ThirdPersonController>();
            player = playerController != null ? playerController.gameObject : null;
        }

        return player != null ? player.transform : null;
    }

    private static ArenaSpawnPoint[] CreateSpawnPoints(Vector3 center)
    {
        GameObject parent = GameObject.Find("ArenaSpawnPoints");

        if (parent == null)
        {
            parent = new GameObject("ArenaSpawnPoints");
            Undo.RegisterCreatedObjectUndo(parent, "Create Arena Spawn Points");
        }

        Vector3[] offsets =
        {
            new Vector3(12f, 0f, 0f),
            new Vector3(-12f, 0f, 0f),
            new Vector3(0f, 0f, 12f),
            new Vector3(0f, 0f, -12f)
        };

        ArenaSpawnPoint[] points = new ArenaSpawnPoint[offsets.Length];

        for (int i = 0; i < offsets.Length; i++)
        {
            Transform marker = CreateOrFindMarker($"ArenaSpawnPoint_{i + 1}", center + offsets[i]);
            marker.SetParent(parent.transform);

            ArenaSpawnPoint spawnPoint = marker.GetComponent<ArenaSpawnPoint>();

            if (spawnPoint == null)
            {
                spawnPoint = Undo.AddComponent<ArenaSpawnPoint>(marker.gameObject);
            }

            points[i] = spawnPoint;
        }

        return points;
    }

    private static ArenaLootManager CreateOrFindLootManager(Vector3 center)
    {
        GameObject managerObject = GameObject.Find("ArenaLootManager");

        if (managerObject == null)
        {
            managerObject = new GameObject("ArenaLootManager");
            Undo.RegisterCreatedObjectUndo(managerObject, "Create Arena Loot Manager");
        }

        ArenaLootManager lootManager = managerObject.GetComponent<ArenaLootManager>();

        if (lootManager == null)
        {
            lootManager = Undo.AddComponent<ArenaLootManager>(managerObject);
        }

        ArenaLootSpawnPoint[] lootSpawnPoints = CreateLootSpawnPoints(center);
        GameObject lootBoxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LootBoxPath);

        SerializedObject serializedLootManager = new SerializedObject(lootManager);
        SetObjectArray(serializedLootManager, "lootSpawnPoints", lootSpawnPoints);

        if (lootBoxPrefab != null)
        {
            SetObjectReference(serializedLootManager, "lootBoxPrefab", lootBoxPrefab);
        }
        else
        {
            Debug.LogWarning($"Could not find loot box prefab at {LootBoxPath}. Assign a loot box prefab to ArenaLootManager manually.");
        }

        serializedLootManager.ApplyModifiedProperties();
        EditorUtility.SetDirty(lootManager);
        return lootManager;
    }

    private static ArenaLootSpawnPoint[] CreateLootSpawnPoints(Vector3 center)
    {
        GameObject parent = GameObject.Find("ArenaLootSpawnPoints");

        if (parent == null)
        {
            parent = new GameObject("ArenaLootSpawnPoints");
            Undo.RegisterCreatedObjectUndo(parent, "Create Arena Loot Spawn Points");
        }

        Vector3[] offsets =
        {
            new Vector3(4f, 0f, 4f),
            new Vector3(-4f, 0f, 4f),
            new Vector3(4f, 0f, -4f),
            new Vector3(-4f, 0f, -4f),
            new Vector3(0f, 0f, 7f),
            new Vector3(0f, 0f, -7f)
        };

        ArenaLootSpawnPoint[] points = new ArenaLootSpawnPoint[offsets.Length];

        for (int i = 0; i < offsets.Length; i++)
        {
            Transform marker = CreateOrFindMarker($"ArenaLootSpawnPoint_{i + 1}", center + offsets[i]);
            marker.SetParent(parent.transform);

            ArenaLootSpawnPoint spawnPoint = marker.GetComponent<ArenaLootSpawnPoint>();

            if (spawnPoint == null)
            {
                spawnPoint = Undo.AddComponent<ArenaLootSpawnPoint>(marker.gameObject);
            }

            points[i] = spawnPoint;
        }

        return points;
    }

    private static Transform CreateOrFindMarker(string objectName, Vector3 position)
    {
        GameObject marker = GameObject.Find(objectName);

        if (marker == null)
        {
            marker = new GameObject(objectName);
            Undo.RegisterCreatedObjectUndo(marker, $"Create {objectName}");
        }

        marker.transform.position = position;
        return marker.transform;
    }

    private static void SetObjectReference(SerializedObject serializedObject, string propertyName, Object value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property != null)
        {
            property.objectReferenceValue = value;
        }
    }

    private static void SetObjectArray<T>(SerializedObject serializedObject, string propertyName, T[] values) where T : Object
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property == null)
        {
            return;
        }

        property.arraySize = values.Length;

        for (int i = 0; i < values.Length; i++)
        {
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }
    }
}
