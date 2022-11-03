using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class ClientHandler : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

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

        //string[] msgParams = message.Split(';');

        //if (msgParams[0].StartsWith("MOVE"))
        //{
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

            //UDPSocketHandler.Instance.ClientSendMessage(message);
        //}

        Debug.Log("CLIENT: RECV: " + message);

        UDPSocketHandler.Instance.ClientSendMessage(message);
    }
}
