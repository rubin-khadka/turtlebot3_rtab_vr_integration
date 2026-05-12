using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nav;

public class RobotMinimap : MonoBehaviour
{
    [Header("UI References")]
    public RawImage minimapImage;
    public RectTransform robotDot;
    
    [Header("Map Metadata (from YAML)")]
    public float resolution = 0.03f;
    public float originX = -3.45481f;  // ROS world X of bottom-left map corner
    public float originY = -4.27063f;  // ROS world Y of bottom-left map corner
    
    [Header("VR Options")]
    public Transform vrCamera;
    public bool faceCamera = true;
    
    private ROSConnection ros;
    private Vector2 robotPos2D;  // Only X/Y for 2D map
    private float robotYaw;
    private bool hasData = false;
    private int mapWidth, mapHeight;
    
    // Your intentional canvas size
    private const float CANVAS_SIZE = 700f;
    
    void Start()
    {
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
        
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OdometryMsg>("/odom", OnOdometryReceived);
    }
    
    void Update()
    {
        if (faceCamera && vrCamera != null && minimapImage != null)
        {
            minimapImage.transform.LookAt(vrCamera.position);
            minimapImage.transform.rotation = Quaternion.Euler(0, minimapImage.transform.eulerAngles.y, 0);
        }
        
        if (hasData && robotDot != null)
            UpdateMinimap();
    }
    
    void OnOdometryReceived(OdometryMsg msg)
    {
        // RAW world-frame position (no FLU conversion)
        robotPos2D = new Vector2(
            (float)msg.pose.pose.position.x,
            (float)msg.pose.pose.position.y
        );
        
        // Extract yaw (rotation around Z-axis in ROS)
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
        float pixelX = (robotPos2D.x - originX) / resolution;
        float pixelY = (robotPos2D.y - originY) / resolution;
        
        float uiX = (pixelX / mapWidth - 0.5f) * CANVAS_SIZE;
        float uiY = (pixelY / mapHeight - 0.5f) * CANVAS_SIZE;
        
        uiX = Mathf.Clamp(uiX, -CANVAS_SIZE * 0.5f, CANVAS_SIZE * 0.5f);
        uiY = Mathf.Clamp(uiY, -CANVAS_SIZE * 0.5f, CANVAS_SIZE * 0.5f);
        
        robotDot.anchoredPosition = new Vector2(uiX, uiY);
        
        robotDot.localRotation = Quaternion.Euler(0f, 0f, -robotYaw);
    }
}