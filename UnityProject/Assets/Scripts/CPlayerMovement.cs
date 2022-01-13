using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerMovement : MonoBehaviour
{
  [SerializeField]
  CharacterController mCharacterController;
  [SerializeField]
  Animator mAnimator;

  public float mWalkSpeed = 1.5f;
  public float mRotationSpeed = 50.0f;


  // Update is called once per frame
  void Update()
  {
    // The following code takes input and uses it for
    // the walk/run movement.
    // Since we are using a shift button key to 
    // change from walking to running,
    // I might need to rethink how I will need to do
    // for mobile touch device.
    // TODO: Change this for mobile devices.
    float hInput = Input.GetAxis("Horizontal");
    float vInput = Input.GetAxis("Vertical");

    float speed = mWalkSpeed;

    if (Input.GetKey(KeyCode.LeftShift))
    {
      speed = mWalkSpeed * 2.0f;
    }

    if (mAnimator == null) return;

    // In the section below we rotate the player
    // based on the rotation speed and attennuate it with
    // the delta time.
    transform.Rotate(
      0.0f, 
      hInput * mRotationSpeed * Time.deltaTime, 
      0.0f);

    Vector3 forward =
        transform.TransformDirection(Vector3.forward).normalized;
    forward.y = 0.0f;

    mCharacterController.Move(forward * vInput * speed * Time.deltaTime);

    mAnimator.SetFloat("PosX", 0);
    mAnimator.SetFloat("PosZ", vInput * speed / (2.0f * mWalkSpeed));
  }
}
