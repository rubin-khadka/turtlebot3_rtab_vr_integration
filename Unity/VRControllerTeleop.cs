using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class VRControllerTeleop : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxLinearSpeed = 0.2f;   // m/s
    public float maxAngularSpeed = 0.2f;  // rad/s
    
    [Header("ROS Settings")]
    public string cmdVelTopic = "/cmd_vel";  // ROS topic for velocity commands
    
    [Header("Input Settings")]
    public bool useKeyboardFallback = true;  // Allow keyboard control when no VR input
    
    private ROSConnection ros;
    
    void Start()
    {
        // Initialize ROS connection and register publisher
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(cmdVelTopic);
        Debug.Log("VR Controller Teleop Started");
        Debug.Log("Controls: W=Forward, S=Backward, A=Turn Left, D=Turn Right");
    }
    
    void Update()
    {
        // Update VR input state
        OVRInput.Update();
        
        // Read thumbstick input (x=horizontal/turn, y=vertical/forward)
        Vector2 moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        
        // Fallback to keyboard when thumbstick is not being used
        if (useKeyboardFallback && moveInput.magnitude < 0.01f)
        {
            float vertical = 0f;
            float horizontal = 0f;
            
            // Forward/Backward
            if (Input.GetKey(KeyCode.W)) vertical = 1f;
            if (Input.GetKey(KeyCode.S)) vertical = -1f;
            
            // Turn Left/Right
            if (Input.GetKey(KeyCode.A)) horizontal = 1f;   // Positive = turn left
            if (Input.GetKey(KeyCode.D)) horizontal = -1f;  // Negative = turn right
            
            moveInput = new Vector2(horizontal, vertical);
            
            // Log when keyboard input is active
            if (moveInput.magnitude > 0.01f)
            {
                Debug.Log($"Keyboard Input - Linear: {vertical}, Angular: {horizontal}");
            }
        }
        
        // Create and publish velocity command
        TwistMsg twist = new TwistMsg();
        twist.linear.x = moveInput.y * maxLinearSpeed;   // Forward/backward
        twist.angular.z = moveInput.x * maxAngularSpeed;  // Left/right rotation
        
        ros.Publish(cmdVelTopic, twist);
    }
    
    void FixedUpdate()
    {
        // Update VR input at fixed timestep for physics accuracy
        OVRInput.FixedUpdate();
    }
}