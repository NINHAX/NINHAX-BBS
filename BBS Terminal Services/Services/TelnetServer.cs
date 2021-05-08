using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.TS.Services
{
    class TelnetServer
    {
        private IPAddress ip;
        private ushort port;
        private Socket socket;
        private readonly ushort dataSize;
        private byte[] data;
        private Dictionary<Socket, NVT> NVTs;

        public delegate void ConnectionEvent(NVT nvt);
        public event ConnectionEvent Connected;
        public delegate void DisconnectionEvent(EndPoint remote);
        public event DisconnectionEvent Disconnected;
        public delegate void ErrorConnectionEvent(EndPoint remote);
        public event ErrorConnectionEvent ErrorConnection;
        public delegate void ErrorSendEvent(EndPoint socket);
        public event ErrorSendEvent ErrorSend;
        public delegate void ErrorReciveEvent(EndPoint socket);
        public event ErrorSendEvent ErrorRecive;
        public delegate void ControlCharEvent(NVT nvt, byte[] data, int bytesRecived);
        public event ControlCharEvent ControlChar;
        public delegate void ControlMessageEvent(NVT nvt, string data);
        public event ControlMessageEvent MessageRecived;

        public TelnetServer(IPAddress ip, ushort port, ushort dataSize = 1024)
        {
            this.ip = ip;
            this.port = port;
            this.dataSize = dataSize;
            this.data = new byte[dataSize];
            this.NVTs = new Dictionary<Socket, NVT>();
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            socket.Bind(new IPEndPoint(ip, port));
            socket.Listen(0);
            socket.BeginAccept(new AsyncCallback(HandleConnection), socket);
        }

        public void Stop() => socket.Close();

        private NVT GetNVTBySocket(Socket socket)
        {
            NVT result;
            if (!NVTs.TryGetValue(socket, out result))
                result = null;
            return result;
        }

        public Socket GetSocketByNVT(NVT nvt) => NVTs.FirstOrDefault(nvto => nvto.Value.nvtId == nvt.nvtId).Key;

        private void HandleConnection(IAsyncResult asyncResult)
        {
            Socket esocket = (Socket)asyncResult.AsyncState;
            try
            {
                socket = esocket.EndAccept(asyncResult);

                Guid nvtId = Guid.NewGuid();
                NVT nvt = new NVT(nvtId, (IPEndPoint)socket.RemoteEndPoint);
                NVTs.Add(socket, nvt);

                SendDataToSocket(socket, TelnetIACHandler.Do(TelnetIACHandler.Option.Echo));
                SendDataToSocket(socket, TelnetIACHandler.Do(TelnetIACHandler.Option.RemoteFlowControl));
                SendDataToSocket(socket, TelnetIACHandler.Will(TelnetIACHandler.Option.Echo));
                SendDataToSocket(socket, TelnetIACHandler.Will(TelnetIACHandler.Option.SuppressGoAhead));
                SendDataToSocket(socket, TelnetIACHandler.Will(TelnetIACHandler.Option.BinaryTransmission));
                SendDataToSocket(socket, TelnetIACHandler.Do(TelnetIACHandler.Option.NegotiateAboutWindowSize));

                Connected(nvt);
                esocket.BeginAccept(new AsyncCallback(HandleConnection), esocket);
            }
            catch
            {
                ErrorConnection(esocket.RemoteEndPoint);
            }
        }

        private void CloseSocket(Socket socket)
        {
            Disconnected(socket.RemoteEndPoint);
            NVTs.Remove(socket);
            socket.Close();
        }

        private void SendData(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            try
            {
                socket.EndSend(result);
                socket.BeginReceive(data, 0, dataSize, SocketFlags.None, new AsyncCallback(receiveData), socket);
            }
            catch
            {
                ErrorSend(socket.RemoteEndPoint);
            }
        }

        public void SendMessageToNVT(NVT nvt, string message)
        {
            Socket socket = GetSocketByNVT(nvt);
            SendStringToSocket(socket, message);
        }

        private void SendDataToSocket(Socket socket, byte[] data) =>
           socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendData), socket);

        private void SendStringToSocket(Socket socket, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            SendDataToSocket(socket, data);
        }

        private void receiveData(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            try
            {
                NVT nvt = GetNVTBySocket(socket);
                int bytesReceived = socket.EndReceive(result);
                if (bytesReceived == 0)
                {
                    CloseSocket(socket);
                    Disconnected(socket.RemoteEndPoint);
                    socket.BeginAccept(new AsyncCallback(HandleConnection), socket);
                }
                else if (data[0] == (byte)TelnetIACHandler.Command.IAC)
                {
                    int offset = 0;
                    while (offset < bytesReceived)
                    {
                        TelnetIACHandler.Binary(data, offset, nvt);
                        TelnetIACHandler.WindowSize(data, offset, nvt);
                        while (++offset < bytesReceived)
                        {
                            if (data[offset] == (byte)TelnetIACHandler.Command.IAC) break;
                        }
                    }
                    socket.BeginReceive(data, 0, dataSize, SocketFlags.None, new AsyncCallback(receiveData), socket);
                }
                else if (data[0] < 0xF0)
                {
                    string inputData = nvt.inputData;
                    nvt.lastActionAt = DateTime.Now;
                    if ((data[0] == 0x2E && data[1] == 0x0D && nvt.inputData.Length == 0) || (data[0] == 0x0D && data[1] == 0x0A) || data[0] == 0x0D || data[0] == 0x0A)
                    {
                        MessageRecived(nvt, nvt.inputData);
                        nvt.ResetInputData();
                    }
                    else
                    {
                        if (data[0] == 0x08)
                        {
                            if (inputData.Length > 0)
                            {
                                nvt.RemoveLastChar();
                                SendDataToSocket(socket, new byte[] { 0x08, 0x20, 0x08 });
                            }
                            else
                                socket.BeginReceive(data, 0, dataSize, SocketFlags.None, new AsyncCallback(receiveData), socket);
                        }
                        else if (data[0] == 0x7F)
                            socket.BeginReceive(data, 0, dataSize, SocketFlags.None, new AsyncCallback(receiveData), socket);
                        else if (data[0] < 0x20)
                            ControlChar(nvt, data, bytesReceived);
                        else
                        {
                            nvt.AppendInputData(Encoding.ASCII.GetString(data, 0, bytesReceived));
                            if (nvt.mode != NVT.Mode.PasswordMode)
                                SendDataToSocket(socket, new byte[] { data[0] });
                            else
                                SendStringToSocket(socket, "*");
                            socket.BeginReceive(data, 0, dataSize, SocketFlags.None, new AsyncCallback(receiveData), socket);
                        }
                    }
                }
                else
                    socket.BeginReceive(data, 0, dataSize, SocketFlags.None, new AsyncCallback(receiveData), socket);
            }
            catch
            {
                try
                {
                    ErrorRecive(socket.RemoteEndPoint);
                }
                catch { }
            }
        }

        private bool ClientTimeout(NVT nvt) => (DateTime.Now - nvt.lastActionAt).TotalSeconds > 500;

        /// <summary>
        /// Scans all clients and kills inactive ones
        /// </summary>
        public void PurgeSockets()
        {
            foreach (KeyValuePair<Socket, NVT> nvt in NVTs)
            {
                if (ClientTimeout(nvt.Value))
                {
                    CloseSocket(nvt.Key);
                }
            }
        }
    }
}
