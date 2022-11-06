using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UDP;

public class UDPSocketHandler : MonoBehaviour
{
    public static UDPSocketHandler Instance;

    private UDPSocket socket;

    private bool connectionEstablished;
    private bool isServer;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        socket?.Close();
        Debug.Log("OnDestroy");
    }

    public void ClientHandler(string ip, int port)
    {
        isServer = false;

        socket = new UDPSocket();
        socket.Client(ip, port);
        socket.OnReceived += ClientReceiveMessage;
        
        socket.Send("CONNECT");
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

    public void ServerHandler()
    {
        isServer = true;

        socket = new UDPSocket();
        socket.Server("127.0.0.1", 4000);

        socket.OnReceived += ServerReceiveMessage;
    }

    private void ServerReceiveMessage(string ip, int port, byte[] data, int bytesRead)
    {
        string message = Encoding.ASCII.GetString(data, 0, bytesRead);

        if ("CONNECT".Equals(message) && !connectionEstablished)
        {
            socket.Connect(ip, port);
            
            socket.Send("ACCEPTED");

            connectionEstablished = true;
        }
        Debug.Log("SERVER: RECV: " + message);
    }

    public void SocketSendMessage(string message)
    {
        socket.Send(message);
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
        socket.OnReceived -= ClientReceiveMessage;
        socket.OnReceived += listener;
    }

    public void AddOnReceiveMessageListenerForServer(UDPSocket.UdpOnReceived listener)
    {
        socket.OnReceived -= ServerReceiveMessage;
        socket.OnReceived += listener;
    }
}
