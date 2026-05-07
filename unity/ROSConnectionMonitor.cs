using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using TMPro;
using UnityEngine.UI;

public class ROSConnectionMonitor : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text connectionStatusText;
    public Image connectionIndicator;
    public TMP_Text lastMessageText;   
    
    [Header("Settings")]
    public float updateInterval = 0.5f;
    
    [Header("Status Messages")]
    public string connectingMessage = "Connecting";
    public string connectedMessage = "Connected";
    public string disconnectedMessage = "Disconnected";
    
    [Header("Colors")]
    public Color connectingColor = Color.yellow;
    public Color connectedColor = Color.green;
    public Color disconnectedColor = Color.red;
    
    private ROSConnection ros;
    private float lastMessageTime = 0f;
    private float timer = 0f;
    private bool isConnected = false;
    
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        lastMessageTime = Time.time;
        UpdateUI(false, connectingMessage, connectingColor);
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= updateInterval)
        {
            timer = 0f;
            CheckConnection();
        }
        
        // Update last message display if enabled
        if (lastMessageText != null && isConnected)
        {
            UpdateLastMessageDisplay();
        }
    }
    
    void CheckConnection()
    {
        bool currentlyConnected = !ros.HasConnectionError;
        
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
        if (connectionStatusText != null)
        {
            connectionStatusText.text = message;
            connectionStatusText.color = color;
        }
        
        if (connectionIndicator != null)
        {
            connectionIndicator.color = color;
        }
    }
    
    // Call this method from your laser scan subscriber when data arrives
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