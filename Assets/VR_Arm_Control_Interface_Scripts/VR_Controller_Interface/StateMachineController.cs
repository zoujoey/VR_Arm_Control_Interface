using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosStackInfo = RosMessageTypes.ROSArmControlInterface.StackInfoMsg;

public class StateMachineController : MonoBehaviour
{
    public GameObject robotBase;
    public GameObject robotHand;
    public GameObject controllerCube;
    public GameObject boundingBox;
    public Transform hand_controller;

    public Vector3 robotBaseOffset;
    public Vector3 controllerCubeOffset;
    public Vector3 boundingBoxOffset;

    public enum State
    {
        Start,
        VRControl,
        ResetDefault,
        CubeControl,
        MoveGripper,
        GraspGripper,
        GripperControl,
        Finish
    }

    public State currentState = State.Finish;

    // ROS Variables
    private ROSConnection ros;
    private string stateTopicName = "/arm/stack_info"; // Example topic
    private float timeElapsed;

    private BoxCollider boundaryCollider;
    private BoxCollider cubeCollider;
    private Vector3 cubeSize;

    void Start()
    {
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<RosStackInfo>(stateTopicName, StateUpdate);

        // Ensure boundary and cube have BoxColliders
        boundaryCollider = boundingBox.GetComponent<BoxCollider>();
        if (boundaryCollider == null)
        {
            Debug.LogError("BoundingBox must have a BoxCollider.");
            return;
        }
        cubeCollider = controllerCube.GetComponent<BoxCollider>();
        if (cubeCollider == null)
        {
            Debug.LogError("Cube must have a BoxCollider.");
            return;
        }
        cubeSize = controllerCube.transform.localScale;
        SetCubeTransparency(0.5f);
        currentState = State.Finish;
    }

    void FixedUpdate()
    {
        if (currentState == State.CubeControl)
        {
            RestrictCubeWithinBounds();
        }
    }
    void Update()
    {
        // Execute actions based on the current state
        switch (currentState)
        {
            case State.Start:
                SnapToController();
                break;
            case State.ResetDefault:
                SnapToController();
                break;

            case State.CubeControl:
                RestrictCubeWithinBounds();
                break;

            case State.MoveGripper:
            case State.GraspGripper:
            case State.GripperControl:
            case State.Finish:
                // No actions needed for now
                break;
        }
        if (currentState != State.CubeControl)
        {
            FollowEndEffectorPose();
        }

        UpdateCubeColor();
    }

    void SnapToController()
    {
        // Align robot base, cube, and bounding box with the controller
        if (robotBase != null)
            robotBase.transform.position = hand_controller.position - robotHand.transform.position + robotBase.transform.position + robotBaseOffset;

        if (controllerCube != null)
            controllerCube.transform.position = robotHand.transform.position + controllerCubeOffset;

        if (boundingBox != null)
            boundingBox.transform.position = controllerCube.transform.position + boundingBoxOffset;
    }

    void RestrictCubeWithinBounds()
    {
        // Restrict the cube within the bounding box
        Vector3 newPosition = controllerCube.transform.position;
        Bounds bounds = boundaryCollider.bounds;

        newPosition.x = Mathf.Clamp(newPosition.x, bounds.min.x + cubeSize.x / 2, bounds.max.x - cubeSize.x / 2);
        newPosition.y = Mathf.Clamp(newPosition.y, bounds.min.y + cubeSize.y / 2, bounds.max.y - cubeSize.y / 2);
        newPosition.z = Mathf.Clamp(newPosition.z, bounds.min.z + cubeSize.z / 2, bounds.max.z - cubeSize.z / 2);

        controllerCube.transform.position = newPosition;
    }

    void UpdateCubeColor()
    {
        // Change cube color based on the state
        if (currentState == State.CubeControl)
        {
            controllerCube.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (currentState == State.Start || currentState == State.ResetDefault)
        {
            controllerCube.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            controllerCube.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    void SetCubeTransparency(float alpha)
    {
        Renderer renderer = controllerCube.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = renderer.material;
            Color color = material.color;
            color.a = alpha; // Set the alpha value (transparency)
            material.color = color;
            material.SetFloat("_Mode", 3); // Set material to Transparent mode
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0); // Disable writing to the depth buffer
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000; // Set render queue to transparent
        }
    }

    void FollowEndEffectorPose()
    {
        // Make sure the robot hand or end-effector is assigned
        if (robotHand != null)
        {
            controllerCube.transform.position = robotHand.transform.position;  // Only follow the position of the end-effector
        }
    }

    // ROS Subscriber callback for receiving StackInfoMsg
    void StateUpdate(RosStackInfo stateMsg)
    {
        // Transition to the state received from ROS based on `current_state`
        switch (stateMsg.current_state)
        {
            case "Start":
                currentState = State.Start;
                break;
            case "VRControl":
                currentState = State.VRControl;
                break;
            case "ResetToDefaultVR":
                currentState = State.ResetDefault;
                break;
            case "CubeControl":
                currentState = State.CubeControl;
                break;
            case "GripperControlMove":
                currentState = State.MoveGripper;
                break;
            case "GripperControlGrasp":
                currentState = State.GraspGripper;
                break;
            case "GripperControl":
                currentState = State.GripperControl;
                break;
            case "Finish":
                currentState = State.Finish;
                break;
            default:
                Debug.LogWarning($"Unknown state received: {stateMsg.current_state}");
                break;
        }
    }
}
