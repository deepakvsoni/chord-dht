namespace ChordDHT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class Program
    {
        #region Socket

        #region Server

        private static AutoResetEvent _connectionComplete
            = new AutoResetEvent(false);

        private static AutoResetEvent _shutDown
             = new AutoResetEvent(false);

        internal class ConnectionData
        {
            public ConnectionData(Socket socket)
            {
                Socket = socket;
                Data = new byte[10];
                Message = new StringBuilder();
            }

            public Socket Socket { get; set; }

            public byte[] Data { get; set; }

            public StringBuilder Message { get; set; }

            public int Sent { get; set; }
        }

        static void StartServer(object p)
        {
            IPHostEntry entry = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = entry.AddressList[0];
            IPEndPoint endpoint = new IPEndPoint(ipAddress, 10000);

            Socket serverSocket = new Socket(AddressFamily.InterNetworkV6
                , SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(endpoint);
            serverSocket.Listen(100);
            Console.WriteLine("Server listening.");

            while (true)
            {
                serverSocket.BeginAccept(AcceptConnectionCallback
                    , serverSocket);

                int index = WaitHandle.WaitAny(
                    new[] { _connectionComplete, _shutDown });

                if (index == 1)
                {
                    serverSocket.Close();
                    Console.WriteLine("Server shutting down.");
                    break;
                }
            }
        }

        static void AcceptConnectionCallback(IAsyncResult result)
        {
            Socket serverSocket = (Socket)result.AsyncState;

            Socket callerSocket = serverSocket.EndAccept(result);

            _connectionComplete.Set();

            ConnectionData data = new ConnectionData(callerSocket);

            callerSocket.BeginReceive(data.Data, 0, 10, SocketFlags.None
                , DataReceiveCallback, data);

        }

        static void DataReceiveCallback(IAsyncResult result)
        {
            ConnectionData data = (ConnectionData)result.AsyncState;

            int bytesRead = data.Socket.EndReceive(result);
            if (bytesRead > 0)
            {
                data.Message.Append(Encoding.ASCII.GetString(data.Data,
                    0, bytesRead));

                if (!data.Message.ToString().EndsWith("<E>"
                    , StringComparison.OrdinalIgnoreCase))
                {
                    data.Socket.BeginReceive(data.Data, 0, 10
                        , SocketFlags.None
                        , DataReceiveCallback, data);
                    return;
                }
            }
            if (data.Message.ToString().Contains("Shutdown"))
            {
                _shutDown.Set();
            }
            else
            {
                Console.WriteLine("Server: Received "
                    + data.Message);
            }
            var bytesToSend = Encoding.ASCII.GetBytes("Hello from Server!!<E>");
            data.Socket.BeginSend(bytesToSend, 0, bytesToSend.Length
                , SocketFlags.None,
                ServerSendCompleted, data);
        }

        static void ServerSendCompleted(IAsyncResult result)
        {
            ConnectionData data = (ConnectionData)result.AsyncState;

            int bytesSent = data.Socket.EndSend(result);

            Console.WriteLine("Bytes sent:" + bytesSent);

            //_shutDown.Set();
        }

        #endregion

        #region Client

        static AutoResetEvent _clientConnected = new AutoResetEvent(false);
        static AutoResetEvent _sendCompleted = new AutoResetEvent(false);
        static AutoResetEvent _receiveCompleted = new AutoResetEvent(false);


        static void StartClient()
        {
            IPHostEntry entry = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = entry.AddressList[0];
            IPEndPoint endpoint = new IPEndPoint(ipAddress, 10000);

            Socket clientSocket = new Socket(AddressFamily.InterNetworkV6
                , SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(endpoint, ClientConnected, clientSocket);

            _clientConnected.WaitOne();

            SendData("Hello from Client<E>", clientSocket);

            _sendCompleted.WaitOne();

            ReceiveData(clientSocket);

            _receiveCompleted.WaitOne();

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();

            Console.WriteLine("Shutting down.");
        }

        static void ClientConnected(IAsyncResult result)
        {
            Socket clientSocket = (Socket)result.AsyncState;

            clientSocket.EndConnect(result);

            _clientConnected.Set();
        }

        static void SendData(string data, Socket clientSocket)
        {
            byte[] bData = Encoding.ASCII.GetBytes(data);

            clientSocket.BeginSend(bData, 0, bData.Length, SocketFlags.None,
                SendDataCompleted, clientSocket);
        }

        static void SendDataCompleted(IAsyncResult result)
        {

            Socket clientSocket = (Socket)result.AsyncState;

            int bytesSent = clientSocket.EndSend(result);

            Console.WriteLine("Bytes sent:" + bytesSent);

            _sendCompleted.Set();
        }

        
        static void ReceiveData(Socket clientSocket)
        {
            ConnectionData data = new ConnectionData(clientSocket);
            clientSocket.BeginReceive(data.Data, 0, 10, SocketFlags.None,
                ReceiveDataCompleted, data);
        }

        static void ReceiveDataCompleted(IAsyncResult result)
        {
            ConnectionData data = (ConnectionData)result.AsyncState;

            int bytesRead = data.Socket.EndReceive(result);
            if (bytesRead > 0)
            {
                data.Message.Append(Encoding.ASCII.GetString(data.Data,
                    0, bytesRead));

                if (!data.Message.ToString().EndsWith("<E>"
                    , StringComparison.OrdinalIgnoreCase))
                {
                    data.Socket.BeginReceive(data.Data, 0, 10
                        , SocketFlags.None
                        , ReceiveDataCompleted, data);
                    return;
                }
            }
            Console.WriteLine(data.Message);

            _receiveCompleted.Set();            
        }

        #endregion

        #endregion

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                StartClient();
            }
            else
            {
                ThreadPool.QueueUserWorkItem(StartServer);

                Console.WriteLine("Press any key to stop server.");

                Console.ReadKey();

                _shutDown.Set();

                Console.ReadKey();
            }
        }

        #region Raja

        //static int[,] map1 = new int[5, 5];

        //static Program()
        //{
        //    /*
        //       { 1, 1, 1, 1, 0 }, 
        //       { 0, 1, 0, 0, 0 }, 
        //       { 1, 1, 0, 0, 0 }, 
        //       { 0, 1, 1, 1, 0 }, 
        //       { 1, 0, 0, 1, 1 }
        //    */
        //    map1[0, 0] = 1;
        //    map1[0, 1] = 1;
        //    map1[0, 2] = 1;
        //    map1[0, 3] = 1;
        //    map1[0, 4] = 1;
        //    map1[1, 0] = 0;
        //    map1[1, 1] = 0;
        //    map1[1, 2] = 0;
        //    map1[1, 3] = 0;
        //    map1[1, 4] = 1;
        //    map1[2, 0] = 1;
        //    map1[2, 1] = 1;
        //    map1[2, 2] = 0;
        //    map1[2, 3] = 0;
        //    map1[2, 4] = 1;
        //    map1[3, 0] = 0;
        //    map1[3, 1] = 1;
        //    map1[3, 2] = 1;
        //    map1[3, 3] = 1;
        //    map1[3, 4] = 1;
        //    map1[4, 0] = 1;
        //    map1[4, 1] = 0;
        //    map1[4, 2] = 0;
        //    map1[4, 3] = 1;
        //    map1[4, 4] = 1;
        //}

        //static void Main(string[] args)
        //{

        //    Stack<Tuple<int, int>> Path = new Stack<Tuple<int, int>>();
        //    List<string> Visited = new List<string>();

        //    int maxCol = 5, maxRow = 5;
        //    int[,] input = map1;
        //    if (input[0, 0] == 0)
        //    {
        //        Console.WriteLine("No path found.");
        //        return;
        //    }

        //    Path.Push(new Tuple<int, int>(0, 0));
        //    Visited.Add("0,0");

        //    while (Path.Count > 0 &&
        //        (Path.Peek().Item1 != (maxCol - 1) ||
        //        Path.Peek().Item2 != (maxRow - 1)))
        //    {
        //        Tuple<int, int> top = Path.Peek();
        //        string colInc = string.Concat(top.Item1, ",", top.Item2 + 1);
        //        string rowInc = string.Concat(top.Item1 + 1, ",", top.Item2);

        //        if ((top.Item2 + 1) < maxCol &&
        //            !Visited.Contains(colInc) &&
        //            input[top.Item1, top.Item2 + 1] == 1)
        //        {
        //            var t = new Tuple<int, int>(top.Item1, top.Item2 + 1);
        //            Path.Push(t);
        //            Visited.Add(colInc);
        //        }
        //        else if ((top.Item1 + 1) < maxRow &&
        //            !Visited.Contains(rowInc) &&
        //            input[top.Item1 + 1, top.Item2] == 1)
        //        {
        //            Path.Push(new Tuple<int, int>(top.Item1 + 1, top.Item2));
        //            Visited.Add(rowInc);
        //        }
        //        else
        //        {
        //            Path.Pop();
        //        }
        //    }
        //    if (Path.Count == 0)
        //    {
        //        Console.WriteLine("No path found.");
        //    }
        //    else
        //    {
        //        Stack<Tuple<int, int>> orderedPath = new Stack<Tuple<int, int>>();
        //        while (Path.Count > 0)
        //        {
        //            var top = Path.Pop();
        //            orderedPath.Push(top);
        //        }
        //        foreach (var index in orderedPath)
        //        {
        //            Console.Write("[{0}, {1}] ", index.Item1, index.Item2);
        //        }
        //    }

        //    Console.ReadKey();
        //}

        #endregion

    }
}