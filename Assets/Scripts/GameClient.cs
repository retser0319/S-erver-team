using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class GameClient : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    Thread receiveThread;
    public static int LocalPlayerId = -1;
    public static GameClient Instance { get; private set; }

    public int PlayerId { get; private set; } = -1;

    [Header("Player Spawn Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private GameObject localPlayer;

    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    private readonly Queue<string> messageQueue = new Queue<string>();
    private readonly object queueLock = new object();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        ConnectToServer("127.0.0.1", 9000);
    }

    void ConnectToServer(string ip, int port)
    {
        try
        {
            client = new TcpClient();
            client.Connect(ip, port);
            stream = client.GetStream();
            Debug.Log("[CLIENT] Connected to server!");

            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] Connection failed: {e.Message}");
        }
    }

    void Update()
    {
        lock (queueLock)
        {
            while (messageQueue.Count > 0)
            {
                string msg = messageQueue.Dequeue();
                HandleServerMessage(msg);
            }
        }
    }

    void ReceiveData()
    {
        try
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0)
                {
                    Debug.LogWarning("[CLIENT] Disconnected from server.");
                    break;
                }

                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[] lines = msg.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                lock (queueLock)
                {
                    foreach (var raw in lines)
                    {
                        string line = raw.Trim();
                        messageQueue.Enqueue(line);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] Receive error: {e.Message}");
        }
    }

    void HandleServerMessage(string msg)
    {
        if (msg.StartsWith("ASSIGN:"))
        {
            string numStr = msg.Substring("ASSIGN:".Length);
            if (int.TryParse(numStr, out int id))
            {
                PlayerId = id;
                LocalPlayerId = id;

                Debug.Log($"[CLIENT] Assigned as Player {PlayerId}");

                SpawnLocalPlayer();
            }
            else
            {
                Debug.LogError($"[CLIENT] Failed to parse ASSIGN message: {msg}");
            }
            return;
        }

        if (msg.StartsWith("JOIN:"))
        {
            string numStr = msg.Substring("JOIN:".Length);
            if (int.TryParse(numStr, out int id))
            {
                Debug.Log($"[CLIENT] JOIN message for Player {id}");

                if (id == PlayerId)
                    return;

                SpawnRemotePlayer(id);
            }
            return;
        }

        if (msg.StartsWith("LEFT:"))
        {
            string numStr = msg.Substring("LEFT:".Length);
            if (int.TryParse(numStr, out int id))
            {
                Debug.Log($"[CLIENT] Player {id} left.");

                if (players.TryGetValue(id, out var go))
                {
                    Destroy(go);
                    players.Remove(id);
                }
            }
            return;
        }

        if (msg.StartsWith("POS:"))
        {
            var parts = msg.Split(':');
            if (parts.Length >= 5 &&
                int.TryParse(parts[1], out int id) &&
                float.TryParse(parts[2], out float x) &&
                float.TryParse(parts[3], out float y) &&
                float.TryParse(parts[4], out float angle))
            {
                UpdateRemotePlayerPosition(id, x, y, angle);
            }
            return;
        }

        Debug.Log($"[SERVER] {msg}");
    }

    void SpawnLocalPlayer()
    {
        if (localPlayer != null)
        {
            Debug.Log("[CLIENT] localPlayer already exists. Skip spawn.");
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("[CLIENT] playerPrefab is not set!");
            return;
        }

        if (spawnPoints == null)
        {
            Debug.LogError("[CLIENT] spawnPoints is NULL!");
            return;
        }

        Debug.Log($"[CLIENT] SpawnLocalPlayer() called. PlayerId = {PlayerId}, spawnPoints.Length = {spawnPoints.Length}");

        if (PlayerId <= 0 || PlayerId > spawnPoints.Length)
        {
            Debug.LogError($"[CLIENT] Invalid PlayerId {PlayerId} or spawnPoints not set properly! " +
                           $"spawnPoints.Length = {spawnPoints.Length}");
            return;
        }

        Transform spawnPos = spawnPoints[PlayerId - 1];

        localPlayer = Instantiate(playerPrefab, spawnPos.position, spawnPos.rotation);

        var ctl = localPlayer.GetComponent<Ctl_Player>();
        if (ctl != null)
        {
            ctl.P = PlayerId;
        }

        players[PlayerId] = localPlayer;

        Debug.Log($"[CLIENT] Spawned local player at slot {PlayerId}");
    }
    void SpawnRemotePlayer(int id)
    {
        if (players.ContainsKey(id))
        {
            Debug.Log($"[CLIENT] Remote player {id} already exists. Skip spawn.");
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("[CLIENT] playerPrefab is not set for remote!");
            return;
        }

        Vector3 pos = Vector3.zero;
        if (spawnPoints != null && spawnPoints.Length >= id)
        {
            pos = spawnPoints[id - 1].position;
        }

        GameObject remote = Instantiate(playerPrefab, pos, Quaternion.identity);
        var ctl = remote.GetComponent<Ctl_Player>();
        if (ctl != null)
        {
            ctl.P = id;
        }

        players[id] = remote;

        Debug.Log($"[CLIENT] Spawned remote player {id}");
    }

    void UpdateRemotePlayerPosition(int id, float x, float y, float angle)
    {
        if (id == PlayerId)
            return;

        if (!players.TryGetValue(id, out var go))
        {
            SpawnRemotePlayer(id);
            if (!players.TryGetValue(id, out go))
                return;
        }

        go.transform.position = new Vector3(x, y, go.transform.position.z);
        go.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void SendPosition(int p, Vector3 pos, float angleZ)
    {
        if (p != PlayerId) return;

        if (stream == null) return;

        string msg = $"POS:{pos.x:F3}:{pos.y:F3}:{angleZ:F3}";
        byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
        try
        {
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] SendPosition error: {e.Message}");
        }
    }

    public void SendMessageToServer(string msg)
    {
        if (stream == null) return;

        byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
        try
        {
            stream.Write(data, 0, data.Length);
            Debug.Log($"[CLIENT] Sent: {msg}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] Send error: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        try
        {
            if (stream != null)
            {
                byte[] data = Encoding.UTF8.GetBytes("QUIT\n");
                stream.Write(data, 0, data.Length);
            }

            stream?.Close();
            client?.Close();
            receiveThread?.Abort();
        }
        catch { }
    }
}