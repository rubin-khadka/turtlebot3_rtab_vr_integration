from launch import LaunchDescription
from launch.actions import IncludeLaunchDescription
from launch.substitutions import PathJoinSubstitution
from launch_ros.substitutions import FindPackageShare
from launch.launch_description_sources import PythonLaunchDescriptionSource

def generate_launch_description():

    return LaunchDescription([

        IncludeLaunchDescription(
            PythonLaunchDescriptionSource([
                PathJoinSubstitution([
                    FindPackageShare('rtabmap_launch'),
                    'launch',
                    'rtabmap.launch.py'
                ])
            ]),
            launch_arguments={
                'use_sim_time': 'true',
                
                # Frames
                'frame_id': 'base_footprint',
                'odom_frame_id': 'odom', 
                'map_frame_id': 'map',
                'publish_tf': 'true',
                'publish_tf_map': 'true',

                # Topics
                'rgb_topic': '/camera/image',
                'depth_topic': '/camera/depth_image',
                'camera_info_topic': '/camera/camera_info',
                'scan_topic': '/scan',
                'odom_topic': '/odom',

                # Odometry - Use ONLY wheel odometry
                'subscribe_odom_info': 'true',
                'visual_odometry': 'false',
                'icp_odometry': 'false',
                'odom_uses_imu': 'false',

                # RGB-D settings
                'subscribe_rgbd': 'true',
                'subscribe_scan': 'true',
                'rgbd_sync': 'true',
                'approx_sync': 'true',
                'approx_rgbd_sync': 'true',
                'approx_sync_max_interval': '0.1',
                'queue_size': '20',
                'compressed': 'false',

                # Mesh generation
                'mesh_output_voxel': 'true',
                'mesh_output_voxel_size': '0.03',
                'mesh_output_textured': 'true',
                'mesh_output_compressed': 'true',
                'mesh_output_as_cloud': 'false',
                'mesh_visibility': 'true',
                'mesh_triangle_compression': 'true',

                'args': '-d \
                    --Grid/3D true \
                    --Grid/Sensor true \
                    --Grid/CellSize 0.03 \
                    --Grid/RangeMax 5.0 \
                    --cloud_output_voxel true \
                    --cloud_voxel_size 0.03 \
                    --cloud_filter_radius 0.15 \
                    --cloud_filter_min_neighbors 2 \
                    \
                    --Kp/MaxFeatures -1 \
                    --Vis/MaxFeatures 1500 \
                    \
                    --Mem/STMSize 30 \
                    --Mem/ImageCompression true \
                    --Mem/UseOdomFeatures true \
                    \
                    --RGBD/LinearUpdate 0.1 \
                    --RGBD/AngularUpdate 0.1 \
                    --RGBD/TemporalUpdate 1.0 \
                    \
                    --Reg/Strategy 0 \
                    --Reg/Force2D true \
                    \
                    --Rtabmap/DetectionRate 2 \
                    --Rtabmap/CreateIntermediateNodes true \
                    \
                    --RGBD/LoopThr 1.0 \
                    --RGBD/LoopMaxGraphDepth 0 \
                    --RGBD/LoopMinInliers 1000 \
                    --RGBD/ProximityByTime false \
                    --RGBD/ProximityMaxGraphDepth 0 \
                    --RGBD/ProximityPathMaxNeighbors 0 \
                    --RGBD/NeighborLinkRefining false \
                    --RGBD/StarLoopClosure false \
                    --RGBD/StarLoopMinInliers 1000 \
                    --Mem/LocalLoopDetection false \
                    --Mem/RehearsalSimilarity 0.0 \
                    --Mem/BadSignaturesIgnored false \
                    --Mem/EphemeralMemory false \
                    --Vis/EstimationType 0 \
                    --Vis/MinInliers 1000 \
                    --Vis/InlierDistance 100 \
                    --Vis/MaxFeatures 500 \
                    --Kp/DetectorStrategy 0',
                
                'rviz': 'true'
            }.items()
        ),
    ])