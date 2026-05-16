using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class VRControllerTeleop : MonoBehaviour
{
    public float maxLinearSpeed = 0.2f;
    public float maxAngularSpeed = 0.2f;
    public string cmdVelTopic = "/cmd_vel";
    public bool useKeyboardFallback = true;
    
    private ROSConnection ros;
    
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(cmdVelTopic);
        Debug.Log("VR Controller Teleop Started");
        Debug.Log("Controls: W=Forward, S=Backward, A=Turn Left, S=Turn Right");
    }
    
    void Update()
    {
        OVRInput.Update();
        
        Vector2 moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        
        if (useKeyboardFallback && moveInput.magnitude < 0.01f)
        {
            float vertical = 0f;
            float horizontal = 0f;
            
            // Forward/Backward
            if (Input.GetKey(KeyCode.W)) vertical = 1f;
            if (Input.GetKey(KeyCode.s)) vertical = -1f;
            
            // Turn Left/Right
            if (Input.GetKey(KeyCode.A)) horizontal = 1f; 
            if (Input.GetKey(KeyCode.D)) horizontal = -1f; 
            
            moveInput = new Vector2(horizontal, vertical);
            
            if (moveInput.magnitude > 0.01f)
            {
                Debug.Log($"Movement: vertical={vertical}, horizontal={horizontal}");
            }
        }
        
        TwistMsg twist = new TwistMsg();
        twist.linear.x = moveInput.y * maxLinearSpeed;
        twist.angular.z = moveInput.x * maxAngularSpeed;
        
        ros.Publish(cmdVelTopic, twist);
    }
    
    void FixedUpdate()
    {
        OVRInput.FixedUpdate();
    }
}