using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Nav;

public class OdomSubscriber : MonoBehaviour
{
    [Header("Robot Reference")]
    public GameObject robot;
    
    [Header("Smoothing Settings")]
    [SerializeField] private float smoothPosition = 0.3f;
    [SerializeField] private float smoothRotation = 0.3f;
    [SerializeField] private float positionThreshold = 0.001f;
    
    [Header("Y-Axis Locking (Prevents shaking)")]
    [SerializeField] private bool lockYPosition = true;      // Lock vertical position
    [SerializeField] private float fixedYHeight = 0.1f;     // Fixed height above ground
    [SerializeField] private bool lockXRotation = true;      // Lock roll
    [SerializeField] private bool lockZRotation = true;      // Lock tilt
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    private ROSConnection ros;
    private Vector3 initialPosition;
    private Quaternion initialOrientation;
    private bool initialPositionSet = false;
    private ArticulationBody robotArticulation;
    
    // For smoothing
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 lastReceivedPosition;
    private Quaternion lastReceivedRotation;
    
    void Start()
    {
        if (robot == null) robot = gameObject;
        robotArticulation = robot.GetComponent<ArticulationBody>();
        
        if (robotArticulation == null)
        {
            Debug.LogWarning("No ArticulationBody found, using Transform directly");
        }
        
        // Initialize target positions
        targetPosition = robot.transform.position;
        targetRotation = robot.transform.rotation;
        lastReceivedPosition = targetPosition;
        lastReceivedRotation = targetRotation;
        
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OdometryMsg>("/odom", ReceiveROSMsg);
        
        Debug.Log($"OdomSubscriber started - Y Position Locked: {lockYPosition} at height {fixedYHeight}");
    }
    
    private void ReceiveROSMsg(OdometryMsg odomMsg)
    {
        // Convert ROS coordinates to Unity
        Vector3 position = odomMsg.pose.pose.position.From<FLU>();
        Quaternion orientation = odomMsg.pose.pose.orientation.From<FLU>();
        
        // Store initial position on first message
        if (!initialPositionSet)
        {
            initialPosition = position;
            initialOrientation = orientation;
            initialPositionSet = true;
            Debug.Log($"Initial position set: {initialPosition}");
            return;
        }
        
        // Calculate relative movement
        Vector3 newPosition = position - initialPosition;
        Quaternion newRotation = orientation * Quaternion.Inverse(initialOrientation);
        
        // LOCK Y POSITION - prevent up/down shaking
        if (lockYPosition)
        {
            newPosition.y = fixedYHeight;
        }
        
        // LOCK X and Z ROTATION - prevent tilting
        Vector3 eulerRotation = newRotation.eulerAngles;
        if (lockXRotation) eulerRotation.x = 0;
        if (lockZRotation) eulerRotation.z = 0;
        newRotation = Quaternion.Euler(eulerRotation);
        
        // Only update if change is significant
        float positionDelta = Vector3.Distance(newPosition, lastReceivedPosition);
        float rotationDelta = Quaternion.Angle(newRotation, lastReceivedRotation);
        
        if (positionDelta > positionThreshold || rotationDelta > 0.1f)
        {
            targetPosition = newPosition;
            targetRotation = newRotation;
            lastReceivedPosition = newPosition;
            lastReceivedRotation = newRotation;
        }
        
        if (showDebugLogs && Time.frameCount % 300 == 0)
        {
            Debug.Log($"Target position: {targetPosition} (Y locked to {fixedYHeight})");
        }
    }
    
    void Update()
    {
        // Smoothly interpolate toward target position
        Vector3 smoothedPosition = Vector3.Lerp(robot.transform.position, targetPosition, smoothPosition);
        Quaternion smoothedRotation = Quaternion.Slerp(robot.transform.rotation, targetRotation, smoothRotation);
        
        // Apply smoothed movement
        if (robotArticulation != null)
        {
            robotArticulation.TeleportRoot(smoothedPosition, smoothedRotation);
        }
        else
        {
            robot.transform.SetPositionAndRotation(smoothedPosition, smoothedRotation);
        }
    }
}