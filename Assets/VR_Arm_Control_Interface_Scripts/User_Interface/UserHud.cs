using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using TMPro;
using RosControllerInfo = RosMessageTypes.ROSArmControlInterface.ControllerInfoMsg;
using RosStackInfo = RosMessageTypes.ROSArmControlInterface.StackInfoMsg;
using RosStateInfo = RosMessageTypes.ROSArmControlInterface.StateInfoMsg;

public class UserHUD : MonoBehaviour
{
    ROSConnection ros;

    // Text elements for the HUD
    public TextMeshProUGUI text1HUD; // For ControllerInfoMsg data
    public TextMeshProUGUI text2HUD; // For StackInfoMsg and StateInfoMsg data

    // ROS topics
    public string controllerInfoTopic = "/arm/controller_info";
    public string stackInfoTopic = "/arm/stack_info";
    public string stateInfoTopic = "/arm/state_info";

    private string currentStackState = "";
    private string previousStackState = "";
    private string stateMessage = "";

    void Start()
    {
        // Initialize ROS connection
        ros = ROSConnection.GetOrCreateInstance();

        // Subscribe to ROS topics
        ros.Subscribe<RosControllerInfo>(controllerInfoTopic, UpdateControllerInfo);
        ros.Subscribe<RosStackInfo>(stackInfoTopic, UpdateStackInfo);
        ros.Subscribe<RosStateInfo>(stateInfoTopic, UpdateStateInfo);
    }

    // Update ControllerInfoMsg HUD
    void UpdateControllerInfo(RosControllerInfo msg)
    {
        string eePoseCur = FormatPosRot(msg.ee_pose_cur);
        string eePoseTarget = FormatPosRot(msg.ee_pose_target);
        string eePoseDefault = FormatPosRot(msg.ee_pose_default);
        string jointState = FormatJointState(msg.joint_state);
        string gripperState = FormatGripperState(msg.gripper_state);
        
        text1HUD.text = $@"
        Control Mode: {msg.control_mode}
        EE Pose (Current): {eePoseCur}
        EE Pose (Target): {eePoseTarget}
        EE Pose (Default): {eePoseDefault}
        Joint State: {jointState}
        Gripper State: {gripperState}
        Target Gripper State: {msg.rg_pose_target:F3}
        ";
        Debug.Log($"Received Message Text 1");
    }

    // Update StackInfoMsg HUD
    void UpdateStackInfo(RosStackInfo msg)
    {
        currentStackState = msg.current_state;
        previousStackState = msg.previous_state;

        UpdateText2HUD();
    }

    // Update StateInfoMsg HUD
    void UpdateStateInfo(RosStateInfo msg)
    {
        stateMessage = msg.message;

        UpdateText2HUD();
    }

    // Combine StackInfoMsg and StateInfoMsg for text2HUD
    void UpdateText2HUD()
    {
        text2HUD.text = $@"
        Current State: {currentStackState}
        Previous State: {previousStackState}
        State Message: {stateMessage}
        ";
        Debug.Log($"Received Message Text 2");
    }

    // Helper to format PosRot message
    string FormatPosRot(RosMessageTypes.ROSArmControlInterface.PosRotMsg posRot)
    {
        return $"Pos: ({posRot.pos_x:F3}, {posRot.pos_y:F3}, {posRot.pos_z:F3}), " +
               $"Rot: ({posRot.rot_x:F3}, {posRot.rot_y:F3}, {posRot.rot_z:F3}, {posRot.rot_w:F3})";
    }

    // Helper to format JointState message
    string FormatJointState(RosMessageTypes.ROSArmControlInterface.JointStateMsg jointState)
    {
        return $@"
            J1: {jointState.joint_1:F3}, J2: {jointState.joint_2:F3}, J3: {jointState.joint_3:F3}, 
            J4: {jointState.joint_4:F3}, J5: {jointState.joint_5:F3}, J6: {jointState.joint_6:F3}, 
            J7: {jointState.joint_7:F3}";
    }

    // Helper to format GripperState message
    string FormatGripperState(RosMessageTypes.ROSArmControlInterface.JointStateMsg gripperState)
    {
        return $"Gripper Left: {gripperState.joint_1:F3}, Gripper Right: {gripperState.joint_2:F3}";
    }
}
