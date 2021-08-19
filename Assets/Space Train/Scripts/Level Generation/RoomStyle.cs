using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    [CreateAssetMenu(fileName = "New RoomStyle", menuName = "Level Generation/Room Style")]
    public class RoomStyle : ScriptableObject
    {
        [SerializeField] private List<PropSpawningInfo> props = new List<PropSpawningInfo>();
        public List<PropSpawningInfo> Props => props;
        [SerializeField] private GameObject floor;
        public GameObject Floor => floor;
        [SerializeField] private GameObject wall;
        public GameObject Wall => wall;
        [SerializeField] private GameObject door;
        public GameObject Door => door;
    }
}
