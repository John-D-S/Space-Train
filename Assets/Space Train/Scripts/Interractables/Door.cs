using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInterractable
{
    [SerializeField]
    private GameObject doorPart;
    [SerializeField] private bool openOnStart;
    [SerializeField, Tooltip("The door will not close when any NPCs or the character are within this radius.")] private float distanceToCheckForCharacters = 2f;
    //the localPosition.y of the door at its lowest
    private float closedDoorHeight;
    //the localPosition.y of the door at its highest
    [SerializeField] private float openedDoorHeight = 10;
    [SerializeField] private float doorMoveSpeed = 3;
    private float DoorYpos => doorPart.transform.localPosition.y;
    private Vector3 initialDoorPos;
    
    [System.NonSerialized]
    public bool open = false;

    //gets the target height of the door based on whether or not the bool open is true
    private float TargetHeight
    {
        get
        {
            if (open)
                return openedDoorHeight;
            else
                return closedDoorHeight;
        }
    }

    private void SetDoorHeight(float _YPos) => doorPart.transform.localPosition = new Vector3(doorPart.transform.localPosition.x, _YPos, doorPart.transform.localPosition.z);

    private void Start()
    {
        closedDoorHeight = DoorYpos;
        initialDoorPos = transform.position;
        if(openOnStart)
        {
            Interract();
        }
    }

    public void Interract()
    {
        open = !open;
        StartCoroutine(CloseDoorWhenClearOfCharacters());
    }

    private IEnumerator CloseDoorWhenClearOfCharacters()
    {
        while(open)
        {
            bool charactersNearby = Physics.CheckSphere(initialDoorPos, distanceToCheckForCharacters, LayerMask.GetMask("Character"));
            if(!charactersNearby)
            {
                open = false;
            }
            yield return new WaitForFixedUpdate();
        }
    }
    
    void FixedUpdate()
    {
        //if the door's height is below its target, move it up until it isn't; visa versa for if it is above it's target.
        if (DoorYpos < TargetHeight)
        {
            doorPart.transform.localPosition += Vector3.up * (Time.deltaTime * doorMoveSpeed);
            if (DoorYpos > TargetHeight)
                SetDoorHeight(TargetHeight);
                
        }
        else if (DoorYpos > TargetHeight)
        {
            doorPart.transform.localPosition -= Vector3.up * (Time.deltaTime * doorMoveSpeed);
            if (DoorYpos < TargetHeight)
                SetDoorHeight(TargetHeight);
        }
    }
}
