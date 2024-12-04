// using UnityEngine;
// using UnityEngine.XR;
// using Unity.Robotics.ROSTCPConnector;
// using RosMessageTypes.UnityRoboticsDemo;
// using RosJoint = RosMessageTypes.UnityRoboticsDemo.JointStateMsg;


// public class SnapToHeadsetController : MonoBehaviour
// {
//     public GameObject pandaBase;
//     public GameObject pandaHand;
//     public GameObject controllerCube;
//     public GameObject boundingBox;
//     public Transform hand_controller;

//     // The button you want to use to trigger the snapping
//     public OVRInput.Button snapButton = OVRInput.Button.One;

//     public OVRInput.Button initButton = OVRInput.Button.Two;

//     // Set these to the desired relative positions

//     public Vector3 pandaBaseOffset; // Position it at foot level, slightly in front
//     public Vector3 controllerCubeOffset; // Slightly to the right and forward
//     public Vector3 boundingBoxOffset; // Slightly to the left and up


//     //ROS Parameters
//     ROSConnection ros;
//     public string topicName = "pos_rot";

//     // Publish the cube's position and rotation every N seconds
//     public float publishMessageFrequency = 0.5f;

//     // Used to determine how much time has elapsed since the last message was published
//     private float timeElapsed;
//     private bool pos_initialized;
//     private bool initialized;
//     void Start()
//     {
//         // start the ROS connection
//         ros = ROSConnection.GetOrCreateInstance();
//         ros.RegisterPublisher<PosRotMsg>(topicName);
//         initialized = false;
//         pos_initialized = false;
//         ROSConnection.GetOrCreateInstance().Subscribe<RosJoint>("/unity/joint_states", JointUpdate);
//     }

//     void Update()
//     {
//         // Check if the snap button is pressed
//         if (OVRInput.GetDown(snapButton))
//         {
//             SnapToController();
//         }

//         if (OVRInput.GetDown(initButton)){
//             if(pos_initialized){
//                 initialized = true;
//             }
//         }
        
//         SetCubeColour();
        
//         timeElapsed += Time.deltaTime;
//         if (timeElapsed > publishMessageFrequency & initialized)
//         {
//             // cube.transform.rotation = Random.rotation;
//             PosRotMsg cubePos = new PosRotMsg(
//                 controllerCube.transform.position.x,
//                 controllerCube.transform.position.y,
//                 controllerCube.transform.position.z,
//                 controllerCube.transform.rotation.x,
//                 controllerCube.transform.rotation.y,
//                 controllerCube.transform.rotation.z,
//                 controllerCube.transform.rotation.w
//             );

//             // Finally send the message to server_endpoint.py running in ROS
//             ros.Publish(topicName, cubePos);

//             timeElapsed = 0;
//         }
//     }

//     void SetCubeColour()
//     {
//         if(pos_initialized & initialized){
//             controllerCube.GetComponent<Renderer>().material.color = Color.red;
//         }
//         else if (pos_initialized){
//             controllerCube.GetComponent<Renderer>().material.color = Color.green;
//         }
//         else{
//             controllerCube.GetComponent<Renderer>().material.color = Color.blue;
//         }
//     }
//     void SnapToController()
//     {
//         // Snap the positions of the objects to the headset controller's position plus the offset
//         if (pandaBase != null)
//             pandaBase.transform.position = hand_controller.position - pandaHand.transform.position + pandaBase.transform.position + pandaBaseOffset;

//         if (controllerCube != null)
//             controllerCube.transform.position = pandaHand.transform.position + controllerCubeOffset;

//         if (boundingBox != null)
//             boundingBox.transform.position = controllerCube.transform.position + boundingBoxOffset;
//     }

//     void JointUpdate(RosJoint updatedState)
//     {
//         pos_initialized = true;
//     }
// }