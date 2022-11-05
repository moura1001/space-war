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

    private Dictionary<string, GameObject> gameEntities;

    // Start is called before the first frame update
    void Start()
    {
        gameEntities = new Dictionary<string, GameObject>();
        UDPSocketHandler.Instance.AddOnReceiveMessageListenerForClient(OnReceiveMessage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InstantiateEntity(string type, string id, Vector3 position)
    {
        GameObject entity = null;

        switch (type)
        {
            case "player1":
                entity = Instantiate(player1, position, Quaternion.identity);
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
            /*if (msgParams.Length == 3)
            {
                string type = msgParams[1];

                string[] moveParams = msgParams[2].Split(',');
                float horizontalInput = 0;
                float verticalInput = 0;

                if (moveParams.Length == 2)
                {
                    horizontalInput = float.Parse(moveParams[0]);
                    verticalInput = float.Parse(moveParams[1]);
                }

                player.transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * 10 * Time.deltaTime);
            }*/

            UDPSocketHandler.Instance.ClientSendMessage(message);
        
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
