using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Player
{
    public int Id;
    public TcpClient Client;
    public NetworkStream Stream;

    public Player(int id, TcpClient client)
    {
        Id = id;
        Client = client;
        Stream = client.GetStream();
    }
}

class GameServer
{
    private TcpListener listener;
    private Dictionary<int, Player> players = new Dictionary<int, Player>();
    private int port = 9000;

    public void Start()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"[SERVER] Listening on port {port}...");
        listener.BeginAcceptTcpClient(OnClientConnected, null);
    }

    private void OnClientConnected(IAsyncResult ar)
    {
        TcpClient client = listener.EndAcceptTcpClient(ar);
        int id = client.Client.RemoteEndPoint.GetHashCode();
        Player player = new Player(id, client);
        players[id] = player;

        Console.WriteLine($"[SERVER] Player {id} connected.");
        listener.BeginAcceptTcpClient(OnClientConnected, null);

        StartReceive(player);
    }

    private void StartReceive(Player player)
    {
        byte[] buffer = new byte[1024];
        player.Stream.BeginRead(buffer, 0, buffer.Length, (ar) =>
        {
            int bytesRead = player.Stream.EndRead(ar);
            if (bytesRead <= 0)
            {
                Console.WriteLine($"[SERVER] Player {player.Id} disconnected.");
                players.Remove(player.Id);
                return;
            }

            string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            Console.WriteLine($"[SERVER] Received from {player.Id}: {msg}");

            Broadcast($"Player {player.Id}: {msg}");

            StartReceive(player); // 계속 수신
        }, null);
    }

    private void Broadcast(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\n");
        foreach (var p in players.Values)
        {
            p.Stream.Write(data, 0, data.Length);
        }
    }

    static void Main(string[] args)
    {
        new GameServer().Start();
        while (true) Thread.Sleep(100);
    }
}
