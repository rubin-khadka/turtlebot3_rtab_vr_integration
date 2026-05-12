using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class KeyboardTeleop : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxLinearSpeed = 0.5f;   // m/s
    public float maxAngularSpeed = 1.0f;  // rad/s
    
    [Header("ROS Settings")]
    public string cmdVelTopic = "/cmd_vel";
    
    [Header("Keyboard Mapping")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    
    private ROSConnection ros;
    private float linearSpeed = 0f;
    private float angularSpeed = 0f;
    
    void Start()
    {
        // Get or create ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        
        ros.RegisterPublisher<TwistMsg>(cmdVelTopic);
        
        Debug.Log($"KeyboardTeleop initialized. Publishing to {cmdVelTopic}");
        Debug.Log("Controls: W=Forward, S=Backward, A=Turn Left, D=Turn Right");
        Debug.Log($"Max Linear Speed: {maxLinearSpeed} m/s, Max Angular Speed: {maxAngularSpeed} rad/s");
    }
    
    void Update()
    {
        // Get keyboard input
        float vertical = 0f;
        float horizontal = 0f;
        
        if (Input.GetKey(forwardKey)) vertical = 1f;
        if (Input.GetKey(backwardKey)) vertical = -1f;
        if (Input.GetKey(leftKey)) horizontal = 1f;   // Positive = turn left
        if (Input.GetKey(rightKey)) horizontal = -1f;  // Negative = turn right
        
        // Apply smoothing for gradual acceleration
        linearSpeed = Mathf.Lerp(linearSpeed, vertical * maxLinearSpeed, Time.deltaTime * 5f);
        angularSpeed = Mathf.Lerp(angularSpeed, horizontal * maxAngularSpeed, Time.deltaTime * 5f);
        
        // Create Twist message
        TwistMsg twist = new TwistMsg();
        twist.linear.x = linearSpeed;
        twist.angular.z = angularSpeed;
        
        // Publish using the new API 
        ros.Publish(cmdVelTopic, twist);
        
        // Debug output (only when moving)
        if (Mathf.Abs(linearSpeed) > 0.01f || Mathf.Abs(angularSpeed) > 0.01f)
        {
            Debug.Log($"Publishing - Linear: {linearSpeed:F2}, Angular: {angularSpeed:F2}");
        }
    }
    
    void OnDisable()
    {
        if (ros != null)
        {
            TwistMsg stopMsg = new TwistMsg();
            stopMsg.linear.x = 0;
            stopMsg.angular.z = 0;
            ros.Publish(cmdVelTopic, stopMsg);
            Debug.Log("Published stop command");
        }
    }
}