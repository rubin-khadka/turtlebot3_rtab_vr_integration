using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nav;

public class RobotMinimap : MonoBehaviour
{
    [Header("UI References")]
    public RawImage minimapImage;      // The map texture display
    public RectTransform robotDot;     // Robot position indicator (child of minimap)
    
    [Header("Map Metadata (from YAML)")]
    public float resolution = 0.03f;   // Meters per pixel (from map YAML)
    public float originX = -3.45481f;  // ROS world X of bottom-left map corner
    public float originY = -4.27063f;  // ROS world Y of bottom-left map corner
    
    [Header("VR Options")]
    public Transform vrCamera;         // VR camera for billboarding effect
    public bool faceCamera = true;     // Keep minimap facing the user in VR
    
    private ROSConnection ros;
    private Vector2 robotPos2D;        // Robot's 2D position (X,Y from odometry)
    private float robotYaw;            // Robot's rotation around Z-axis
    private bool hasData = false;      // Flag for first odometry message received
    private int mapWidth, mapHeight;   // Map texture dimensions in pixels
    
    // Fixed canvas size for consistent UI positioning
    private const float CANVAS_SIZE = 700f;
    
    void Start()
    {
        // Get map dimensions from the assigned texture
        if (minimapImage?.texture != null)
        {
            mapWidth = minimapImage.texture.width;
            mapHeight = minimapImage.texture.height;
            Debug.Log($"Map: {mapWidth}x{mapHeight}px | Canvas: {CANVAS_SIZE}x{CANVAS_SIZE}");
        }
        else
        {
            Debug.LogError("Minimap texture not assigned!");
            return;
        }
        
        // Subscribe to robot odometry for real-time position updates
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OdometryMsg>("/odom", OnOdometryReceived);
    }
    
    void Update()
    {
        // Billboarding: keep minimap facing the VR camera (horizontal rotation only)
        if (faceCamera && vrCamera != null && minimapImage != null)
        {
            minimapImage.transform.LookAt(vrCamera.position);
            minimapImage.transform.rotation = Quaternion.Euler(0, minimapImage.transform.eulerAngles.y, 0);
        }
        
        // Update robot dot position once we have odometry data
        if (hasData && robotDot != null)
            UpdateMinimap();
    }
    
    void OnOdometryReceived(OdometryMsg msg)
    {
        // Extract 2D position from odometry (ignoring Z height)
        robotPos2D = new Vector2(
            (float)msg.pose.pose.position.x,
            (float)msg.pose.pose.position.y
        );
        
        // Extract yaw rotation from quaternion (Z-axis rotation in 2D plane)
        Quaternion q = new Quaternion(
            (float)msg.pose.pose.orientation.x,
            (float)msg.pose.pose.orientation.y,
            (float)msg.pose.pose.orientation.z,
            (float)msg.pose.pose.orientation.w
        );
        robotYaw = q.eulerAngles.z;
        
        hasData = true;
    }
    
    void UpdateMinimap()
    {
        // Convert ROS world coordinates to map pixel coordinates
        float pixelX = (robotPos2D.x - originX) / resolution;
        float pixelY = (robotPos2D.y - originY) / resolution;
        
        // Convert pixel coordinates to UI position (centered on canvas)
        float uiX = (pixelX / mapWidth - 0.5f) * CANVAS_SIZE;
        float uiY = (pixelY / mapHeight - 0.5f) * CANVAS_SIZE;
        
        // Clamp to keep robot dot within minimap bounds
        uiX = Mathf.Clamp(uiX, -CANVAS_SIZE * 0.5f, CANVAS_SIZE * 0.5f);
        uiY = Mathf.Clamp(uiY, -CANVAS_SIZE * 0.5f, CANVAS_SIZE * 0.5f);
        
        // Apply position and rotation to robot indicator
        robotDot.anchoredPosition = new Vector2(uiX, uiY);
        robotDot.localRotation = Quaternion.Euler(0f, 0f, -robotYaw);  // Negative for Unity's coordinate system
    }
}