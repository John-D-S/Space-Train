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
        // This is the Input Handler that will.
        private WillowInputHandler myWillowInputHandler;

        // Turn this on if you want to move the mouse towards the player.
        [SerializeField] private bool RotateTowardMouse = false;

        // The movement speed, self explanatary.
        [SerializeField] private float MovementSpeed = 1.6f;

        // Run Speed, selsh explanatary.
        [SerializeField] private float RunSpeed = 6.68f;

        // Rotation self explainatary.
        [SerializeField] private float RotationSpeed = 4f;

        // Rotation speed while while holding 'run'
        [SerializeField] private float RunRotationSpeed = 11f;

        // The myCamera, assign to myCamera in scene.
        [SerializeField] private Camera myCamera;
        
        // Gets the Camera and Input Handeler in Scene.
        private void Awake()
        {
            myCamera = Camera.main;
            myWillowInputHandler = GetComponent<WillowInputHandler>();
        }

        // Update is called once per frame.
        private void Update()
        {
            // Gets the Vector 3 of the inputed vector
            Vector3 targetVector = new Vector3(myWillowInputHandler.InputVector.x, 0, myWillowInputHandler.InputVector.y);
            
            // Moves towards the target.
            Vector3 movementVector = MoveTowardTarget(targetVector);

            // If it is to Rotating just with the keys.
            if(!RotateTowardMouse)
            {
                // Run the rotation script which will rotate with the keys.
                RotateTowardMovementVector(movementVector);
            }

            // If rotating with the mouse.
            if(RotateTowardMouse)
            {
                // Will use a Raycast to rotate
                RotateFromMouseVector();
            }
        }

        // Implementing IF we want to follow the mouse.
        private void RotateFromMouseVector()
        {
            // Here is where the mouse is on the screen.
            Ray ray = myCamera.ScreenPointToRay(myWillowInputHandler.MousePosition);

            // Basically if the mouse hits anything 300 meters away
            if(Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
            {
                // Gets the hitpoint Vector 3.
                Vector3 target = hitInfo.point;
                // Gets its y position.
                target.y = transform.position.y;
                // Gets this game object to look at target.
                transform.LookAt(target);
            }
        }

        // If Just using the keys it will implement this.
        private Vector3 MoveTowardTarget(Vector3 targetVector)
        {
            // this is the movement speed it will run at.
            float speed = new float();

            // If Walking Speed = Walking.
            if(myWillowInputHandler.myWillowMovementState == WillowMovementState.Walking)
            {
                // Speed = the normal movement speed.
                speed = MovementSpeed * Time.deltaTime;
            }
            // If Running Speed = Running.
            else if(myWillowInputHandler.myWillowMovementState == WillowMovementState.Running)
            {
                // Speed will = run speed.
                speed = RunSpeed * Time.deltaTime;
            }
            // If Idle Speed = 0.
            else
            {
                // Stops moving.
                speed = 0;
            }

            // Will get the inputed Vector and will move towards the target.
            targetVector = Quaternion.Euler(0, myCamera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
            // The position of the mouse to turn towards.
            Vector3 targetPosition = transform.position + targetVector * speed;
            // Goes to the new position.
            transform.position = targetPosition;
            
            // Will output the target vector for if not rotating with the mouse
            return targetVector;
        }

        // Rotation with the keyboard.
        private void RotateTowardMovementVector(Vector3 movementDirection)
        {
            // This will be the speed of rotation.
            float rotatingSpeed = new float();

            // If rotatingSpeed = Walking.
            if(myWillowInputHandler.myWillowMovementState == WillowMovementState.Walking)
            {
                // Rotate normally if just a normal rotation.
                rotatingSpeed = RotationSpeed;
            }
            // If rotatingSpeed = Running.
            else if(myWillowInputHandler.myWillowMovementState == WillowMovementState.Running)
            {
                // Rotate faster if running.
                rotatingSpeed = RunRotationSpeed;
            }
            // If Idle rotatingSpeed = 0.
            else
            {
                // No rotation if idle.
                rotatingSpeed = 0;
            }

            // If no movement directional input dont dont turn.
            if(movementDirection.magnitude == 0)
            {
                return;
            }

            // Will get the new rotation to look at using the disired input.
            Quaternion rotation = Quaternion.LookRotation(movementDirection);
            
            // Rotates towards this Direction at the current rotation speed.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotatingSpeed);
        }
    }
}