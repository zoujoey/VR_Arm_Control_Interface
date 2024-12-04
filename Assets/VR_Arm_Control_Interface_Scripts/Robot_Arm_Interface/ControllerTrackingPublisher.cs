using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using PosRotMsg = RosMessageTypes.ROSArmControlInterface.PosRotMsg;

/// <summary>
///
/// </summary>
public class ControllerTrackingPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/ccube_position";

    // The game object
    public GameObject cube_controller;
    // Publish the cube's position and rotation every N seconds
    public float publishMessageFrequency = 0.5f;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;

    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PosRotMsg>(topicName);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            // cube.transform.rotation = Random.rotation;

            PosRotMsg controllerPos = new PosRotMsg(
                cube_controller.transform.position.x,
                cube_controller.transform.position.y,
                cube_controller.transform.position.z,
                cube_controller.transform.rotation.x,
                cube_controller.transform.rotation.y,
                cube_controller.transform.rotation.z,
                cube_controller.transform.rotation.w
            );

            // Finally send the message to server_endpoint.py running in ROS
            ros.Publish(topicName, controllerPos);

            timeElapsed = 0;
        }
    }
}