using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RoomStyle", menuName = "Level Generation/Room Style")]
public class RoomStyle : ScriptableObject
{
    [SerializeField] private string roomTag;
    [SerializeField] private List<string> LinkableRoomTags = new List<string>();

    [SerializeField] private GameObject floor;
    public GameObject Floor
    {
        get => floor;
    }
    [SerializeField] private GameObject wall;
    public GameObject Wall
    {
        get => wall;
    }
    [SerializeField] private GameObject door;
    public GameObject Door
    {
        get => door;
    }
}
