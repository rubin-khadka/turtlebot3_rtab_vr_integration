using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Sensor;
using TMPro;

public class LidarScanSubscriber : MonoBehaviour
{
    [Header("ROS Settings")]
    public string scanTopic = "/scan";           // LiDAR scan topic to subscribe to
    
    [Header("UI References")]
    public TMP_Text forwardText;                 // Distance display in front of robot (0°)
    public TMP_Text leftText;                    // Distance display to left (90°)
    public TMP_Text rightText;                   // Distance display to right (-90°)
    public TMP_Text backText;                    // Distance display behind (180°)
    
    [Header("Settings")]
    public float maxDistance = 5.0f;             // Maximum distance to display (anything further shows "> Xm")
    
    [Header("Color Settings")]
    public Color safeColor = Color.green;                    // >2m distance
    public Color warningColor = new Color(1f, 0.5f, 0f);    // 1-2m distance (orange)
    public Color dangerColor = Color.yellow;                 // 0.5-1m distance
    public Color criticalColor = Color.red;                  // <0.5m distance (immediate danger)
    
    [Header("ROS Connection Monitor")]
    public ROSConnectionMonitor connectionMonitor;  // Reference to track message freshness
    
    private ROSConnection ros;
    private LaserScanMsg latestScan;               // Most recent complete scan data
    private bool hasNewScan = false;               // Flag for new scan received this frame
    
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<LaserScanMsg>(scanTopic, ReceiveLaserScan);
        
        Debug.Log($"LaserScanSubscriber started. Listening to: {scanTopic}");
        
        // Show placeholder text until first scan arrives
        SetInitialText();
    }
    
    private void SetInitialText()
    {
        if (forwardText != null) forwardText.text = "FORWARD: --";
        if (leftText != null) leftText.text = "LEFT: --";
        if (rightText != null) rightText.text = "RIGHT: --";
        if (backText != null) backText.text = "BACK: --";
    }
    
    private void ReceiveLaserScan(LaserScanMsg scanMsg)
    {
        // Store latest scan data
        latestScan = scanMsg;
        hasNewScan = true;
        
        // Notify connection monitor that ROS data is flowing
        if (connectionMonitor != null)
            connectionMonitor.RecordMessageReceived();
    }
    
    void Update()
    {
        // Only update display when new scan data arrives (not every frame)
        if (hasNewScan && latestScan != null)
        {
            UpdateDistanceDisplay();
            hasNewScan = false;
        }
    }
    
    private void UpdateDistanceDisplay()
    {
        // Extract distances at cardinal directions
        float forward = GetDistanceAtAngle(0f);    // Front of robot
        float left = GetDistanceAtAngle(90f);      // Left side
        float right = GetDistanceAtAngle(-90f);    // Right side
        float back = GetDistanceAtAngle(180f);     // Behind robot
        
        // Update each UI text element with distance and color
        UpdateText(forwardText, forward);
        UpdateText(leftText, left);
        UpdateText(rightText, right);
        UpdateText(backText, back);
    }
    
    private float GetDistanceAtAngle(float angleDegrees)
    {
        if (latestScan.ranges.Length == 0)
            return maxDistance;  // Return max if no scan data
        
        // Convert angle to radians and find corresponding array index
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        int index = Mathf.RoundToInt((angleRad - latestScan.angle_min) / latestScan.angle_increment);
        index = Mathf.Clamp(index, 0, latestScan.ranges.Length - 1);
        
        float distance = latestScan.ranges[index];
        
        // Filter invalid readings (infinity, NaN, or below minimum range)
        if (float.IsInfinity(distance) || float.IsNaN(distance) || 
            distance < latestScan.range_min)
        {
            return maxDistance;  // Treat invalid readings as clear path
        }
        
        return distance;
    }
    
    private void UpdateText(TMP_Text textElement, float distance)
    {
        if (textElement == null) return;
        
        string displayText;
        Color textColor;
        
        // Format distance display (show "> 5.0m" for clear paths beyond max range)
        if (distance >= maxDistance)
        {
            displayText = $"> {maxDistance:F1}m";
            textColor = safeColor;
        }
        else
        {
            displayText = $"{distance:F2}m";
            
            // Color code based on danger level
            if (distance < 0.5f)
                textColor = criticalColor;    // Immediate collision risk
            else if (distance < 1.0f)
                textColor = dangerColor;      // Very close
            else if (distance < 2.0f)
                textColor = warningColor;     // Getting close
            else
                textColor = safeColor;        // Safe distance
        }
        
        // Preserve direction label (FORWARD/LEFT/RIGHT/BACK) and update distance
        string direction = GetDirectionFromText(textElement);
        textElement.text = $"{direction}: {displayText}";
        
        textElement.color = textColor;
    }
    
    // Extract direction label from text element reference
    private string GetDirectionFromText(TMP_Text textElement)
    {
        if (textElement == forwardText) return "FORWARD";
        if (textElement == leftText) return "LEFT";
        if (textElement == rightText) return "RIGHT";
        if (textElement == backText) return "BACK";
        return "";
    }
}