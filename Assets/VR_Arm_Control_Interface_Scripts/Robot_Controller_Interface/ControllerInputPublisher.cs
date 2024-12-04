using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using ButtonState = RosMessageTypes.ROSArmControlInterface.ButtonStateMsg;

public class ControllerInputPublisher : MonoBehaviour
{
    ROSConnection ros;
    public string topicName = "/button_state";

    // Controller input mappings
    public OVRInput.Axis1D Grab = OVRInput.Axis1D.SecondaryHandTrigger;
    public OVRInput.Axis1D Index = OVRInput.Axis1D.SecondaryIndexTrigger;
    public OVRInput.Axis2D Stick = OVRInput.Axis2D.SecondaryThumbstick;

    public OVRInput.Button StickButton = OVRInput.Button.SecondaryThumbstick; // Thumbstick as a button
    public OVRInput.Button X_Input = OVRInput.Button.One;
    public OVRInput.Button Y_Input = OVRInput.Button.Two;

    // Publish the cube's position and rotation every N seconds
    public float publishMessageFrequency = 0.1f;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;

    // Threshold to consider an Axis1D as "pushed down"
    private const float Axis1DThreshold = 0.5f;

    void Start()
    {
        // Start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<ButtonState>(topicName);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageFrequency)
        {
            // Determine states for Axis1D inputs
            bool grabPressed = OVRInput.Get(Grab) > Axis1DThreshold;
            bool indexPressed = OVRInput.Get(Index) > Axis1DThreshold;

            // Determine states for thumbstick as a button
            bool stickButtonPressed = OVRInput.GetDown(StickButton);

            // Determine thumbstick up/down movement
            Vector2 stickValue = OVRInput.Get(Stick);
            bool stickUp = stickValue.y < -Axis1DThreshold;
            bool stickDown = stickValue.y > Axis1DThreshold;

            // Create the ButtonState message
            ButtonState controllerstate = new ButtonState(
                grabPressed,
                indexPressed,
                stickButtonPressed,
                OVRInput.GetDown(X_Input),
                OVRInput.GetDown(Y_Input),
                stickUp,
                stickDown
            );

            // Publish the message to ROS
            ros.Publish(topicName, controllerstate);

            timeElapsed = 0;
        }
    }
}
