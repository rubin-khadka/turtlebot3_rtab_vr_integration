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
                
                'frame_id': 'base_footprint',
                'odom_frame_id': 'odom', 
                'map_frame_id': 'map',
                'publish_tf': 'true',
                'publish_tf_map': 'true',

                'rgb_topic': '/camera/image',
                'depth_topic': '/camera/depth_image',
                'camera_info_topic': '/camera/camera_info',
                'scan_topic': '/scan',
                'odom_topic': '/odom',

                'subscribe_odom_info': 'false',
                'visual_odometry': 'false', 
                'icp_odometry': 'false',

                'subscribe_rgbd': 'true',
                'subscribe_scan': 'true',
                'rgbd_sync': 'true',
                'approx_sync': 'true',
                'approx_rgbd_sync': 'true',
                'approx_sync_max_interval': '0.05',
                'compressed': '30', 

                # Mesh generation (what you need for Unity)
                'mesh_output_voxel': 'true',
                'mesh_output_voxel_size': '0.02',   
                'mesh_output_textured': 'true',        
                'mesh_output_compressed': 'true',
                'mesh_output_as_cloud': 'false',
                'mesh_visibility': 'true',            
                'mesh_triangle_compression': 'true',  
                
                # DENSE POINT CLOUD PARAMETERS - REMOVED PROBLEMATIC PARAMETER
                'args': '-d \
                    --Grid/3D true \
                    --Grid/Sensor true \
                    --Grid/CellSize 0.01 \
                    --Grid/RangeMax 5.0 \
                    --cloud_output_voxel true \
                    --cloud_voxel_size 0.01 \
                    --cloud_filter_radius 0.1 \
                    --cloud_filter_min_neighbors 3 \
                    --Kp/MaxFeatures 4000 \
                    --Vis/MaxFeatures 4000 \
                    --Vis/MinInliers 15 \
                    --Mem/STMSize 50 \
                    --RGBD/LinearUpdate 0.03 \
                    --RGBD/AngularUpdate 0.05 \
                    --Reg/Force3DoF true \
                    --Mem/ImageCompression true \
                    --Mem/CompressionParallelized true \
                    --Mem/UseOdomFeatures false \
                    --GFTT/QualityLevel 0.01 \
                    --RGBD/ProximityMaxGraphDepth 2 \
                    --RGBD/NeighborLinkRefining true \
                    --RGBD/LoopThr 0.11 \
                    --RGBD/ProximityPathMaxNeighbors 2',
                
                'rviz': 'true'
            }.items()
        ),
    ])