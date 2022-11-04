using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHandler : MonoBehaviour
{
    public GameObject enemy;

    private float spawnTime = 3f;

    public delegate void OnSpawnEnemy(GameObject enemy);
    public event OnSpawnEnemy OnSpawn;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnEnemy", spawnTime, spawnTime);
    }

    private void SpawnEnemy()
    {
        string id = System.Guid.NewGuid().ToString();

        var position = new Vector3(Random.Range(-8.0f, 8.0f), 5, 0);

        var enemyScript = Instantiate(enemy, position, Quaternion.identity);
        enemyScript.GetComponent<Enemy>().SetId(id);

        OnSpawn(enemyScript);
    }
}
