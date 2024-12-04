// using UnityEngine;
// using Unity.Robotics.ROSTCPConnector;
// using RosJoint = RosMessageTypes.UnityRoboticsDemo.JointStateMsg;

// public class RosJointSubscriber : MonoBehaviour
// {
//     public GameObject J1;
//     public GameObject J2;
//     public GameObject J3;
//     public GameObject J4;
//     public GameObject J5;
//     public GameObject J6;
//     public GameObject J7;
//     void Start()
//     {
//         // Subscribe to the ROS topic 'arm_joint_states' to receive joint angles
//         ROSConnection.GetOrCreateInstance().Subscribe<RosJoint>("/unity/joint_states", JointUpdate);
//     }

//     void JointUpdate(RosJoint updatedState)
//     {
//         Debug.Log($"Received joint angles (radians): Joint1={updatedState.joint_1}, Joint2={updatedState.joint_2}, Joint3={updatedState.joint_3}, Joint4={updatedState.joint_4}, Joint5={updatedState.joint_5}, Joint6={updatedState.joint_6}, Joint7={updatedState.joint_7}");
//         // Update only the z-rotation of each joint based on the received ROS message
//         J1.transform.localEulerAngles = new Vector3(0, 0,   -updatedState.joint_1*Mathf.Rad2Deg);
//         J2.transform.localEulerAngles = new Vector3(90, 0,  -updatedState.joint_2*Mathf.Rad2Deg);
//         J3.transform.localEulerAngles = new Vector3(-90, 0, -updatedState.joint_3*Mathf.Rad2Deg);
//         J4.transform.localEulerAngles = new Vector3(-90, 0, -updatedState.joint_4*Mathf.Rad2Deg);
//         J5.transform.localEulerAngles = new Vector3(90, 0,  -updatedState.joint_5*Mathf.Rad2Deg);
//         J6.transform.localEulerAngles = new Vector3(-90, 0, -updatedState.joint_6*Mathf.Rad2Deg);
//         J7.transform.localEulerAngles = new Vector3(-90, 0, -updatedState.joint_7*Mathf.Rad2Deg);
//     }
// }