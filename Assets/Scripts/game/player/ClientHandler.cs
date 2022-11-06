using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class ClientHandler : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject enemy;

    private GameObject player;
    private GameObject server;

    private Dictionary<string, GameObject> gameEntities;

    private float moveSpeed = 10;

    private float horizontalInputServer;
    private float verticalInputServer;
    private bool sendMoveMessage;

    // Start is called before the first frame update
    void Start()
    {
        gameEntities = new Dictionary<string, GameObject>();
        UDPSocketHandler.Instance.AddOnReceiveMessageListenerForClient(OnReceiveMessage);
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

        }
        else if (sendMoveMessage && horizontalInput == 0 && verticalInput == 0)
        {
            sendMoveMessage = false;
            UDPSocketHandler.Instance.SocketSendMessage($"MOVE;{horizontalInput}@{verticalInput}");
        }

        if (server != null && (horizontalInputServer != 0 || verticalInputServer != 0))
        {
            server.transform.Translate(new Vector3(horizontalInputServer, verticalInputServer) * moveSpeed * Time.deltaTime);
        }
    }

    private void InstantiateEntity(string type, string id, Vector3 position)
    {
        GameObject entity = null;

        switch (type)
        {
            case "player1":
                entity = Instantiate(player1, position, Quaternion.identity);
                server = entity;
                break;

            case "player2":
                entity = Instantiate(player2, position, Quaternion.identity);
                player = entity;
                break;

            case "enemy":
                entity = Instantiate(enemy, position, Quaternion.identity);
                break;
        }

        gameEntities.Add(id, entity);
    }

    private void DestroyEntity(string id)
    {
        GameObject entity;

        if (gameEntities.TryGetValue(id, out entity))
        {
            gameEntities.Remove(id);
            Destroy(entity);
        }
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
                    horizontalInputServer = float.Parse(moveParams[0]);
                    verticalInputServer = float.Parse(moveParams[1]);
                }
            }
        
        } 
        else if (msgParams[0].Equals("CREATE"))
        {
            if (msgParams.Length == 4)
            {
                string type = msgParams[1];

                string id = msgParams[2];

                string[] positionParams = msgParams[3].Split('@');
                float posX = 0;
                float posY = 0;

                if (positionParams.Length == 2)
                {
                    posX = float.Parse(positionParams[0]);
                    posY = float.Parse(positionParams[1]);
                }

                InstantiateEntity(type, id, new Vector3(posX, posY, 0));
            }
        }

        Debug.Log("CLIENT: RECV: " + message);
    }
}
