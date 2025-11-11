using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class GameClient : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    Thread receiveThread;
    volatile bool running;

    [Header("Chat UI")]
    public ChatUI chatUI;

    private readonly ConcurrentQueue<string> inbox = new ConcurrentQueue<string>();
    private readonly StringBuilder recvBuffer = new StringBuilder();

    void Start()
    {
        ConnectToServer("127.0.0.1", 9000);
    }

    void Update()
    {
        while (inbox.TryDequeue(out var line))
        {
            chatUI?.Append(line);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (chatUI != null)
            {
                var msg = chatUI.ConsumeInput();
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    SendMessageToServer(msg);
                }
            }
        }
    }

    void ConnectToServer(string ip, int port)
    {
        try
        {
            client = new TcpClient();
            client.Connect(ip, port);
            stream = client.GetStream();
            running = true;

            Debug.Log("[CLIENT] Connected to server!");
            chatUI?.Append("[SYSTEM] Connected to server.");

            receiveThread = new Thread(ReceiveLoop) { IsBackground = true };
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError($"[CLIENT] Connection failed: {e.Message}");
            chatUI?.Append($"[SYSTEM] Connection failed: {e.Message}");
        }
    }

    void ReceiveLoop()
    {
        byte[] buffer = new byte[2048];
        try
        {
            while (running)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0)
                {
                    inbox.Enqueue("[SYSTEM] Disconnected from server.");
                    break;
                }


                string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                recvBuffer.Append(chunk);

                int newline;
                while ((newline = recvBuffer.ToString().IndexOf('\n')) >= 0)
                {
                    string line = recvBuffer.ToString(0, newline).TrimEnd('\r');
                    inbox.Enqueue(line);
                    recvBuffer.Remove(0, newline + 1);
                }
            }
        }
        catch (Exception e)
        {
            inbox.Enqueue($"[SYSTEM] Receive error: {e.Message}");
        }
        finally
        {
            running = false;
        }
    }

    public void SendFromButton()
    {
        if (chatUI == null) return;
        var msg = chatUI.ConsumeInput();
        if (!string.IsNullOrWhiteSpace(msg))
        {
            SendMessageToServer(msg);
        }
    }

    public void SendMessageToServer(string msg)
    {
        if (stream == null || !stream.CanWrite) return;
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            inbox.Enqueue($"[SYSTEM] Send error: {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        running = false;
        try { stream?.Close(); } catch { }
        try { client?.Close(); } catch { }
        try { receiveThread?.Join(200); } catch { }
    }
}