using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class GameHandler : MonoBehaviour
{
    public GameObject serverHandler;
    public GameObject clientHandler;

    void Start()
    {
        if (UDPSocketHandler.Instance.IsServer())
        {
            ServerHandler();
            ClientHandler();
        
        } else
        {
            ClientHandler();
        }
    }

    public void ServerHandler()
    {
        Instantiate(serverHandler, serverHandler.transform.position, Quaternion.identity);
    }

    public void ClientHandler()
    {
        Instantiate(clientHandler, clientHandler.transform.position, Quaternion.identity);
    }
}
