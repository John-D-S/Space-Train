using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Willow;

namespace SpaceTrain.Player
{
    // This is in the same folder hopefully.
    [RequireComponent(typeof(InputHandler))]
    // Make sure this is linked to "Willow with animations.
    [RequireComponent(typeof(Animator))]
    // Make sure to Freeze Rotation on X, Y and Z, Mass is 10 and it Uses Gravity.
    [RequireComponent(typeof(Rigidbody))]
    // Make sure it fits around the body.
    [RequireComponent(typeof(CapsuleCollider))]
    public class TopDownCharacterMover : MonoBehaviour
    {
        // This is the Input Handler that will take in the input of the player.
        private InputHandler _input;

        // Turn this on if you want to move the mouse towards the player.
        [SerializeField]
        private bool RotateTowardMouse; 
        
        // The movement speed, self explanatary.
        [SerializeField]
        private float MovementSpeed;
    
        // Run Speed, self explanatary.
        [SerializeField]
        private float RunSpeed;
    
        // Rotation self explainatary.
        [SerializeField]
        private float RotationSpeed;

        // Rotation speed while while holding 'run'
        [SerializeField]
        private float RunRotationSpeed;

        // The myCamera, assign to myCamera in scene.
        [SerializeField]
        private Camera Camera;

        [SerializeField, Tooltip("How close you must be to interract with a thing.")] private float interractDistance = 1;
    
        // Gets the Camera and Input Handeler in Scene.
        private void Awake()
        {
            _input = GetComponent<InputHandler>();
            Camera = Camera.main;

        }

        private void Interract()
        {
            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interractDistance))
            {
                IInterractable interractable = hit.collider.gameObject.GetComponent<IInterractable>();
                if(interractable != null)
                {
                    interractable.Interract();
                }
            }
        }
        
        // Update is called once per frame.
        private void Update()
        {
            // Gets the Vector 3 of the inputed vector.
            Vector3 targetVector = new Vector3(_input.InputVector.x, 0, _input.InputVector.y);
            
            // Moves towards the target.
            Vector3 movementVector = MoveTowardTarget(targetVector);

            // If it is to Rotating just with the keys.
            if (!RotateTowardMouse)
            {
                // Run the rotation script which will rotate with the keys.
                RotateTowardMovementVector(movementVector);
            }
            // If rotating with the mouse.
            if (RotateTowardMouse)
            {
                // Will use a Raycast to rotate.
                RotateFromMouseVector();
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                Interract();
            }
        }

        // Implementing IF we want to follow the mouse.
        private void RotateFromMouseVector()
        {
            // Here is where the mouse is on the screen.
            Ray ray = Camera.ScreenPointToRay(_input.MousePosition);

            // Basically if the mouse hits anything 300 meters away.
            if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
            {
                // Gets the hitpoint Vector 3.
                Vector3 target = hitInfo.point;
                // Gets its y position.
                target.y = transform.position.y;
                // Gets this game object to look at target.
                transform.LookAt(target);
            }
        }

        // If just using the keys it will implement this.
        private Vector3 MoveTowardTarget(Vector3 targetVector)
        {
            // This is the movement speed it will run at.
            float speed = new float();
        
            // If Walking Speed = Walking.
            if(_input.myMovementState == MovementState.Walking)
            {
                // Speed = the normal movement speed.
                speed = MovementSpeed * Time.deltaTime;
            }
            // If Running Speed = Running.
            else if(_input.myMovementState == MovementState.Running)
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
            targetVector = Quaternion.Euler(0, Camera.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
            // The position of the mouse to turn towards.
            Vector3 targetPosition = transform.position + targetVector * speed;
            // Goes to the new position.
            transform.position = targetPosition;
            // Will output the target vector for if not rotating with the mouse
            return targetVector;
        }

        /// <summary> Rotation with the keyboard. </summary>
        /// <param name="movementDirection"></param>
        private void RotateTowardMovementVector(Vector3 movementDirection)
        {
            // This will be the speed of rotation.
            float rotatingSpeed = new float();
        
            // If rotatingSpeed = Walking.
            if(_input.myMovementState == MovementState.Walking)
            {
                // Rotate normally if just a normal rotation.
                rotatingSpeed = RotationSpeed;
            }
            // If rotatingSpeed = Running.
            else if(_input.myMovementState == MovementState.Running)
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