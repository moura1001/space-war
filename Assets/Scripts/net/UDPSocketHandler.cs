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
    private bool isServer;

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

    void OnDestroy()
    {
        server?.Close();
        client?.Close();
        Debug.Log("OnDestroy");
    }

    public void ClientHandler(string ip, int port)
    {
        isServer = false;

        /*socket = new UDPSocket();
        socket.Client(ip, port);
        socket.OnReceived += ClientReceiveMessage;
        
        socket.Send("CONNECT");*/
        //Debug.Log("pçpçpçpçpçpç");
        client = new UDPSocket();
        client.Client(ip, port);
        client.OnReceived += ClientReceiveMessage;

        client.Send("CONNECT");
    }

    private void ClientReceiveMessage(string ip, int port, byte[] data, int bytesRead)
    {
        string message = Encoding.ASCII.GetString(data, 0, bytesRead);

        if ("ACCEPTED".Equals(message))
        {
            connectionEstablished = true;
        }

        Debug.Log("CLIENT: RECV: " + message);
    }

    public void ClientSendMessage(string message)
    {
        client.Send(message);
        Debug.Log("CLIENT MESSAGE SENDED");
    }

    public void ServerHandler()
    {
        isServer = true;

        /*socket = new UDPSocket();
        socket.Server("127.0.0.1", 4000);

        socket.OnReceived += ServerReceiveMessage;*/

        server = new UDPSocket();
        server.Server("127.0.0.1", 4000);

        server.OnReceived += ServerReceiveMessage;
    }

    private void ServerReceiveMessage(string ip, int port, byte[] data, int bytesRead)
    {
        string message = Encoding.ASCII.GetString(data, 0, bytesRead);

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

    public void ServerSendMessage(string message)
    {
        server.Send(message);
    }

    public bool ConnectionEstablished()
    {
        return connectionEstablished;
    }

    public bool IsServer()
    {
        return isServer;
    }

    public void AddOnReceiveMessageListenerForClient(UDPSocket.UdpOnReceived listener)
    {
        client.OnReceived -= ClientReceiveMessage;
        client.OnReceived += listener;
    }

    public void AddOnReceiveMessageListenerForServer(UDPSocket.UdpOnReceived listener)
    {
        server.OnReceived -= ServerReceiveMessage;
        server.OnReceived += listener;
    }
}
