using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

/// <summary>
/// Sends gem count and game status (alive/dead) to an ESP32 via UDP for display on an LCD.
/// Add this to the same GameObject as Person (or any object in the scene) and set the ESP32 IP.
/// </summary>
public class GameStatusSender : MonoBehaviour
{
    [Header("ESP32 / Display")]
    [Tooltip("IP address of the ESP32 on your WiFi (e.g. 192.168.1.100)")]
    public string esp32Ip = "192.168.1.100";
    [Tooltip("UDP port the ESP32 is listening on")]
    public int udpPort = 8888;
    [Tooltip("Send interval in seconds")]
    public float sendInterval = 0.2f;

    private UdpClient client;
    private Person person;
    private float nextSendTime;
    private int lastGems = -1;
    private bool lastAlive = true;

    void Start()
    {
        person = FindObjectOfType<Person>();
        if (person == null)
        {
            Debug.LogWarning("GameStatusSender: No Person found in scene. LCD updates will not work.");
            return;
        }
        try
        {
            client = new UdpClient();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("GameStatusSender: Could not create UDP client: " + e.Message);
        }
        nextSendTime = Time.realtimeSinceStartup + sendInterval;
    }

    void Update()
    {
        if (client == null || person == null) return;

        if (Time.realtimeSinceStartup < nextSendTime) return;

        int gems = person.GemsCollected;
        bool alive = person.IsAlive;
        // Send when values change or periodically to keep display in sync
        if (gems != lastGems || alive != lastAlive)
        {
            lastGems = gems;
            lastAlive = alive;
            SendStatus(gems, alive);
        }

        nextSendTime = Time.realtimeSinceStartup + sendInterval;
    }

    private void SendStatus(int gems, bool alive)
    {
        // Protocol: "G:2,S:1" = gems 2, status 1=alive 0=dead
        string payload = "G:" + gems + ",S:" + (alive ? "1" : "0");
        byte[] data = Encoding.ASCII.GetBytes(payload);
        try
        {
            client.Send(data, data.Length, esp32Ip, udpPort);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("GameStatusSender: Send failed: " + e.Message);
        }
    }

    void OnDestroy()
    {
        if (client != null)
        {
            try { client.Close(); } catch { }
            client = null;
        }
    }
}
