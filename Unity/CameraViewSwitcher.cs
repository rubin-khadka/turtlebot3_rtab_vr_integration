using UnityEngine;

public class CameraViewSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    public Transform leftWheel;
    public Transform rightWheel;
    public Transform scanLink;  
    
    [Header("VR Camera Rig")]
    public OVRCameraRig cameraRig;
    public Transform cameraLink;
    
    private int currentView = 0;  // 0=Main, 1=Left, 2=Right, 3=Scan
    
    void Start()
    {
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
        // Left mouse click cycles through views
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left click - Cycling to next view");
            CycleToNextView();
        }
        
        // Keyboard key C cycles through views
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C key pressed - Cycling to next view");
            CycleToNextView();
        }
    }
    
    void CycleToNextView()
    {
        currentView++;
        if (currentView > 3) currentView = 0;  
        
        switch (currentView)
        {
            case 0:
                SetMainView();
                break;
            case 1:
                SetLeftWheelView();
                break;
            case 2:
                SetRightWheelView();
                break;
            case 3:
                SetScanLinkView();
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
        
        cameraRig.transform.SetParent(leftWheel);
        cameraRig.transform.localPosition = new Vector3(-0.1f, 0.1f, -0.1f);
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
        
        cameraRig.transform.SetParent(rightWheel);
        cameraRig.transform.localPosition = new Vector3(0.1f, 0.1f, -0.1f);
        cameraRig.transform.localRotation = Quaternion.identity;
        Debug.Log("RIGHT WHEEL VIEW");
    }
    
    void SetScanLinkView()  // ADDED: New method for scan_link
    {
        if (scanLink == null)
        {
            Debug.LogWarning("ScanLink not assigned!");
            return;
        }
        
        cameraRig.transform.SetParent(scanLink);
        cameraRig.transform.localPosition = new Vector3(0.1f, 0.1f, -0.1f);
        cameraRig.transform.localRotation = Quaternion.identity;
        Debug.Log("SCAN LINK VIEW (Laser Scanner)");
    }
    
    void SetMainView()
    {
        if (cameraLink == null) return;
        
        cameraRig.transform.SetParent(cameraLink);
        cameraRig.transform.localPosition = Vector3.zero;
        cameraRig.transform.localRotation = Quaternion.identity;
        Debug.Log("MAIN VIEW");
    }
    
    void OnGUI()
    {
        string modeText = currentView switch
        {
            0 => "MAIN VIEW",
            1 => "LEFT WHEEL VIEW",
            2 => "RIGHT WHEEL VIEW",
            3 => "SCAN LINK VIEW",
            _ => "UNKNOWN"
        };
        
        GUI.Label(new Rect(10, 50, 400, 30), $"Camera: {modeText}");
        GUI.Label(new Rect(10, 80, 400, 25), "Press C or Left Click = Cycle Camera Views");
    }
}