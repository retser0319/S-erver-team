using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class GameClient : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    Thread receiveThread;

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
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] Connection failed: {e.Message}");
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
                Debug.Log($"[SERVER] {msg}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] Receive error: {e.Message}");
        }
    }

    public void SendMessageToServer(string msg)
    {
        if (stream == null) return;

        byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
        stream.Write(data, 0, data.Length);
        Debug.Log($"[CLIENT] Sent: {msg}");
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