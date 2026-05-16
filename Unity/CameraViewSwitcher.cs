using UnityEngine;

public class CameraViewSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    public Transform leftWheel;      // Left wheel transform for wheel-level view
    public Transform rightWheel;     // Right wheel transform for wheel-level view
    public Transform scanLink;       // LiDAR scanner transform for sensor view
    
    [Header("VR Camera Rig")]
    public OVRCameraRig cameraRig;   // Main VR camera rig to reposition
    public Transform cameraLink;     // Default camera position (robot's main camera)
    
    private int currentView = 0;     // 0=Main, 1=Left Wheel, 2=Right Wheel, 3=Scan Link
    
    void Start()
    {
        // Auto-find references if not manually assigned
        if (cameraRig == null)
            cameraRig = FindObjectOfType<OVRCameraRig>();
        
        if (cameraLink == null)
            cameraLink = GameObject.Find("camera_link")?.transform;
        
        if (scanLink == null)
            scanLink = GameObject.Find("scan_link")?.transform;
        
        Debug.Log("CameraViewSwitcher Ready");
        Debug.Log("Press C or Left Click to cycle camera views");
    }
    
    void Update()
    {
        // Cycle views with mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left click - Cycling to next view");
            CycleToNextView();
        }
        
        // Cycle views with C key (keyboard fallback for VR testing)
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C key pressed - Cycling to next view");
            CycleToNextView();
        }
    }
    
    void CycleToNextView()
    {
        // Loop through 4 views: Main → Left Wheel → Right Wheel → Scan → Main
        currentView++;
        if (currentView > 3) currentView = 0;
        
        switch (currentView)
        {
            case 0:
                SetMainView();        // Default first-person robot view
                break;
            case 1:
                SetLeftWheelView();   // Close-up of left wheel
                break;
            case 2:
                SetRightWheelView();  // Close-up of right wheel
                break;
            case 3:
                SetScanLinkView();    // View from LiDAR scanner position
                break;
        }
    }
    
    void SetLeftWheelView()
    {
        if (leftWheel == null)
        {
            Debug.LogWarning("LeftWheel not assigned!");
            return;
        }
        
        // Attach camera to left wheel and offset slightly for better view
        cameraRig.transform.SetParent(leftWheel);
        cameraRig.transform.localPosition = new Vector3(-0.1f, 0.1f, -0.1f);  // Left, up, behind
        cameraRig.transform.localRotation = Quaternion.identity;
        Debug.Log("LEFT WHEEL VIEW");
    }
    
    void SetRightWheelView()
    {
        if (rightWheel == null)
        {
            Debug.LogWarning("RightWheel not assigned!");
            return;
        }
        
        // Attach camera to right wheel and offset slightly for better view
        cameraRig.transform.SetParent(rightWheel);
        cameraRig.transform.localPosition = new Vector3(0.1f, 0.1f, -0.1f);   // Right, up, behind
        cameraRig.transform.localRotation = Quaternion.identity;
        Debug.Log("RIGHT WHEEL VIEW");
    }
    
    void SetScanLinkView()
    {
        if (scanLink == null)
        {
            Debug.LogWarning("ScanLink not assigned!");
            return;
        }
        
        // Attach camera to LiDAR scanner mount point
        cameraRig.transform.SetParent(scanLink);
        cameraRig.transform.localPosition = new Vector3(0.1f, 0.1f, -0.1f);   // Slight offset for visibility
        cameraRig.transform.localRotation = Quaternion.identity;
        Debug.Log("SCAN LINK VIEW (Laser Scanner)");
    }
    
    void SetMainView()
    {
        if (cameraLink == null) return;
        
        // Return to default camera position (centered on robot)
        cameraRig.transform.SetParent(cameraLink);
        cameraRig.transform.localPosition = Vector3.zero;
        cameraRig.transform.localRotation = Quaternion.identity;
        Debug.Log("MAIN VIEW");
    }
    
    void OnGUI()
    {
        // Display current camera view on screen
        string modeText = currentView switch
        {
            0 => "MAIN VIEW",
            1 => "LEFT WHEEL VIEW",
            2 => "RIGHT WHEEL VIEW",
            3 => "FOLLOWING VIEW",
            _ => "UNKNOWN"
        };
        
        GUI.Label(new Rect(10, 50, 400, 30), $"Camera: {modeText}");
        GUI.Label(new Rect(10, 80, 400, 25), "Press C or Left Click = Cycle Camera Views");
    }
}