# VR Controlled Robot Teleoperation

> VR-based robot teleoperation using Unity (Windows) + ROS2 Jazzy / Gazebo (Ubuntu 24.04) with RTAB-Map static 3D environment

## Overview

This project demonstrates **virtual reality teleoperation** of a mobile robot. The VR headset controller sends commands through Unity to ROS2, which drives the robot in Gazebo. Odometry data is streamed back to Unity for synchronized contorl and visualization.

**Tech Stack:**
| Component | Platform | Role |
|-----------|----------|------|
| Unity | Windows | VR rendering, visualization, camera switching |
| ROS2 Jazzy + Gazebo | Ubuntu 24.04 | Physics simulation, robot control, odometry |
| ROS-TCP-Connector | Cross-platform | Bi-directional communication between Unity and ROS2 |
| RTAB-Map | Ubuntu | Static 3D map generation |
| VR Headset | Meta Quest | Head-mounted display for VR visualization |
| VR Controllers | Meta Quest | Input devices for teleoperation commands |

## ROS2 Jazzy (Ubuntu 24.04)

### Map Building Demo
https://github.com/user-attachments/assets/681ad4d4-4473-4bb8-8107-3008278fa6f9

### Static 3D map Point Cloud

<img width="1407" height="765" alt="static_pc" src="https://github.com/user-attachments/assets/12a71609-5810-451a-922b-f99ece1ac7fb" />

### Static 3D map Mesh

<img width="1164" height="725" alt="static_map" src="https://github.com/user-attachments/assets/86e5ac00-91c6-4f59-958b-9c8ee684271f" />

### Static 2D map

<img width="529" height="321" alt="rtabmap" src="https://github.com/user-attachments/assets/3f7545b8-5861-4e53-b730-1288c16c33bf" />

### Robot in Gazebo

Robot spawned in Gazebo with the static 3D mesh map loaded:

<img width="1501" height="908" alt="robot_gazebo" src="https://github.com/user-attachments/assets/92aabbac-f5b9-4f41-9ea7-5f2cd363b15f" />

## Unity (Windows)

### VR Teleoperation Demo — Headset Perspective

*What the operator sees through the Meta Quest headset during teleoperation:*

- **Robot POV:** First-person view from the robot
- **Minimap:** Spatial awareness of environment
- **Real-time distance feedback:** Proximity to walls and objects
- **Switchable views:** Left wheel view, right wheel view, and following camera to monitor wall clearance and prevent collisions

https://github.com/user-attachments/assets/e6e9bcdb-ee10-4410-992b-40227894b76e

### ROS2 Connection States

The Unity interface shows the current status of the ROS-TCP-Connector bridge:

| State | Description |
|-------|-------------|
| **Connecting** | Initializing ROS2 communication, waiting for connection |
| **Active** | Fully connected, odometry data flowing without delay |
| **Stale** | Connected but no odometry data received (e.g., Gazebo paused or robot not moving) |
| **Disconnected** | No connection to ROS2 server |

#### Connecting
*Initializing ROS2 and Unity communication, waiting for connection*

<img width="867" height="902" alt="Unity_connecting" src="https://github.com/user-attachments/assets/550136cb-b740-4625-8ff8-6a64bd26e151" />

#### Active
*ROS2 and Unity communicating — connection active, receiving messages without delay*

<img width="860" height="897" alt="Unity_connected_active" src="https://github.com/user-attachments/assets/6e705623-9458-485f-b435-0c0f2eabf4a2" />

#### Stale
*Connection active but no odometry data coming through (e.g., Gazebo paused or not started, or odometry topic stopped)*

<img width="858" height="897" alt="Unity_connected_stale" src="https://github.com/user-attachments/assets/35fd777e-296f-480d-90d7-07192037b686" />

#### Disconnected
*No active connection to ROS2 server*

<img width="857" height="902" alt="Unity_disconnected" src="https://github.com/user-attachments/assets/bf1a6e70-66cd-4770-9cc7-1a4b747d300f" />

