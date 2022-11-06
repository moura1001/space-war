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
    private bool sendMoveMessage;

    // Start is called before the first frame update
    void Start()
    {
        gameEntities = new Dictionary<string, GameObject>();

        UDPSocketHandler.Instance.AddOnReceiveMessageListenerForServer(OnReceiveMessage);

        StartCoroutine(InstantiateEntities());
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        float verticalInput = Input.GetAxis("Vertical");

        Vector3 newPos = player != null ? player.transform.position + (new Vector3(horizontalInput, verticalInput) * moveSpeed * Time.deltaTime) : new Vector3(float.MaxValue, float.MaxValue);


        if ((horizontalInput != 0 || verticalInput != 0) && (newPos.x < 8 && newPos.x > -8 && newPos.y < 4 && newPos.y > -4))
        {
            sendMoveMessage = true;

            UDPSocketHandler.Instance.SocketSendMessage($"MOVE;{horizontalInput}@{verticalInput}");
            player.transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime);
        
        } else if (sendMoveMessage && horizontalInput == 0 && verticalInput == 0)
        {
            sendMoveMessage = false;
            UDPSocketHandler.Instance.SocketSendMessage($"MOVE;{horizontalInput}@{verticalInput}");
        }

        if (client != null && (horizontalInputClient != 0 || verticalInputClient != 0))
        {
            client.transform.Translate(new Vector3(horizontalInputClient, verticalInputClient) * moveSpeed * Time.deltaTime);
        }
    }

    private IEnumerator InstantiateEntities()
    {
        yield return new WaitForSeconds(2);

        InstantiatePlayer1();
        InstantiatePlayer2();
        InstantiateSpawnHandler();
    }

    private void InstantiatePlayer1()
    {
        Vector3 pos = player1.transform.position;

        string id = Guid.NewGuid().ToString();

        player = Instantiate(player1, pos, Quaternion.identity);

        gameEntities.Add(id, player);

        UDPSocketHandler.Instance.SocketSendMessage($"CREATE;player1;{id};{pos.x}@{pos.y}");
    }

    private void InstantiatePlayer2()
    {
        Vector3 pos = player2.transform.position;

        string id = Guid.NewGuid().ToString();

        client = Instantiate(player2, pos, Quaternion.identity);

        gameEntities.Add(id, client);

        UDPSocketHandler.Instance.SocketSendMessage($"CREATE;player2;{id};{pos.x}@{pos.y}");
    }

    private void OnReceiveMessage(string ip, int port, byte[] data, int bytesRead)
    {
        string message = Encoding.ASCII.GetString(data, 0, bytesRead);

        string[] msgParams = message.Split(';');

        if (msgParams[0].Equals("MOVE"))
        {
            if (msgParams.Length == 2)
            {
                string[] moveParams = msgParams[1].Split('@');

                if (moveParams.Length == 2)
                {
                    horizontalInputClient = float.Parse(moveParams[0]);
                    verticalInputClient = float.Parse(moveParams[1]);
                }
            }
        }

        Debug.Log("SERVER: RECV: " + message);
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

        UDPSocketHandler.Instance.SocketSendMessage($"CREATE;enemy;{id};{pos.x}@{pos.y}");

        enemyScript.OnArrivalPosition += OnArrivalPositionEnemy;
    }

    private void OnArrivalPositionEnemy(GameObject enemy)
    {
        string id = enemy.GetComponent<Enemy>().GetId();

        DestroyEnemy(id);
    }

    private void DestroyEnemy(string id)
    {
        GameObject enemy;

        if (gameEntities.TryGetValue(id, out enemy))
        {
            gameEntities.Remove(id);
            Destroy(enemy);

            UDPSocketHandler.Instance.SocketSendMessage($"DESTROY;{id}");
        }
    }
}
