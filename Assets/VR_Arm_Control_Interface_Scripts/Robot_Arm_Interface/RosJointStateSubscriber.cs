using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosJoint = RosMessageTypes.ROSArmControlInterface.ControllerInfoMsg;

public class RosJointSubscriber : MonoBehaviour
{
    public GameObject J1;
    public GameObject J2;
    public GameObject J3;
    public GameObject J4;
    public GameObject J5;
    public GameObject J6;
    public GameObject J7;
    public GameObject G1;
    public GameObject G2;
    void Start()
    {
        // Subscribe to the ROS topic 'arm_joint_states' to receive joint angles
        ROSConnection.GetOrCreateInstance().Subscribe<RosJoint>("/arm/controller_info", JointUpdate);
    }

    void JointUpdate(RosJoint CI)
    {
        // Debug.Log($"Received joint angles (radians): Joint1={CI.joint_state.joint_1})");
        // Debug.Log($"Received joint angles (radians): Joint2={CI.joint_state.joint_2})");
        // Debug.Log($"Received joint angles (radians): Joint3={CI.joint_state.joint_3})");
        // Debug.Log($"Received joint angles (radians): Joint4={CI.joint_state.joint_4})");
        // Debug.Log($"Received joint angles (radians): Joint5={CI.joint_state.joint_5})");
        // Debug.Log($"Received joint angles (radians): Joint6={CI.joint_state.joint_6})");
        // Debug.Log($"Received joint angles (radians): Joint7={CI.joint_state.joint_7})");
        // Debug.Log($"Received joint angles (radians): Gripper1={CI.gripper_state.joint_1})");
        // Debug.Log($"Received joint angles (radians): Gripper2={CI.gripper_state.joint_2})");
        
        
        // Update only the z-rotation of each joint based on the received ROS message
        J1.transform.localEulerAngles = new Vector3(0, -CI.joint_state.joint_1*Mathf.Rad2Deg, 0);
        J2.transform.localEulerAngles = new Vector3(CI.joint_state.joint_2*Mathf.Rad2Deg, 0, 90);
        J3.transform.localEulerAngles = new Vector3(-CI.joint_state.joint_3*Mathf.Rad2Deg, 0, -90);
        J4.transform.localEulerAngles = new Vector3(-CI.joint_state.joint_4*Mathf.Rad2Deg, 0, -90);
        J5.transform.localEulerAngles = new Vector3(CI.joint_state.joint_5*Mathf.Rad2Deg, 0, 90);
        J6.transform.localEulerAngles = new Vector3(-CI.joint_state.joint_6*Mathf.Rad2Deg, 0, -90);
        J7.transform.localEulerAngles = new Vector3(-CI.joint_state.joint_7*Mathf.Rad2Deg, 0, -90);
        G1.transform.localPosition = new Vector3(CI.gripper_state.joint_1, (float)0.05839986, 0);
        G2.transform.localPosition = new Vector3(-CI.gripper_state.joint_2, (float)0.05839986, 0);
    }
}