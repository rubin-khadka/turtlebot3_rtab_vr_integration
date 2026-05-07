using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Sensor;
using TMPro;

public class LaserScanSubscriber : MonoBehaviour
{
    [Header("ROS Settings")]
    public string scanTopic = "/scan";
    
    [Header("UI References")]
    public TMP_Text forwardText;
    public TMP_Text leftText;
    public TMP_Text rightText;
    public TMP_Text backText;
    
    [Header("Settings")]
    public float maxDistance = 5.0f;
    
    [Header("Color Settings")]
    public Color safeColor = Color.green;
    public Color warningColor = new Color(1f, 0.5f, 0f); 
    public Color dangerColor = Color.yellow;
    public Color criticalColor = Color.red;
    
    [Header("ROS Connection Monitor")]
    public ROSConnectionMonitor connectionMonitor; 
    
    private ROSConnection ros;
    private LaserScanMsg latestScan;
    private bool hasNewScan = false;
    
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<LaserScanMsg>(scanTopic, ReceiveLaserScan);
        
        Debug.Log($"LaserScanSubscriber started. Listening to: {scanTopic}");
        
        // Set initial text
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
        latestScan = scanMsg;
        hasNewScan = true;
        
        if (connectionMonitor != null)
            connectionMonitor.RecordMessageReceived();
    }
    
    void Update()
    {
        if (hasNewScan && latestScan != null)
        {
            UpdateDistanceDisplay();
            hasNewScan = false;
        }
    }
    
    private void UpdateDistanceDisplay()
    {
        float forward = GetDistanceAtAngle(0f);
        float left = GetDistanceAtAngle(90f);
        float right = GetDistanceAtAngle(-90f);
        float back = GetDistanceAtAngle(180f);
        
        UpdateText(forwardText, forward);
        UpdateText(leftText, left);
        UpdateText(rightText, right);
        UpdateText(backText, back);
    }
    
    private float GetDistanceAtAngle(float angleDegrees)
    {
        if (latestScan.ranges.Length == 0)
            return maxDistance;
        
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        int index = Mathf.RoundToInt((angleRad - latestScan.angle_min) / latestScan.angle_increment);
        index = Mathf.Clamp(index, 0, latestScan.ranges.Length - 1);
        
        float distance = latestScan.ranges[index];
        
        if (float.IsInfinity(distance) || float.IsNaN(distance) || 
            distance < latestScan.range_min)
        {
            return maxDistance;
        }
        
        return distance;
    }
    
    private void UpdateText(TMP_Text textElement, float distance)
    {
        if (textElement == null) return;
        
        string displayText;
        Color textColor;
        
        if (distance >= maxDistance)
        {
            displayText = $"> {maxDistance:F1}m";
            textColor = safeColor;
        }
        else
        {
            displayText = $"{distance:F2}m";
            
            if (distance < 0.5f)
                textColor = criticalColor;
            else if (distance < 1.0f)
                textColor = dangerColor;
            else if (distance < 2.0f)
                textColor = warningColor;
            else
                textColor = safeColor;
        }
        
        // Preserve the direction label
        string direction = GetDirectionFromText(textElement);
        textElement.text = $"{direction}: {displayText}";
        
        textElement.color = textColor;
    }
    
    private string GetDirectionFromText(TMP_Text textElement)
    {
        if (textElement == forwardText) return "FORWARD";
        if (textElement == leftText) return "LEFT";
        if (textElement == rightText) return "RIGHT";
        if (textElement == backText) return "BACK";
        return "";
    }
}