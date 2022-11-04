using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class ServerHandler : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject spawnHandler;

    private GameObject player;
    private GameObject client;

    private Dictionary<string, GameObject> gameEntities;

    private float moveSpeed = 10;

    private float horizontalInputClient;
    private float verticalInputClient;

    // Start is called before the first frame update
    void Start()
    {
        gameEntities = new Dictionary<string, GameObject>();

        UDPSocketHandler.Instance.AddOnReceiveMessageListenerForServer(OnReceiveMessage);

        InstantiatePlayer1();
        InstantiatePlayer2();
        InstantiateSpawnHandler();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        float verticalInput = Input.GetAxis("Vertical");

        Vector3 newPos = player.transform.position + (new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime);


        if ((horizontalInput != 0 || verticalInput != 0) && (newPos.x < 8 && newPos.x > -8 && newPos.y < 4 && newPos.y > -4))
        {
            UDPSocketHandler.Instance.ServerSendMessage($"MOVE;player2;{horizontalInput}@{verticalInput}");
            player.transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime);

            client.transform.Translate(new Vector3(horizontalInputClient, verticalInputClient, 0) * 10 * Time.deltaTime);
        }
    }

    private void InstantiatePlayer1()
    {
        Vector3 pos = player1.transform.position;

        string id = Guid.NewGuid().ToString();

        player = Instantiate(player1, pos, Quaternion.identity);

        gameEntities.Add(id, player);

        UDPSocketHandler.Instance.ServerSendMessage($"CREATE;player1;{id};{pos.x},{pos.y}");
    }

    private void InstantiatePlayer2()
    {
        Vector3 pos = player2.transform.position;

        string id = Guid.NewGuid().ToString();

        client = Instantiate(player2, pos, Quaternion.identity);

        gameEntities.Add(id, client);

        UDPSocketHandler.Instance.ServerSendMessage($"CREATE;player2;{id};{pos.x},{pos.y}");
    }

    private void OnReceiveMessage(string ip, int port, byte[] data, int bytesRead)
    {
        string message = Encoding.ASCII.GetString(data, 0, bytesRead);

        string[] msgParams = message.Split(';');

        if (msgParams[0].StartsWith("MOVE"))
        {
            if (msgParams.Length == 3)
            {
                string type = msgParams[1];

                string[] moveParams = msgParams[2].Split('@');

                if (moveParams.Length == 2)
                {
                    horizontalInputClient = float.Parse(moveParams[0]);
                    verticalInputClient = float.Parse(moveParams[1]);
                }
            }
        }
    }

    private void InstantiateSpawnHandler()
    {
        var spawn = Instantiate(spawnHandler).GetComponent<SpawnHandler>();
        spawn.OnSpawn += OnSpawnEnemy;

    }

    private void OnSpawnEnemy(GameObject enemy)
    {
        Vector3 pos = enemy.transform.position;
        Enemy enemyScript = enemy.GetComponent<Enemy>();

        string id = enemyScript.GetId();

        gameEntities.Add(id, enemy);

        UDPSocketHandler.Instance.ServerSendMessage($"CREATE;enemy;{id};{pos.x}@{pos.y}");
    }
}
