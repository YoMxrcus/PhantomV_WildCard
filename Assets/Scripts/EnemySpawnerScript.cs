using UnityEngine;
using System.Collections;


public class EnemySpawnerScript : MonoBehaviour
{

    [SerializeField] GameObject EnemyPrefab;
    [SerializeField] GameObject[] Spawners;
    [SerializeField] float SpawnHeightOffset = 1f;
    [SerializeField] bool PlayerDetect = false;
    public GameObject Player;
    public int SpawnCap;
    public int CurrentSpawnAmount = 0;
    private Coroutine spawncoroutine;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("ThirdPersonTEST");
    }

    // Update is called once per frame
  

    public IEnumerator SpawnCoroutine()
    {
        while (PlayerDetect)
        {
            yield return new WaitForSeconds(3f);

            if (CurrentSpawnAmount < SpawnCap && EnemyPrefab != null && Spawners != null && Spawners.Length > 0)
            {
                int selectedSpawnNum = Random.Range(0, Spawners.Length);
                GameObject selectedSpawn = Spawners[selectedSpawnNum];
                if (selectedSpawn == null)
                {
                    continue;
                }

                Vector3 spawnPosition = selectedSpawn.transform.position;
                spawnPosition.y += SpawnHeightOffset;

                GameObject enemyInstance = Instantiate(EnemyPrefab, spawnPosition, Quaternion.identity);

                Enemy enemy = enemyInstance.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.SpawnerRef = gameObject;
                }

                CurrentSpawnAmount++;

            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag) 
        {
            case "Player":
                PlayerDetect = true;
                StartSpawning();
                break;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag) 
        {
            case "Player":
                PlayerDetect = false;
                StopSpawning();
                break;
        }

    }

    void StartSpawning()
    {
        spawncoroutine = StartCoroutine(SpawnCoroutine());
    }
    void StopSpawning()
    {
        if (spawncoroutine != null)
        {
            StopCoroutine(spawncoroutine);
            spawncoroutine = null;
        }
    }
}
