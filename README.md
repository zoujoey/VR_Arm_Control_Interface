
# VR Arm Control Interface

This Unity project integrates a VR controller with a robotic arm for intuitive control and autonomous functionality. The system enables precise manipulation of a FR3 robot arm using a VR setup, supporting various modes for positioning, grasping, and autonomous missions.

## Features

- **Real-time VR Controller Integration**: 
  - Track and control the robot arm's end-effector using a VR controller.
  
- **Dynamic Gripper Control**:
  - Adjust gripper width for precise grasping and manipulation.
  
- **Autonomous Mission Execution**:
  - Launch pre-defined autonomous missions with the ability to return to manual control.

- **Flexible State Management**:
  - Manage control modes via intuitive VR inputs.

## Control States

### State Overview:
1. **Idle (State 0)**:
   - Default state. All buttons are unpressed, waiting for user input.

2. **Position Control (State 1)**:
   - Triggered by the "Grab" button.
   - Allows the robot arm to follow the VR controller's position.

3. **Gripper Control (State 2)**:
   - Triggered by the "Index" button.
   - Controls the gripper with joystick input for width adjustment.
     - **Grasping Action**:
       - Triggered by pressing "Index" again.
       - The gripper attempts to grasp an object.
     - **Width Adjustment**:
       - Adjust width using the joystick to set target width for both grasp and move actions.

4. **Reset (State 3)**:
   - Triggered by pressing "X" input.
   - Resets the system to default settings.
     - **Safety Feature**: Pressing "X" three times consecutively shuts down the controller.

5. **Autonomous Mission (State 4)**:
   - Triggered by pressing "Y" input.
   - Starts an autonomous mission with a return-to-controller option.
     - Pressing "Y" again during the mission terminates it prematurely.

## Getting Started

### Unity Setup
1. Clone this repository into your Unity project directory.
2. Ensure the Unity Asset Serialization mode is set to **Force Text** for version control compatibility.
   - Navigate to `Edit > Project Settings > Editor > Asset Serialization` and select **Force Text**.
3. Install the required dependencies for VR controller input and visualization.

### ROS Integration
This Unity project is designed to work alongside the **ROS Arm Control Interface**. Clone the ROS package and follow its setup instructions:

[ROS Arm Control Interface Repository](https://github.com/zoujoey/ROS_Arm_Control_Interface.git)

Key ROS commands for Unity integration:
```bash
# Launch Unity Controller Interface
roslaunch ROS_Arm_Control_Interface unity_controller_interface.launch
rosrun ROS_Arm_Control_Interface unity_control.py
```

### Running the System
1. **Start ROS Nodes**:
   - Run the Unity Controller Interface using the command above.
2. **Launch Unity**:
   - Start the Unity application and ensure VR hardware is properly connected.
3. **Control the Robot**:
   - Use the VR controller to switch between states and perform tasks.


---

## License

TBD
---
