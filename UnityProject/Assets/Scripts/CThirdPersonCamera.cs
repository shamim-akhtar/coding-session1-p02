using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPC;

/// <summary>
/// The below is the monobehavior script for tpc.
/// Remember we are not implementing the logic for the
/// tpc in this script.
/// Rather, we will create separate individual classes
/// responsible for a specific third-person camera 
/// behavior.
/// </summary>
public class CThirdPersonCamera : MonoBehaviour
{
  public Transform mCameraTransform;
  public Transform mPlayerTransform;
  public Vector3 CameraAngleOffset;
  public Vector3 CameraPositionOffset;
  public float Damping;

  public enum CameraType
  {
    TRACK,
    TRACK_POS,
    TRAC_POS_ROT,
    //CAMERA_ON_DRIVER_SEAT,
    //CAMERA_REAR_VIEW,
    //CAMERA_TRACK_VEHICLE_FROM_SIDE,
  }
  public CameraType myCameraType = CameraType.TRACK;

  Dictionary<CameraType, TPCBase> myCameras = new Dictionary<CameraType, TPCBase>();
  //TPCBase myCamera;

  // Start is called before the first frame update
  void Start()
  {
    myCameras.Add(CameraType.TRACK, new TPCTrack(mCameraTransform, mPlayerTransform));
    myCameras.Add(CameraType.TRACK_POS, new TPCFollowTrackPosition(mCameraTransform, mPlayerTransform));
    myCameras.Add(CameraType.TRAC_POS_ROT, new TPCFollowTrackPositionAndRotation(mCameraTransform, mPlayerTransform));

    GameConstants.CameraAngleOffset = CameraAngleOffset;
    GameConstants.CameraPositionOffset = CameraPositionOffset;
    GameConstants.Damping = Damping;
  }

  private void Update()
  {
    GameConstants.CameraAngleOffset = CameraAngleOffset;
    GameConstants.CameraPositionOffset = CameraPositionOffset;
    GameConstants.Damping = Damping;
  }

  // Update is called once per frame
  void LateUpdate()
  {
    //myCamera.Tick();
    myCameras[myCameraType].Tick();
  }
}
