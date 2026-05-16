using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using TMPro;
using UnityEngine.UI;

public class ROSConnectionMonitor : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text connectionStatusText;   // Shows connection state (Connected/Disconnected)
    public Image connectionIndicator;        // Visual indicator with status color
    public TMP_Text lastMessageText;         // Shows data freshness (Active/Delayed/Stale)
    
    [Header("Settings")]
    public float updateInterval = 0.5f;     // How often to check connection status
    
    [Header("Status Messages")]
    public string connectingMessage = "Connecting";
    public string connectedMessage = "Connected";
    public string disconnectedMessage = "Disconnected";
    
    [Header("Colors")]
    public Color connectingColor = Color.yellow;
    public Color connectedColor = Color.green;
    public Color disconnectedColor = Color.red;
    
    private ROSConnection ros;
    private float lastMessageTime = 0f;     // Timestamp of last received ROS message
    private float timer = 0f;               // Timer for periodic connection checks
    private bool isConnected = false;       // Current connection state
    
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        lastMessageTime = Time.time;
        UpdateUI(false, connectingMessage, connectingColor);  // Show initial connecting state
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        // Check connection at regular intervals
        if (timer >= updateInterval)
        {
            timer = 0f;
            CheckConnection();
        }
        
        // Update message freshness indicator
        if (lastMessageText != null && isConnected)
        {
            UpdateLastMessageDisplay();
        }
    }
    
    void CheckConnection()
    {
        bool currentlyConnected = !ros.HasConnectionError;
        
        // Only update UI when connection state changes
        if (currentlyConnected != isConnected)
        {
            isConnected = currentlyConnected;
            
            if (isConnected)
            {
                Debug.Log("[ROS Monitor] Connected to ROS!");
                lastMessageTime = Time.time;
                UpdateUI(true, connectedMessage, connectedColor);
            }
            else
            {
                Debug.LogWarning("[ROS Monitor] Disconnected from ROS!");
                UpdateUI(false, disconnectedMessage, disconnectedColor);
            }
        }
    }
    
    void UpdateUI(bool connected, string message, Color color)
    {
        // Update status text and color
        if (connectionStatusText != null)
        {
            connectionStatusText.text = message;
            connectionStatusText.color = color;
        }
        
        // Update indicator color
        if (connectionIndicator != null)
        {
            connectionIndicator.color = color;
        }
    }
    
    // Call this from subscribers to track incoming data freshness
    public void RecordMessageReceived()
    {
        if (isConnected)
        {
            lastMessageTime = Time.time;
            
            if (lastMessageText != null)
            {
                lastMessageText.text = "Active";
                lastMessageText.color = connectedColor;
            }
        }
    }
    
    void UpdateLastMessageDisplay()
    {
        float timeSinceLast = Time.time - lastMessageTime;
        
        // Show data freshness: Active (<1s), Delayed (1-3s), Stale (>3s)
        if (timeSinceLast < 1f)
        {
            lastMessageText.text = "● Active";
            lastMessageText.color = connectedColor;
        }
        else if (timeSinceLast < 3f)
        {
            lastMessageText.text = "● Delayed";
            lastMessageText.color = connectingColor;
        }
        else
        {
            lastMessageText.text = "● Stale";
            lastMessageText.color = disconnectedColor;
        }
    }
}