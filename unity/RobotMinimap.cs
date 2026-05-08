using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Nav;

public class RobotMinimap : MonoBehaviour
{
    [Header("UI References")]
    public RawImage minimapImage;
    public RectTransform robotDot;
    
    [Header("Map Metadata")]
    public float resolution = 0.03f;
    public float originX = -3.45481f;
    public float originY = -4.27063f;
    
    [Header("Robot")]
    public Transform robot;
    
    private ROSConnection ros;
    private Vector3 robotPosition;
    private Quaternion robotRotation;
    private bool hasData = false;
    private int mapWidth;
    private int mapHeight;
    
    void Start()
    {
        if (minimapImage != null && minimapImage.texture != null)
        {
            mapWidth = minimapImage.texture.width;
            mapHeight = minimapImage.texture.height;
            Debug.Log($"Map size: {mapWidth} x {mapHeight}");
        }
        
        RectTransform rect = minimapImage.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(700, 700);
        
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OdometryMsg>("/odom", OnOdometryReceived);
    }
    
    void OnOdometryReceived(OdometryMsg msg)
    {
        robotPosition = msg.pose.pose.position.From<FLU>();
        robotRotation = msg.pose.pose.orientation.From<FLU>();
        hasData = true;
    }
    
    void Update()
    {
        if (hasData && robotDot != null)
        {
            UpdateMinimap();
        }
    }
    
    void UpdateMinimap()
    {
        // POSITION - Swap X and Z
        float pixelX = (robotPosition.z - originX) / resolution;
        float pixelY = (robotPosition.x - originY) / resolution;
        
        float uiX = (pixelX / mapWidth - 0.5f) * 700f;
        float uiY = (pixelY / mapHeight - 0.5f) * 700f;
        
        robotDot.anchoredPosition = new Vector2(uiX, uiY);
        
        // ROTATION - Add 180° to flip the arrow direction
        float angle = robotRotation.eulerAngles.y;
        float uiAngle = angle + 180f;  // This flips the arrow to point correctly
        
        robotDot.rotation = Quaternion.Euler(0, 0, uiAngle);
    }
}