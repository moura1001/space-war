using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDP
{
    public class UDPSocket
    {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 256;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private IPEndPoint remoteAddr;
        private AsyncCallback recv = null;

        public delegate void UdpOnReceived(string ip, int port, byte[] data, int bytesRead);
        public event UdpOnReceived OnReceived;

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive();
        }

        public void Client(string address, int port)
        {
            Connect(address, port);
            Receive();
        }

        public void Connect(string address, int port)
        {
            remoteAddr = new IPEndPoint(IPAddress.Parse(address), port);
        }

        public void Close()
        {
            _socket.Close();
        }

        public void Send(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            _socket.SendTo(data, 0, data.Length, SocketFlags.None, remoteAddr);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);

                string ip = ((IPEndPoint)epFrom).Address.ToString();
                int port = ((IPEndPoint)epFrom).Port;

                OnReceived?.Invoke(ip, port, state.buffer, bytes);

                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
            }, state);
        }
    }
}
