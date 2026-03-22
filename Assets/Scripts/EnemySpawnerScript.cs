using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{

    [SerializeField] GameObject EnemyPrefab;
    [SerializeField] GameObject[] Spawners;
    public GameObject Player;
    public int SpawnCap;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("ThirdPersonTEST");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
