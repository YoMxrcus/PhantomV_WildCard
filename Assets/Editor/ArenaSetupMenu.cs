using UnityEditor;
using UnityEngine;

public static class ArenaSetupMenu
{
    private const string SnakeEnemyPath = "Assets/Prefabs/SNAKEENEMY.prefab";
    private const string ArenaZombieWalkerPath = "Assets/Prefabs/ArenaZombieWalker.prefab";
    private const string ArenaZombieRunnerPath = "Assets/Prefabs/ArenaZombieRunner.prefab";
    private const string LootBoxPath = "Assets/Prefabs/LOOTBOX 1.prefab";
    private const string CondensorLootImagePath = "Assets/Images/NEWLootImages/condenser.png";

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
        ArenaOverheatManager overheatManager = CreateOrFindOverheatManager();
        ArenaEnemyDirectionIndicator enemyDirectionIndicator = CreateOrFindEnemyDirectionIndicator();
        GameObject snakeEnemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SnakeEnemyPath);
        GameObject arenaZombieWalkerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ArenaZombieWalkerPath);
        GameObject arenaZombieRunnerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ArenaZombieRunnerPath);

        SerializedObject serializedManager = new SerializedObject(manager);
        SetObjectReference(serializedManager, "playerSpawnPoint", playerSpawn);
        SetObjectReference(serializedManager, "player", player != null ? player.gameObject : null);
        SetObjectReference(serializedManager, "lootManager", lootManager);
        SetObjectReference(serializedManager, "overheatManager", overheatManager);
        SetObjectArray(serializedManager, "spawnPoints", spawnPoints);

        if (snakeEnemyPrefab != null)
        {
            SetObjectArray(serializedManager, "enemyPrefabs", new[] { snakeEnemyPrefab });
        }
        else
        {
            Debug.LogWarning($"Could not find default enemy prefab at {SnakeEnemyPath}. Assign an enemy prefab to ArenaWaveManager manually.");
        }

        if (arenaZombieWalkerPrefab != null && arenaZombieRunnerPrefab != null)
        {
            SetObjectArray(serializedManager, "postWaveFiveEnemyPrefabs", new[] { arenaZombieWalkerPrefab, arenaZombieRunnerPrefab });
        }
        else
        {
            Debug.LogWarning("Arena zombie prefabs were not found yet. Run Tools > PhantomV > Arena > Create Arena Zombie Prefabs, then run Create Basic Arena Setup again or assign them manually.");
        }

        serializedManager.ApplyModifiedProperties();
        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(enemyDirectionIndicator);
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

        (string name, Vector3 offset, ArenaSpawnPoint.SpawnPointType spawnType)[] spawnDefinitions =
        {
            ("ArenaSnakeSpawn_RockPile_1", new Vector3(12f, 0f, 0f), ArenaSpawnPoint.SpawnPointType.Snake),
            ("ArenaSnakeSpawn_RockPile_2", new Vector3(-12f, 0f, 0f), ArenaSpawnPoint.SpawnPointType.Snake),
            ("ArenaSnakeSpawn_RockPile_3", new Vector3(0f, 0f, 12f), ArenaSpawnPoint.SpawnPointType.Snake),
            ("ArenaSnakeSpawn_RockPile_4", new Vector3(0f, 0f, -12f), ArenaSpawnPoint.SpawnPointType.Snake),
            ("ArenaZombieSpawn_Cryopod_1", new Vector3(16f, 0f, 8f), ArenaSpawnPoint.SpawnPointType.Zombie),
            ("ArenaZombieSpawn_Cryopod_2", new Vector3(-16f, 0f, 8f), ArenaSpawnPoint.SpawnPointType.Zombie),
            ("ArenaZombieSpawn_Cryopod_3", new Vector3(16f, 0f, -8f), ArenaSpawnPoint.SpawnPointType.Zombie),
            ("ArenaZombieSpawn_Cryopod_4", new Vector3(-16f, 0f, -8f), ArenaSpawnPoint.SpawnPointType.Zombie)
        };

        ArenaSpawnPoint[] points = new ArenaSpawnPoint[spawnDefinitions.Length];

        for (int i = 0; i < spawnDefinitions.Length; i++)
        {
            Transform marker = CreateOrFindMarker(spawnDefinitions[i].name, center + spawnDefinitions[i].offset);
            marker.SetParent(parent.transform);

            ArenaSpawnPoint spawnPoint = marker.GetComponent<ArenaSpawnPoint>();

            if (spawnPoint == null)
            {
                spawnPoint = Undo.AddComponent<ArenaSpawnPoint>(marker.gameObject);
            }

            SerializedObject serializedSpawnPoint = new SerializedObject(spawnPoint);
            SetEnum(serializedSpawnPoint, "spawnType", (int)spawnDefinitions[i].spawnType);
            serializedSpawnPoint.ApplyModifiedProperties();
            EditorUtility.SetDirty(spawnPoint);

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
        Texture condensorLootImage = AssetDatabase.LoadAssetAtPath<Texture>(CondensorLootImagePath);

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

        if (condensorLootImage != null)
        {
            SetObjectReference(serializedLootManager, "condensorLootImage", condensorLootImage);
        }
        else
        {
            Debug.LogWarning($"Could not find condenser loot image at {CondensorLootImagePath}. Assign it to ArenaLootManager manually.");
        }

        serializedLootManager.ApplyModifiedProperties();
        EditorUtility.SetDirty(lootManager);
        return lootManager;
    }

    private static ArenaOverheatManager CreateOrFindOverheatManager()
    {
        GameObject managerObject = GameObject.Find("ArenaOverheatManager");

        if (managerObject == null)
        {
            managerObject = new GameObject("ArenaOverheatManager");
            Undo.RegisterCreatedObjectUndo(managerObject, "Create Arena Overheat Manager");
        }

        ArenaOverheatManager overheatManager = managerObject.GetComponent<ArenaOverheatManager>();

        if (overheatManager == null)
        {
            overheatManager = Undo.AddComponent<ArenaOverheatManager>(managerObject);
        }

        EditorUtility.SetDirty(overheatManager);
        return overheatManager;
    }

    private static ArenaEnemyDirectionIndicator CreateOrFindEnemyDirectionIndicator()
    {
        GameObject indicatorObject = GameObject.Find("ArenaEnemyDirectionIndicator");

        if (indicatorObject == null)
        {
            indicatorObject = new GameObject("ArenaEnemyDirectionIndicator");
            Undo.RegisterCreatedObjectUndo(indicatorObject, "Create Arena Enemy Direction Indicator");
        }

        ArenaEnemyDirectionIndicator indicator = indicatorObject.GetComponent<ArenaEnemyDirectionIndicator>();

        if (indicator == null)
        {
            indicator = Undo.AddComponent<ArenaEnemyDirectionIndicator>(indicatorObject);
        }

        EditorUtility.SetDirty(indicator);
        return indicator;
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

    private static void SetEnum(SerializedObject serializedObject, string propertyName, int enumValueIndex)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);

        if (property != null)
        {
            property.enumValueIndex = enumValueIndex;
        }
    }
}
