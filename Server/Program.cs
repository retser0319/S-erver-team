using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Player
{
    public int Slot;
    public TcpClient Client;
    public NetworkStream Stream;

    public Player(int slot, TcpClient client)
    {
        Slot = slot;
        Client = client;
        Stream = client.GetStream();
    }
}

class GameServer
{
    private TcpListener listener;
    private Dictionary<int, Player> players = new Dictionary<int, Player>();
    private int port = 9000;

    private bool[] usedSlots = new bool[3];

    public void Start()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"[SERVER] Listening on port {port}...");
        listener.BeginAcceptTcpClient(OnClientConnected, null);
    }

    private int GetAvailableSlot()
    {
        for (int i = 0; i < usedSlots.Length; i++)
        {
            if (!usedSlots[i])
            {
                usedSlots[i] = true;
                return i + 1;
            }
        }
        return -1;
    }

    private void FreeSlot(int slot)
    {
        if (slot <= 0) return;
        if (slot - 1 < usedSlots.Length)
            usedSlots[slot - 1] = false;
    }

    private void OnClientConnected(IAsyncResult ar)
    {
        TcpClient client = null;

        try
        {
            client = listener.EndAcceptTcpClient(ar);

            listener.BeginAcceptTcpClient(OnClientConnected, null);

            int slot = GetAvailableSlot();
            if (slot == -1)
            {
                Console.WriteLine("[SERVER] Room is full. Rejecting client.");
                var tempStream = client.GetStream();
                byte[] fullMsg = Encoding.UTF8.GetBytes("FULL\n");
                tempStream.Write(fullMsg, 0, fullMsg.Length);
                client.Close();
                return;
            }
            var existingSlots = new List<int>(players.Keys);

            Player player = new Player(slot, client);
            players[slot] = player;

            Console.WriteLine($"[SERVER] Player {slot} connected.");

            Send(player, $"ASSIGN:{slot}");

            foreach (int s in existingSlots)
            {
                Send(player, $"JOIN:{s}");
            }

            BroadcastExcept(player, $"JOIN:{slot}");

            StartReceive(player);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[SERVER] OnClientConnected error: {e.Message}");
            client?.Close();
        }
    }

    private void StartReceive(Player player)
    {
        byte[] buffer = new byte[1024];

        player.Stream.BeginRead(buffer, 0, buffer.Length, (ar) =>
        {
            int bytesRead = 0;

            try
            {
                bytesRead = player.Stream.EndRead(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SERVER] Read error from Player {player.Slot}: {e.Message}");
                Disconnect(player);
                return;
            }

            if (bytesRead <= 0)
            {
                Console.WriteLine($"[SERVER] Player {player.Slot} disconnected.");
                Disconnect(player);
                return;
            }

            string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            string[] lines = msg.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var raw in lines)
            {
                string line = raw.Trim();

                // 🔹 클라이언트가 QUIT 보낸 경우: 바로 종료 처리
                if (line == "QUIT")
                {
                    Console.WriteLine($"[SERVER] Player {player.Slot} sent QUIT");
                    Disconnect(player);
                    return; // 이 플레이어에 대한 수신 루프 종료
                }

                if (!line.StartsWith("POS:"))
                    Console.WriteLine($"[SERVER] From P{player.Slot}: {line}");

                if (line.StartsWith("POS:"))
                {
                    var parts = line.Split(':');
                    if (parts.Length >= 4 &&
                        float.TryParse(parts[1], out float x) &&
                        float.TryParse(parts[2], out float y) &&
                        float.TryParse(parts[3], out float angle))
                    {
                        Broadcast($"POS:{player.Slot}:{x}:{y}:{angle}");
                    }
                }
                else
                {
                    Broadcast($"P{player.Slot}: {line}");
                }
            }

            StartReceive(player);

        }, null);
    }

    private void Disconnect(Player player)
    {
        if (players.ContainsKey(player.Slot))
            players.Remove(player.Slot);

        FreeSlot(player.Slot);

        try
        {
            player.Stream?.Close();
            player.Client?.Close();
        }
        catch { }

        Broadcast($"LEFT:{player.Slot}");

        Console.WriteLine($"[SERVER] Player {player.Slot} left.");
    }

    private void Send(Player player, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\n");
        try
        {
            player.Stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[SERVER] Send error to P{player.Slot}: {e.Message}");
        }
    }

    private void Broadcast(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\n");
        foreach (var p in players.Values)
        {
            try
            {
                p.Stream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SERVER] Broadcast error to P{p.Slot}: {e.Message}");
            }
        }
    }

    private void BroadcastExcept(Player except, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message + "\n");
        foreach (var p in players.Values)
        {
            if (p == except) continue;
            try
            {
                p.Stream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SERVER] BroadcastExcept error to P{p.Slot}: {e.Message}");
            }
        }
    }

    static void Main(string[] args)
    {
        new GameServer().Start();
        while (true) Thread.Sleep(100);
    }
}