using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UDP;

public class UDPSocketHandler : MonoBehaviour
{
    public static UDPSocketHandler Instance;

    //private UDPSocket socket;

    private bool connectionEstablished;

    private UDPSocket client;
    private UDPSocket server;

    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ClientHandler(string ip, int port)
    {
        /*socket = new UDPSocket();
        socket.Client(ip, port);
        socket.OnReceived += ClientReceiveMessage;
        
        socket.Send("TEST!");
        socket.Send("abc");
        socket.Send("123");
        socket.Send("p�p�p�");
        socket.Send("CONNECT");*/
        //Debug.Log("p�p�p�p�p�p�");
        client = new UDPSocket();
        client.Client(ip, port);
        client.OnReceived += ClientReceiveMessage;

        client.Send("TEST!");
        client.Send("abc");
        client.Send("123");
        client.Send("p�p�p�");
        client.Send("CONNECT");
    }

    private void ClientReceiveMessage(string ip, int port, byte[] data, int bytesRead)
    {
        string message = Encoding.UTF8.GetString(data, 0, bytesRead);

        if ("ACCEPTED".Equals(message))
        {
            //MenuHandler.Instance.LoadScene(1);
        }

        Debug.Log("CLIENT: RECV: " + message);
    }

    public void ServerHandler()
    {
        /*socket = new UDPSocket();
        socket.Server("127.0.0.1", 4000);

        socket.OnReceived += ServerReceiveMessage;*/

        server = new UDPSocket();
        server.Server("127.0.0.1", 4000);

        server.OnReceived += ServerReceiveMessage;
    }

    private void ServerReceiveMessage(string ip, int port, byte[] data, int bytesRead)
    {
        string message = Encoding.UTF8.GetString(data, 0, bytesRead);

        if ("CONNECT".Equals(message))
        {
            //socket.Connect(ip, port);
            //socket.Send("ACCEPTED");
            //MenuHandler.Instance.LoadScene(1);
            server.Connect(ip, port);

            Debug.Log("CONNECTED TO CLIENT. IP: " + ip + " PORT: " + port);


            server.Send("ACCEPTED");
            //MenuHandler.Instance.LoadScene(1);
            //SceneManager.LoadScene("GameScene");

            connectionEstablished = true;
        }
        Debug.Log("SERVER: RECV: " + message);
    }

    public bool ConnectionEstablished()
    {
        return connectionEstablished;
    }
}
