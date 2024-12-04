from __future__ import annotations
from typing import Callable, List

import rospy

from .state import Machine, State
from .controller import Controller


# Basic States Categories:

# ARM CONTROL
# CONDITIONALS


# ARM CONTROL


class Wait(State):
    _start_time: rospy.Time

    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
        self.wait_time = 30
        
    def enter(self) -> None:
        print("Current State: " + str(self.name))
        self._start_time = rospy.Time.now()


    def run(self) -> None | State:
        elapsed_time = (rospy.Time.now() - self._start_time).to_sec()
        if elapsed_time >= self.wait_time:
            return self.to['next']
        return None

class GotoPosition(State):
    point: List[float]
    _start_time: rospy.Time

    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
        self.wait_time = 2
        self._has_arrived = False
        self.point = None
    
    def enter(self) -> None:
        print("Current State: " + str(self.name))
        self.machine.controller.automatic_mode()
        self.machine.controller.set_target_position_ee(None, self.point)
        self._start_time = rospy.Time.now()
        self._has_arrived = False

    def run(self) -> State | None:
        if self.machine.controller.has_arrived_target():
            if not self._has_arrived:
                self._start_time = rospy.Time.now()
                self._has_arrived = True
            elapsed_time = (rospy.Time.now() - self._start_time).to_sec()
            if elapsed_time >= self.wait_time:
                return self.to['next']
        return None
    
class GotoPositionRel(State):
    point: List[float]
    _start_time: rospy.Time

    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
        self.wait_time = 2
        self._has_arrived = False
        self.point = None
    
    def enter(self) -> None:
        print("Current State: " + str(self.name))
        self.machine.controller.automatic_mode()
        self.machine.controller.set_target_position_ee(self.point)
        self._start_time = rospy.Time.now()
        self._has_arrived = False

    def run(self) -> State | None:
        if self.machine.controller.has_arrived_target():
            if not self._has_arrived:
                self._start_time = rospy.Time.now()
                self._has_arrived = True
            elapsed_time = (rospy.Time.now() - self._start_time).to_sec()
            if elapsed_time >= self.wait_time:
                return self.to['next']
        
        return None

class UseGripper(State):
    width: float
    speed: float
    force: int
    _start_time: rospy.Time
    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
        self.wait_time = 3
        self._has_arrived = False
        self.width = None
        self.speed = None
        self.force = None
        self.debug_time = 10
    
    def enter(self) -> None:
        print("Current State: " + str(self.name) + str(self.width))
        self.machine.controller.set_target_position_rg(self.width, self.speed, self.force)
        self.machine.controller.gripper_mode()
        self._start_time = rospy.Time.now()
        self._has_arrived = False

    def run(self) -> State | None:
        if self.machine.controller.has_gripped_target():
            if not self._has_arrived:
                self._start_time = rospy.Time.now()
                self._has_arrived = True
            elapsed_time = (rospy.Time.now() - self._start_time).to_sec()
            if elapsed_time >= self.wait_time:
                return self.to['next']
        elapsed_time = (rospy.Time.now() - self._start_time).to_sec()
        if elapsed_time >= self.debug_time:
            print(self.machine.controller.get_controller_info())
        return None

class MoveGripper(UseGripper):
    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
    def enter(self) -> None:
        super().enter()
        self.machine.controller.publish_rg_move()
    
class GraspGripper(UseGripper):
    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
    def enter(self) -> None:
        super().enter()
        self.machine.controller.publish_rg_grasp()

class StopGripper(UseGripper):
    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
    def enter(self) -> None:
        super().enter()
        self.machine.controller.publish_rg_stop()
    def run(self) -> None:
        elapsed_time = (rospy.Time.now() - self._start_time).to_sec()
        if elapsed_time >= self.wait_time:
            return self.to['next']
        return None
    
# class CommandActionGripper(UseGripper):
#     def __init__(self, machine: Machine) -> None:
#         super().__init__(machine)
#     def enter(self) -> None:
#         super().enter()
#         self.machine.controller.publish_rg_command_action()

class TrackCubeControl(State):
    _start_time: rospy.Time

    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
        self.wait_time = 300
        self._has_arrived = False
        self.point = None
    
    def enter(self) -> None:
        print("Current State: " + str(self.name))
        self.machine.controller.manual_position_mode()
        self.machine.controller.set_target_position_cc()
        self._start_time = rospy.Time.now()
        self._has_arrived = False

    def run(self) -> State | None:
        self.machine.controller.set_target_position_cc()
        elapsed_time = (rospy.Time.now() - self._start_time).to_sec()
        if elapsed_time >= self.wait_time:
            return self.to['next']
        return None

class ResetDefault(State):
    _start_time: rospy.Time

    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
        self.wait_time = 5
        self._has_arrived = False
        self.point = None
    
    def enter(self) -> None:
        print("Current State: " + str(self.name))
        self.machine.controller.automatic_mode()
        self.machine.controller.set_default_ee_position()
        self._start_time = rospy.Time.now()
        self._has_arrived = False

    def run(self) -> State | None:
        if self.machine.controller.has_arrived_target():
            if not self._has_arrived:
                self._start_time = rospy.Time.now()
                self._has_arrived = True
            elapsed_time = (rospy.Time.now() - self._start_time).to_sec()
            if elapsed_time >= self.wait_time:
                return self.to['next']
        return None

class Start(ResetDefault):
    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
        

class Finish(State):
    """Lands drone

    Sends land command, disarms the drone and waits for MAVROS to signal grounded state

    """
    has_disarmed: bool

    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
        self.is_terminal = True

    def enter(self) -> None:
        print("Current State: " + str(self.name))

    def run(self) -> None:
        self.machine.finished = True


# CONDITIONALS


class Branch(State):
    """Runs an expression and sends next state from return value

    Attributes:
        expression (Callable[[], str | None]): an expression with no arguments that returns either a string, or None.
        If a string is returned Branch will use it as the key in the self.to dict to find a State to return.
        If None is received Branch will return None, and run again next cycle
    """
    expression: Callable[[], str | None]

    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)

    def run(self) -> State | None:
        result = self.expression()
        if isinstance(result, str):
            return self.to[result]
        return None


class Counter(Branch):
    """Counts to an int, then returns a different state.

    Set to['if_repeat'] as the next State if counter is less than count_to

    Set to['if_count'] as the next State once counter reaches count_to

    Attributes:
        count_to (int): value to count to
    """

    def __init__(self, machine: Machine) -> None:
        super().__init__(machine)
        self.expression = self._counter
        self.count_to = 0
        self._count = 0

    def _counter(self) -> str:
        if self._count >= self.count_to:
            self._count = 0  # you can't use enter() or exit() because Counter must preserve internal state between reenters
            return 'if_count'
        self._count += 1
        return 'if_repeat'

    def exit(self) -> None:
        self.logger(f"{self._count}/{self.count_to}")
