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
    public static int WaveOwnerSlot = -1;
    public static GameClient Instance { get; private set; }

    public int PlayerId { get; private set; } = -1;

    [Header("Player Spawn Settings")]
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Gameplay Sync Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Tile_Manager tileManager;
    [SerializeField] private Round_Manager roundManager;

    private GameObject localPlayer;

    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    private readonly Queue<string> messageQueue = new Queue<string>();
    private readonly object queueLock = new object();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (tileManager == null)
            tileManager = FindObjectOfType<Tile_Manager>();
        if (roundManager == null)
            roundManager = FindObjectOfType<Round_Manager>();
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

        if (msg.StartsWith("WAVE:START:"))
        {
            var parts = msg.Split(':');
            int ownerSlot;

            if (parts.Length >= 3 && int.TryParse(parts[2], out ownerSlot))
            {
                WaveOwnerSlot = ownerSlot;
                Debug.Log($"[CLIENT] WAVE:START from server. Owner = P{ownerSlot}");

                if (roundManager != null)
                {
                    roundManager.StartWaveFromNetwork(ownerSlot);
                }
                else
                {
                    Debug.LogWarning("[CLIENT] roundManager is null, cannot start wave.");
                }
            }
            else
            {
                Debug.LogError(
                    $"[CLIENT] Failed to parse WAVE:START message: {msg}  " +
                    $"(parts.Length = {parts.Length})"
                );
            }

            return;
        }

        if (msg.StartsWith("ENEMY:SPAWN:"))
        {
            var parts = msg.Split(':');
            if (parts.Length >= 5)
            {
                string name = parts[2];
                if (float.TryParse(parts[3], out float x) &&
                    float.TryParse(parts[4], out float y))
                {
                    if (roundManager != null)
                    {
                        if (GameClient.LocalPlayerId == GameClient.WaveOwnerSlot)
                            return;
                        roundManager.SpawnEnemyFromNetwork(name, new Vector2(x, y));
                    }
                }
            }
            return;
        }

        if (msg.StartsWith("FIRE:"))
        {
            var parts = msg.Split(':');
            if (parts.Length >= 5 &&
                int.TryParse(parts[1], out int shooterId) &&
                float.TryParse(parts[2], out float x) &&
                float.TryParse(parts[3], out float y) &&
                float.TryParse(parts[4], out float angle))
            {
                SpawnBullet(shooterId, new Vector3(x, y, 0f), angle);
            }
            return;
        }

        if (msg.StartsWith("TILE:"))
        {
            var parts = msg.Split(':');
            if (parts.Length >= 4 &&
                int.TryParse(parts[1], out int x) &&
                int.TryParse(parts[2], out int y) &&
                int.TryParse(parts[3], out int type))
            {
                ApplyTileChange(x, y, type);
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
        if (msg.StartsWith("ENEMY:DEAD:"))
        {
            var parts = msg.Split(':');
            if (parts.Length >= 5 &&
                int.TryParse(parts[2], out int income) &&
                float.TryParse(parts[3], out float x) &&
                float.TryParse(parts[4], out float y))
            {
                Vector2 pos = new Vector2(x, y);
                HandleEnemyDeadFromNetwork(income, pos);
            }
            else
            {
                Debug.LogWarning($"[CLIENT] Failed to parse ENEMY:DEAD message: {msg}");
            }
            return;
        }
        if (msg.StartsWith("COIN:TAKEN:"))
        {
            var parts = msg.Split(':');
            if (parts.Length >= 6 &&
                int.TryParse(parts[2], out int slot) &&
                int.TryParse(parts[3], out int amount) &&
                float.TryParse(parts[4], out float x) &&
                float.TryParse(parts[5], out float y))
            {
                Vector2 pos = new Vector2(x, y);

                //Destroy(gameObject);

                var gm = FindObjectOfType<Game_Manager>();
                if (gm != null)
                {
                    gm.AddCoin(slot, amount);
                }
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

        if (spawnPoints == null)
        {
            Debug.LogError("[CLIENT] spawnPoints is NULL!");
            return;
        }

        Debug.Log($"[CLIENT] SpawnLocalPlayer() called. PlayerId = {PlayerId}, spawnPoints.Length = {spawnPoints.Length}");

        if (PlayerId <= 0 || PlayerId > spawnPoints.Length)
        {
            Debug.LogError($"[CLIENT] Invalid PlayerId {PlayerId} or spawnPoints not set properly! spawnPoints.Length = {spawnPoints.Length}");
            return;
        }

        Transform spawnPos = spawnPoints[PlayerId - 1];

        GameObject prefabToUse = playerPrefabs[PlayerId - 1];

        localPlayer = Instantiate(prefabToUse, spawnPos.position, spawnPos.rotation);

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

        Vector3 pos = Vector3.zero;
        if (spawnPoints != null && spawnPoints.Length >= id)
        {
            pos = spawnPoints[id - 1].position;
        }

        GameObject prefabToUse = playerPrefabs[id - 1];

        GameObject remote = Instantiate(prefabToUse, pos, Quaternion.identity);
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

    void SpawnBullet(int shooterId, Vector3 pos, float angleZ)
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("[CLIENT] bulletPrefab is not set!");
            return;
        }

        Instantiate(bulletPrefab, pos, Quaternion.Euler(0f, 0f, angleZ));
    }

    void ApplyTileChange(int x, int y, int type)
    {
        if (tileManager == null)
            tileManager = FindObjectOfType<Tile_Manager>();

        if (tileManager == null)
        {
            Debug.LogError("[CLIENT] Tile_Manager not found in scene.");
            return;
        }

        tileManager.TileChange(x, y, type);
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

    public void SendFire(int p, Vector3 pos, float angleZ)
    {
        if (p != PlayerId) return;
        if (stream == null) return;

        string msg = $"FIRE:{pos.x:F3}:{pos.y:F3}:{angleZ:F3}";
        byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
        try
        {
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] SendFire error: {e.Message}");
        }
    }

    public void SendTileChange(int p, int x, int y, int type)
    {
        if (p != PlayerId) return;
        if (stream == null) return;

        string msg = $"TILE:{x}:{y}:{type}";
        byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
        try
        {
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] SendTileChange error: {e.Message}");
        }
    }

    public void SendWaveStart()
    {
        if (stream == null) return;

        string msg = "WAVE:REQ";
        byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
        try
        {
            stream.Write(data, 0, data.Length);
            Debug.Log("[CLIENT] Sent WAVE:REQ");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] SendWaveStart error: {e.Message}");
        }
    }

    public void SendEnemySpawn(string name, Vector2 pos)
    {
        if (stream == null) return;

        string msg = $"ENEMY:SPAWN:{name}:{pos.x:F3}:{pos.y:F3}";
        byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
        try
        {
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] SendEnemySpawn error: {e.Message}");
        }
    }
    public void SendEnemyDead(int income, Vector2 pos)
    {
        if (stream == null) return;

        string msg = $"ENEMY:DEAD:{income}:{pos.x:F3}:{pos.y:F3}";
        byte[] data = Encoding.UTF8.GetBytes(msg + "\n");

        try
        {
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] SendEnemyDead error: {e.Message}");
        }
    }
    public void SendCoinTaken(int p, int amount, Vector2 pos)
    {
        if (stream == null) return;
        if (p != PlayerId) return;

        string msg = $"COIN:TAKEN:{p}:{amount}:{pos.x:F3}:{pos.y:F3}";
        byte[] data = Encoding.UTF8.GetBytes(msg + "\n");

        try
        {
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] SendCoinTaken error: {e.Message}");
        }
    }
    private void HandleEnemyDeadFromNetwork(int income, Vector2 pos)
    {
        // 씬에 있는 모든 Enemy 검색
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();

        Enemy target = null;
        float minDist = float.MaxValue;

        foreach (var e in allEnemies)
        {
            float d = Vector2.Distance(pos, e.transform.position);
            if (d < minDist)
            {
                minDist = d;
                target = e;
            }
        }

        // 너무 멀면 (예: 2유닛 이상) 그냥 무시
        if (target == null || minDist > 2f)
        {
            Debug.LogWarning($"[CLIENT] ENEMY:DEAD but no enemy found near {pos} (minDist={minDist})");
            return;
        }

        // Enemy 안에 LocalDead()를 만들어 두고 여기서 호출
        target.LocalDead();
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
