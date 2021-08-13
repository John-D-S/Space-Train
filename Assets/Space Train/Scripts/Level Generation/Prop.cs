using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    [SerializeField] private Vector2Int propSize;
    public Vector2Int PropSize => propSize;
}

[System.Serializable]
public class PropSpawningInfo
{
    [SerializeField, Tooltip("The prop component of the prop prefab")] 
    private Prop prop;
    [SerializeField, Tooltip("The Target Minimum number of this Prop that can spawn in a room")] 
    private int minNumberInRoom;
    [SerializeField, Tooltip("The Maximum number of this prop that can be placed in a room")] 
    private int maxNumberInRoom;
    public Prop PropComponent => prop;
    public GameObject PropGameObject => prop != null ? prop.gameObject : null;
    public int MinNumberInRoom => minNumberInRoom;
    public int MaxNumberInRoom => maxNumberInRoom;
}
