using UnityEngine;

namespace TPC
{
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
}