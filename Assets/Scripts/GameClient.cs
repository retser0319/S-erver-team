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

    public int PlayerId { get; private set; } = -1;

    [Header("Player Spawn Settings")]
    [SerializeField] private GameObject playerPrefab;      // 플레이어 프리팹
    [SerializeField] private Transform[] spawnPoints;      // 0~3 -> 1P~4P 위치

    private GameObject localPlayer;

    // 🔹 스레드에서 받은 메시지를 임시로 저장할 큐
    private readonly Queue<string> messageQueue = new Queue<string>();
    private readonly object queueLock = new object();

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

            // 수신 전용 스레드 시작
            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] Connection failed: {e.Message}");
        }
    }

    // 🔹 메인 스레드: 큐에 쌓인 메시지를 꺼내 처리
    void Update()
    {
        // 큐에서 메시지 뽑아서 처리
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
                    // 여기서는 Debug.Log 정도는 보통 되긴 하지만
                    // 안정성을 위해서도 최소한으로만 사용
                    Debug.LogWarning("[CLIENT] Disconnected from server.");
                    break;
                }

                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[] lines = msg.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                // 🔹 여기서는 "문자열만 큐에 넣기"
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

    // 🔹 이 함수는 반드시 메인 스레드(Update에서)만 호출됨
    void HandleServerMessage(string msg)
    {
        // 슬롯 배정 메시지: ASSIGN:1
        if (msg.StartsWith("ASSIGN:"))
        {
            string numStr = msg.Substring("ASSIGN:".Length);
            if (int.TryParse(numStr, out int id))
            {
                PlayerId = id;
                Debug.Log($"[CLIENT] Assigned as Player {PlayerId}");
                SpawnLocalPlayer();
            }
            return;
        }

        // 방이 꽉 찼을 때
        if (msg == "FULL")
        {
            Debug.LogWarning("[CLIENT] Room is full. Disconnect.");
            stream?.Close();
            client?.Close();
            return;
        }

        // 나머지 서버 메시지
        Debug.Log($"[SERVER] {msg}");
    }

    void SpawnLocalPlayer()
    {
        if (localPlayer != null) return;
        if (playerPrefab == null)
        {
            Debug.LogError("[CLIENT] playerPrefab is not set!");
            return;
        }
        if (spawnPoints == null || spawnPoints.Length < PlayerId)
        {
            Debug.LogError("[CLIENT] spawnPoints not set properly!");
            return;
        }

        Transform spawnPos = spawnPoints[PlayerId - 1];

        // 여기서는 메인 스레드라 Instantiate/transform 사용 가능
        localPlayer = Instantiate(playerPrefab, spawnPos.position, spawnPos.rotation);

        var ctl = localPlayer.GetComponent<Ctl_Player>();
        if (ctl != null)
        {
            ctl.isLocal = true; // 이 클라이언트의 플레이어
        }

        Debug.Log($"[CLIENT] Spawned local player at slot {PlayerId}");
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
            stream?.Close();
            client?.Close();
            receiveThread?.Abort();
        }
        catch { }
    }
}