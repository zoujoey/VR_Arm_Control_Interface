#!/usr/bin/env python3

from flight_stack import FlightStack
from flight_stack.state import Machine
from flight_stack.controller import Controller
from flight_stack.basic_states import Wait, Land

from basic_hover import Hover
from basic_velocity import Velocity


class Embed(Machine):
    """Basic embedded machine showcase
    """

    def __init__(self, controller: Controller) -> None:
        super().__init__(controller)

        self.hover = self.embed(Hover(controller))
        self.velocity = self.embed(Velocity(controller))

        self.hover.to['next'] = self.velocity

        self.start_state = self.hover


if __name__ == "__main__":
    fs = FlightStack()
    dc = Controller()
    embed = Embed(dc)

    fs.controller = dc
    fs.machine = embed

    fs.start()
