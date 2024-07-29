#!/bin/sh

if [ -n "$DESTDIR" ] ; then
    case $DESTDIR in
        /*) # ok
            ;;
        *)
            /bin/echo "DESTDIR argument must be absolute... "
            /bin/echo "otherwise python's distutils will bork things."
            exit 1
    esac
fi

echo_and_run() { echo "+ $@" ; "$@" ; }

echo_and_run cd "/volume/hand_tracking_ws/src/ROS-TCP-Endpoint"

# ensure that Python install destination exists
echo_and_run mkdir -p "$DESTDIR/volume/hand_tracking_ws/install/lib/python3/dist-packages"

# Note that PYTHONPATH is pulled from the environment to support installing
# into one location when some dependencies were installed in another
# location, #123.
echo_and_run /usr/bin/env \
    PYTHONPATH="/volume/hand_tracking_ws/install/lib/python3/dist-packages:/volume/hand_tracking_ws/build/lib/python3/dist-packages:$PYTHONPATH" \
    CATKIN_BINARY_DIR="/volume/hand_tracking_ws/build" \
    "/usr/bin/python3" \
    "/volume/hand_tracking_ws/src/ROS-TCP-Endpoint/setup.py" \
    egg_info --egg-base /volume/hand_tracking_ws/build/ROS-TCP-Endpoint \
    build --build-base "/volume/hand_tracking_ws/build/ROS-TCP-Endpoint" \
    install \
    --root="${DESTDIR-/}" \
    --install-layout=deb --prefix="/volume/hand_tracking_ws/install" --install-scripts="/volume/hand_tracking_ws/install/bin"
