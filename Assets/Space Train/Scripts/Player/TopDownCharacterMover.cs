using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerVisability
{
    Visable, Invisable
}

[RequireComponent(typeof(InputHandler))]
public class TopDownCharacterMover : MonoBehaviour
{
    private InputHandler _input;

    [SerializeField]
    private bool RotateTowardMouse;

    [SerializeField]
    private float MovementSpeed;
    
    [SerializeField]
    private float RunSpeed;
    
    [SerializeField]
    private float RotationSpeed;
    
    [SerializeField]
    private float RunRotationSpeed;

    [SerializeField]
    private Camera Camera;

    public PlayerVisability myPlayerVisability;
    
    private void Awake()
    {
        _input = GetComponent<InputHandler>();
    }

    private void Start()
    {
        myPlayerVisability = PlayerVisability.Visable;
    }

    // This will be called in Animations at the end of stealth animation.
    public void MakePlayerInvisable()
    {
        myPlayerVisability = PlayerVisability.Invisable;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 targetVector = new Vector3(_input.InputVector.x, 0, _input.InputVector.y);
        Vector3 movementVector = MoveTowardTarget(targetVector);

        if (!RotateTowardMouse)
        {
            RotateTowardMovementVector(movementVector);
        }
        if (RotateTowardMouse)
        {
            RotateFromMouseVector();
        }
    }

    // Implementing IF we want to follow the mouse.
    private void RotateFromMouseVector()
    {
        Ray ray = Camera.ScreenPointToRay(_input.MousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
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
        if(_input.myPlayerState == PlayerState.Walking)
        {
            speed = MovementSpeed * Time.deltaTime;
        }
        // If Running Speed = Running.
        else if(_input.myPlayerState == PlayerState.Running)
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
        if(_input.myPlayerState == PlayerState.Walking)
        {
            rotatingSpeed = RotationSpeed;
        }
        // If rotatingSpeed = Running.
        else if(_input.myPlayerState == PlayerState.Running)
        {
            rotatingSpeed = RunRotationSpeed;
        }
        // If Idle rotatingSpeed = 0.
        else
        {
            rotatingSpeed = 0;
        }
        if(movementDirection.magnitude == 0) { return; }
        Quaternion rotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotatingSpeed);
    }
}
