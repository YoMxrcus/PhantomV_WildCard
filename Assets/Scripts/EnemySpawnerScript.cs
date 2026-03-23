using UnityEngine;
using System.Collections;


public class EnemySpawnerScript : MonoBehaviour
{

    [SerializeField] GameObject EnemyPrefab;
    [SerializeField] GameObject[] Spawners;
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

            if (CurrentSpawnAmount < SpawnCap)
            {
                int selectedSpawnNum = Random.Range(0, Spawners.Length);
                GameObject selectedSpawn = Spawners[selectedSpawnNum].gameObject;
                GameObject enemyInstance = Instantiate(EnemyPrefab, new Vector3(selectedSpawn.transform.position.x, Player.transform.position.y - .25f, selectedSpawn.transform.position.z), Quaternion.identity);

                enemyInstance.GetComponent<Enemy>().SpawnerRef = gameObject;
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
