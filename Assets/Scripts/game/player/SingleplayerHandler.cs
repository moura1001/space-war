using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayerHandler : MonoBehaviour
{
    public GameObject player1;
    public GameObject spawnHandler;
    public GameObject projectil;

    private GameObject player;

    private float moveSpeed = 10;

    private Dictionary<string, (GameObject, int)> enemies;

    // Start is called before the first frame update
    void Start()
    {
        enemies = new Dictionary<string, (GameObject, int)>();

        InstantiatePlayer();
        InstantiateSpawnHandler();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        float verticalInput = Input.GetAxis("Vertical");

        Vector3 newPos = player.transform.position + (new Vector3(horizontalInput, verticalInput) * moveSpeed * Time.deltaTime);


        if ((horizontalInput != 0 || verticalInput != 0) && (newPos.x < 8 && newPos.x > -8 && newPos.y < 4 && newPos.y > -4))
        {
            player.transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            var proj = Instantiate(projectil, new Vector3(player.transform.position.x, player.transform.position.y + 1.5f), Quaternion.identity);
            proj.GetComponent<Projectil>().OnDestroyProjectil += OnDestroyProjectil;
        }
    }

    private void InstantiatePlayer()
    {
        player = Instantiate(player1, new Vector3(0,-4), Quaternion.identity);
    }

    private void InstantiateSpawnHandler()
    {
        var spawn = Instantiate(spawnHandler).GetComponent<SpawnHandler>();
        spawn.OnSpawn += OnSpawnEnemy;
    }

    private void OnSpawnEnemy(GameObject enemy)
    {
        Enemy enemyScript = enemy.GetComponent<Enemy>();

        string id = enemyScript.GetId();

        enemies.Add(id, (enemy, 5));

        enemyScript.OnArrivalPosition += OnArrivalPositionEnemy;
        enemyScript.OnHitDamage += OnEnemyDamage;
    }

    private void OnArrivalPositionEnemy(GameObject enemy)
    {
        string id = enemy.GetComponent<Enemy>().GetId();

        DestroyEnemy(id);
    }

    private void OnEnemyDamage(GameObject enemy, GameObject projectil)
    {
        Destroy(projectil);

        string id = enemy.GetComponent<Enemy>().GetId();

        (GameObject, int) enem;

        if (enemies.TryGetValue(id, out enem))
        {
            enem.Item2 -= 2;

            enemies[id] = enem;

            if (enem.Item2 <= 0)
            {
                enemies.Remove(id);
                Destroy(enemy);
            }
        }
    }

    private void DestroyEnemy(string id)
    {
        (GameObject, int) enemy;

        if (enemies.TryGetValue(id, out enemy))
        {
            enemies.Remove(id);
            Destroy(enemy.Item1);
        }
    }

    private void OnDestroyProjectil(GameObject projectil)
    {
        Destroy(projectil);
    }
}
