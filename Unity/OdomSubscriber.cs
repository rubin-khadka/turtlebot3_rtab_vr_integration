using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Nav;

public class OdomSubscriber : MonoBehaviour
{
    [Header("Robot Reference")]
    public GameObject robot;                              // The robot GameObject to move
    
    [Header("Smoothing Settings")]
    [SerializeField] private float smoothPosition = 0.3f;  // Position interpolation speed (0-1)
    [SerializeField] private float smoothRotation = 0.3f;  // Rotation interpolation speed (0-1)
    [SerializeField] private float positionThreshold = 0.001f;  // Minimum movement to update target
    
    [Header("Y-Axis Locking (Prevents shaking)")]
    [SerializeField] private bool lockYPosition = true;    // Lock vertical position to fixed height
    [SerializeField] private float fixedYHeight = 0.1f;   // Fixed height above ground plane
    [SerializeField] private bool lockXRotation = true;    // Lock roll (prevent sideways tilt)
    [SerializeField] private bool lockZRotation = true;    // Lock pitch (prevent forward/backward tilt)
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;    // Enable periodic debug output
    
    private ROSConnection ros;
    private Vector3 initialPosition;                       // First received ROS position (calibration offset)
    private Quaternion initialOrientation;                 // First received ROS orientation (calibration offset)
    private bool initialPositionSet = false;               // Flag for first message received
    private ArticulationBody robotArticulation;            // Physics-based robot body (if exists)
    
    // Smoothing targets
    private Vector3 targetPosition;                        // Desired position to smoothly move toward
    private Quaternion targetRotation;                     // Desired rotation to smoothly rotate toward
    private Vector3 lastReceivedPosition;                  // Previous target (for change detection)
    private Quaternion lastReceivedRotation;               // Previous target (for change detection)
    
    void Start()
    {
        // Use this GameObject if no robot reference is assigned
        if (robot == null) robot = gameObject;
        robotArticulation = robot.GetComponent<ArticulationBody>();
        
        if (robotArticulation == null)
        {
            Debug.LogWarning("No ArticulationBody found, using Transform directly");
        }
        
        // Initialize targets to current position (no sudden jumps on start)
        targetPosition = robot.transform.position;
        targetRotation = robot.transform.rotation;
        lastReceivedPosition = targetPosition;
        lastReceivedRotation = targetRotation;
        
        // Subscribe to robot odometry topic
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<OdometryMsg>("/odom", ReceiveROSMsg);
        
        Debug.Log($"OdomSubscriber started - Y Position Locked: {lockYPosition} at height {fixedYHeight}");
    }
    
    private void ReceiveROSMsg(OdometryMsg odomMsg)
    {
        // Convert from ROS coordinate system (FLU) to Unity (FLU->FRD via conversion)
        Vector3 position = odomMsg.pose.pose.position.From<FLU>();
        Quaternion orientation = odomMsg.pose.pose.orientation.From<FLU>();
        
        // First message sets the initial offset (calibration)
        if (!initialPositionSet)
        {
            initialPosition = position;
            initialOrientation = orientation;
            initialPositionSet = true;
            Debug.Log($"Initial position set: {initialPosition}");
            return;  // Don't move on first message
        }
        
        // Calculate position/rotation relative to initial pose
        Vector3 newPosition = position - initialPosition;
        Quaternion newRotation = orientation * Quaternion.Inverse(initialOrientation);
        
        // LOCK Y POSITION - prevent vertical bouncing from odometry noise
        if (lockYPosition)
        {
            newPosition.y = fixedYHeight;
        }
        
        // LOCK X and Z ROTATION - prevent robot from tilting (only allow Y-axis rotation)
        Vector3 eulerRotation = newRotation.eulerAngles;
        if (lockXRotation) eulerRotation.x = 0;  // Lock roll
        if (lockZRotation) eulerRotation.z = 0;  // Lock pitch
        newRotation = Quaternion.Euler(eulerRotation);
        
        // Only update target if change is significant (reduces jitter from tiny movements)
        float positionDelta = Vector3.Distance(newPosition, lastReceivedPosition);
        float rotationDelta = Quaternion.Angle(newRotation, lastReceivedRotation);
        
        if (positionDelta > positionThreshold || rotationDelta > 0.1f)
        {
            targetPosition = newPosition;
            targetRotation = newRotation;
            lastReceivedPosition = newPosition;
            lastReceivedRotation = newRotation;
        }
        
        // Periodic debug output (every 300 frames)
        if (showDebugLogs && Time.frameCount % 300 == 0)
        {
            Debug.Log($"Target position: {targetPosition} (Y locked to {fixedYHeight})");
        }
    }
    
    void Update()
    {
        // Smoothly interpolate current position toward target (removes sudden jumps)
        Vector3 smoothedPosition = Vector3.Lerp(robot.transform.position, targetPosition, smoothPosition);
        Quaternion smoothedRotation = Quaternion.Slerp(robot.transform.rotation, targetRotation, smoothRotation);
        
        // Apply movement using ArticulationBody (physics) or Transform (kinematic)
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