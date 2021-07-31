using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStyle : ScriptableObject
{
    [SerializeField] private string roomStyleName;
    [SerializeField] private List<string> allowedLinkedRooms = new List<string>();
}
