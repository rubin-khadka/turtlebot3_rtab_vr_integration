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
                'points_topic': '/camera/points',
                'camera_info_topic': '/camera/camera_info',
                'scan_topic': '/scan',
                'odom_topic': '/odom',

                'subscribe_odom_info': 'true',
                'visual_odometry': 'true', 
                'icp_odometry': 'true',

                'subscribe_rgbd': 'true',
                'subscribe_scan': 'true',
                'rgbd_sync': 'true',
                'approx_sync': 'true',
                'approx_rgbd_sync': 'true',
                'approx_sync_max_interval': '0.05',
                'queue_size': '10',
                'compressed': 'false',

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
                    --Grid/CellSize 0.02 \
                    --Grid/RangeMax 5.0 \
                    --cloud_output_voxel true \
                    --cloud_voxel_size 0.02 \
                    --cloud_filter_radius 0.1 \
                    --cloud_filter_min_neighbors 3 \
                    --Kp/MaxFeatures -1 \
                    --Vis/MaxFeatures 2000 \
                    --Vis/MinInliers 8 \
                    --Mem/STMSize 50 \
                    --RGBD/LinearUpdate 0.02 \
                    --RGBD/AngularUpdate 0.02 \
                    --Reg/Strategy 0 \
                    --Reg/Force3DoF true \
                    --Mem/ImageCompression true \
                    --Mem/CompressionParallelized true \
                    --Mem/UseOdomFeatures false \
                    --GFTT/QualityLevel 0.005 \
                    --RGBD/ProximityMaxGraphDepth 3 \
                    --RGBD/NeighborLinkRefining true \
                    --RGBD/LoopThr 0.09 \
                    --RGBD/ProximityPathMaxNeighbors 3 \
                    --RGBD/NeighborLinkRefining true \
                    --RGBD/LoopThr 0.09 \
                    --RGBD/LoopMaxGraphDepth 2 \
                    --RGBD/LoopMinInliers 20 \
                    --Icp/CorrespondenceRatio 0.2 \
                    --Icp/MaxCorrespondenceDistance 0.1 \
                    --Icp/MaxTranslation 0.5 \
                    --Icp/MaxRotation 0.5 \
                    --Icp/VoxelSize 0.02 \
                    --Icp/PointToPlane true \
                    --Reg/ScanReg true \
                    --Reg/ScanRegICP true \
                    --Reg/Force2D true \
                    --RGBD/ProximityByTime false \
                    --RGBD/ProximityPathMaxNeighbors 2 \
                    --Mem/LocalLoopDetection true \
                    --Mem/RehearsalSimilarity 0.3 \
                    --Rtabmap/PublishRAMUsage true \
                    --Rtabmap/DetectionRate 2 \
                    --Vis/EstimationType 1 \
                    --Vis/CorType 1 \
                    --Vis/FeatureType 1',
                
                'rviz': 'true'
            }.items()
        ),
    ])