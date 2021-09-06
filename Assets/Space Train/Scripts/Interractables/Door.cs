using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Door : MonoBehaviour, IInterractable
{
    [SerializeField] private bool npcsCanOpen = true;
    [SerializeField] private bool requiresKeysToOpen;
    [SerializeField] private GameObject doorPart;
    [SerializeField] private bool openOnStart;
    [SerializeField, Tooltip("The door will not close when any NPCs or the character are within this radius.")] private float distanceToCheckForCharacters = 2f;
    //the localPosition.y of the door at its lowest
    private float closedDoorHeight;
    //the localPosition.y of the door at its highest
    [SerializeField] private float openedDoorHeight = 10;
    [SerializeField] private float doorMoveSpeed = 3;
    private float DoorYpos => doorPart.transform.localPosition.y;
    private Vector3 initialDoorPos;
    
    [System.NonSerialized] public bool open;

    //gets the target height of the door based on whether or not the bool open is true
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
