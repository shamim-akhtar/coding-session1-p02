using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
  public static Vector3 CameraAngleOffset { get; set; }
  public static Vector3 CameraPositionOffset { get; set; }
  public static float Damping { get; set; }
}


// lets create some simple TPC controls.
public abstract class TPCBase
{
  // allow only child classes to have access
  // to these two variables.
  protected Transform mCameraTransform;
  protected Transform mPlayerTransform;

  // constructor.
  public TPCBase(Transform camera, Transform player)
  {
    mCameraTransform = camera;
    mPlayerTransform = player;
  }

  public abstract void Tick();
}

public class TPCTrack : TPCBase
{
  public TPCTrack(Transform camera, Transform player)
    : base(camera, player)
  {
  }

  public override void Tick()
  {
    const float playerHeight = 2.0f;
    Vector3 targetPos = mPlayerTransform.position;
    targetPos.y += playerHeight;
    mCameraTransform.LookAt(targetPos);
  }
}

public abstract class TPCFollow : TPCBase
{
  public TPCFollow(Transform camera, Transform player)
    : base(camera, player)
  {
  }

  public override void Tick()
  {
    // Now we calculate the camera transformed axes.
    // We do this because our camera's rotation might have changed
    // in the derived class Update implementations. Calculate the new 
    // forward, up and right vectors for the camera.
    Vector3 forward = mCameraTransform.rotation * Vector3.forward;
    Vector3 right = mCameraTransform.rotation * Vector3.right;
    Vector3 up = mCameraTransform.rotation * Vector3.up;

    // We then calculate the offset in the camera's coordinate frame. 
    // For this we first calculate the targetPos
    Vector3 targetPos = mPlayerTransform.position;

    // Add the camera offset to the target position.
    // Note that we cannot just add the offset.
    // You will need to take care of the direction as well.
    Vector3 desiredPosition = targetPos
        + forward * GameConstants.CameraPositionOffset.z
        + right * GameConstants.CameraPositionOffset.x
        + up * GameConstants.CameraPositionOffset.y;

    // Finally, we change the position of the camera, 
    // not directly, but by applying Lerp.
    Vector3 position = Vector3.Lerp(mCameraTransform.position,
        desiredPosition, Time.deltaTime * GameConstants.Damping);
    mCameraTransform.position = position;

  }
}
public class TPCFollowTrackPosition : TPCFollow
{
  public TPCFollowTrackPosition(Transform cameraTransform, Transform playerTransform)
      : base(cameraTransform, playerTransform)
  {
  }

  public override void Tick()
  {
    // Create the initial rotation quaternion based on the 
    // camera angle offset.
    Quaternion initialRotation =
       Quaternion.Euler(GameConstants.CameraAngleOffset);

    // Now rotate the camera to the above initial rotation offset.
    // We do it using damping/Lerp
    // You can change the damping to see the effect.
    mCameraTransform.rotation =
        Quaternion.RotateTowards(mCameraTransform.rotation,
            initialRotation,
            Time.deltaTime * GameConstants.Damping);

    // We now call the base class Update method to take care of the
    // position tracking.
    base.Tick();
  }
}
public class TPCFollowTrackPositionAndRotation : TPCFollow
{
  public TPCFollowTrackPositionAndRotation(Transform cameraTransform, Transform playerTransform)
      : base(cameraTransform, playerTransform)
  {
  }

  public override void Tick()
  {
    // We apply the initial rotation to the camera.
    Quaternion initialRotation =
        Quaternion.Euler(GameConstants.CameraAngleOffset);

    // Allow rotation tracking of the player
    // so that our camera rotates when the Player rotates and at the same
    // time maintain the initial rotation offset.
    mCameraTransform.rotation = Quaternion.Lerp(
        mCameraTransform.rotation,
        mPlayerTransform.rotation * initialRotation,
        Time.deltaTime * GameConstants.Damping);

    base.Tick();
  }
}

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
