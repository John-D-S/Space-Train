using LevelGeneration;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] private Vector2Int propSize;
    public Vector2Int PropSize => propSize;

    [Header("-- Repeat Settings --")]
    [SerializeField] private bool repeat;
    public bool Repeat => repeat;
    [SerializeField] private Direction repeatDirection;
    public Direction RepeatDirection => repeatDirection;

    [Header("-- Wall Positioning Settings --")] 
    [SerializeField, Tooltip("checking this means that the positive z side of the prop must be placed against a wall during level generation")] 
    private bool zPosWallPlacement;
    public bool ZPosWallPlacement => zPosWallPlacement;
    [SerializeField, Tooltip("checking this means that the positive x side of the prop must be placed against a wall during level generation")] 
    private bool xPosWallPlacement;
    public bool XPosWallPlacement => xPosWallPlacement;
    [SerializeField, Tooltip("checking this means that the negative z side of the prop must be placed against a wall during level generation")] 
    private bool zNegWallPlacement;
    public bool ZNegWallPlacement => zNegWallPlacement;
    [SerializeField, Tooltip("checking this means that the negative x side of the prop must be placed against a wall during level generation")]
    private bool xNegWallPlacement;
    public bool XNegWallPlacement => xNegWallPlacement;
    
    [SerializeField, Tooltip("This does not affect generation or prop placement, just how the border gizmo appears in game.")] 
    private float tileSize = 2; 
    
    //this will draw a square around the prop to show its size when it it is being tweaked in the inspector.
    //the sides marked with green show in which direction the prop will try to repeat
    //and the sides marked with red show which sides will be placed adjacent to a tile with a wall.
    private void OnDrawGizmosSelected()
    {
        float yPropSize = propSize.y * tileSize;
        float xPropSize = propSize.x * tileSize;
        if(repeat)
        {
            Gizmos.color = Color.green;
            switch(repeatDirection)
            {
                case Direction.Zpos:
                    Gizmos.DrawLine(new Vector3(0, 0.1f, yPropSize), new Vector3(xPropSize, 0.1f, yPropSize));                           
                    break;
                case Direction.Xpos:
                    Gizmos.DrawLine(new Vector3(xPropSize, 0.1f, yPropSize), new Vector3(xPropSize, 0.1f, 0));                    
                    break;
                case Direction.Zneg:
                    Gizmos.DrawLine(new Vector3(xPropSize, 0.1f, 0), new Vector3(0, 0.1f, 0));            
                    break;
                case Direction.Xneg:
                    Gizmos.DrawLine(new Vector3(0, 0.1f, 0), new Vector3(0, 0.1f, yPropSize));            
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }    
        }
        Gizmos.color = zPosWallPlacement
            ? Color.red
            : Color.gray;
        Gizmos.DrawLine(new Vector3(0, 0, yPropSize), new Vector3(xPropSize, 0, yPropSize));
        Gizmos.color = xPosWallPlacement
            ? Color.red
            : Color.gray;
        Gizmos.DrawLine(new Vector3(xPropSize, 0, yPropSize), new Vector3(xPropSize, 0, 0));
        Gizmos.color = zNegWallPlacement
            ? Color.red
            : Color.gray;
        Gizmos.DrawLine(new Vector3(xPropSize, 0, 0), new Vector3(0, 0, 0));
        Gizmos.color = xNegWallPlacement
            ? Color.red
            : Color.gray;
        Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 0, yPropSize));
    }
}

/// <summary>
/// used so that a list of information about prop spawning can be conveniantley made in roomstyle scriptable objects
/// </summary>
[System.Serializable]
public class PropSpawningInfo
{
    [SerializeField, Tooltip("The prop component of the prop prefab")] 
    private Prop prop;
    [SerializeField, Tooltip("The Maximum number of this prop that can be placed in a room")] 
    private int maxNumberInRoom;
    public Prop PropComponent => prop;
    public GameObject PropGameObject => prop != null ? prop.gameObject : null;
    public int MaxNumberInRoom => maxNumberInRoom;
}
