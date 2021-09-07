using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Door : MonoBehaviour, IInterractable
{
    [SerializeField, Tooltip("Whether or not the door will open when NPCs are nearby,")] private bool npcsCanOpen = true;
    [SerializeField, Tooltip("Whether or not the door requires all keys to be collected before the player opens it.")] private bool requiresKeysToOpen;
    [SerializeField, Tooltip("The part of the door that moves up and down")] private GameObject doorPart;
    [SerializeField, Tooltip("Whether the door will open when it is instantiated into the world.")] private bool openOnStart;
    [SerializeField, Tooltip("The door will not close when any NPCs or the character are within this radius.")] private float distanceToCheckForCharacters = 2f;
    //the localPosition.y of the door at its lowest
    private float closedDoorHeight;
    //the localPosition.y of the door at its highest
    [SerializeField, Tooltip("The height of the door when it is opened")] private float openedDoorHeight = 10;
    [SerializeField, Tooltip("How fast the door moves when it opens.")] private float doorMoveSpeed = 3;
    private float DoorYpos => doorPart.transform.localPosition.y;
    private Vector3 initialDoorPos;
    
    [System.NonSerialized] public bool open;

    /// <summary>
    /// gets the target height of the door based on whether or not the bool open is true
    /// </summary>
    private float TargetHeight
    {
        get
        {
            if(requiresKeysToOpen)
            {
                if(Key.numberOfKeysCollected == Key.requiredNumberOfKeys)
                {
                    if (open)
                        return openedDoorHeight;
                }
                return closedDoorHeight;
            } 
            if (open)
               return openedDoorHeight; 
            return closedDoorHeight;
        }
    }

    /// <summary>
    /// changes the door gameobjects position in the world so that its y height matches _YPos
    /// </summary>
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

    /// <summary>
    /// toggles the open variable
    /// </summary>
    public void Interract()
    {
        open = !open;
        StartCoroutine(CloseDoorWhenClearOfCharacters());
    }

    /// <summary>
    /// close the door if there are no colliders with the "Character" tag nearby
    /// </summary>
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

    /// <summary>
    /// returns whether or not there are colliders with the "Character" tag nearby
    /// </summary>
    bool NpcsNearby
    {
        get
        {
            List<Collider> nearbyColliders = Physics.OverlapSphere(initialDoorPos, distanceToCheckForCharacters, LayerMask.GetMask("Character")).ToList();
            foreach(Collider nearbyCollider in nearbyColliders)
            {
                if(nearbyCollider.transform.gameObject.CompareTag("NPC"))
                {
                    return true;
                }
            }
            return false;
        }
    }
    
    void FixedUpdate()
    {
        if(npcsCanOpen)
        {
            if(!open && NpcsNearby)
            {
                Interract();
            }
        }
        
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
