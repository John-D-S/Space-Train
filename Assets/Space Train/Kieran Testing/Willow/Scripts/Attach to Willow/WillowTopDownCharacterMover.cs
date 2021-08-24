using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Willow
{

    // This is in the same folder hopefully.
    [RequireComponent(typeof(WillowInputHandler))]
    // Make sure this is linked to "Willow with animations.
    [RequireComponent(typeof(Animator))]
    // Make sure to Freeze Rotation on X, Y and Z, Mass is 10 and it Uses Gravity.
    [RequireComponent(typeof(Rigidbody))]
    // Make sure it fits around the body.
    [RequireComponent(typeof(CapsuleCollider))]
    
    // This is to actually move Willow and deal with the animations. 
    public class WillowTopDownCharacterMover : MonoBehaviour
    {
        // This is the Input Handler that will 
        private WillowInputHandler myWillowInputHandler;

        [SerializeField] private bool RotateTowardMouse = false;

        [SerializeField] private float MovementSpeed = 1.6f;

        [SerializeField] private float RunSpeed = 6.68f;

        [SerializeField] private float RotationSpeed = 4f;

        [SerializeField] private float RunRotationSpeed = 11f;

        [SerializeField] private Camera Camera;
        private void Awake()
        {
            myWillowInputHandler = GetComponent<WillowInputHandler>();
        }

        // Update is called once per frame.
        private void Update()
        {
            Vector3 targetVector = new Vector3(myWillowInputHandler.InputVector.x, 0, myWillowInputHandler.InputVector.y);
            Vector3 movementVector = MoveTowardTarget(targetVector);

            if(!RotateTowardMouse)
            {
                RotateTowardMovementVector(movementVector);
            }

            if(RotateTowardMouse)
            {
                RotateFromMouseVector();
            }
        }

        // Implementing IF we want to follow the mouse.
        private void RotateFromMouseVector()
        {
            Ray ray = Camera.ScreenPointToRay(myWillowInputHandler.MousePosition);

            if(Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
            {
                Vector3 target = hitInfo.point;
                target.y = transform.position.y;
                transform.LookAt(target);
            }
        }

        private Vector3 MoveTowardTarget(Vector3 targetVector)
        {
            float speed = new float();

            // If Walking Speed = Walking.
            if(myWillowInputHandler.myWillowMovementState == WillowMovementState.Walking)
            {
                speed = MovementSpeed * Time.deltaTime;
            }
            // If Running Speed = Running.
            else if(myWillowInputHandler.myWillowMovementState == WillowMovementState.Running)
            {
                speed = RunSpeed * Time.deltaTime;
            }
            // If Idle Speed = 0.
            else
            {
                speed = 0;
            }

            targetVector = Quaternion.Euler(0, Camera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
            Vector3 targetPosition = transform.position + targetVector * speed;
            transform.position = targetPosition;
            return targetVector;
        }

        private void RotateTowardMovementVector(Vector3 movementDirection)
        {
            float rotatingSpeed = new float();

            // If rotatingSpeed = Walking.
            if(myWillowInputHandler.myWillowMovementState == WillowMovementState.Walking)
            {
                rotatingSpeed = RotationSpeed;
            }
            // If rotatingSpeed = Running.
            else if(myWillowInputHandler.myWillowMovementState == WillowMovementState.Running)
            {
                rotatingSpeed = RunRotationSpeed;
            }
            // If Idle rotatingSpeed = 0.
            else
            {
                rotatingSpeed = 0;
            }

            if(movementDirection.magnitude == 0)
            {
                return;
            }

            Quaternion rotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotatingSpeed);
        }
    }
}