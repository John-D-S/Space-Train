using SpaceTrain.Player;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NpcAi
{
    public class AIDestination : MonoBehaviour
    {
        private CharacterIdentity destinationIdentity;
        public static Dictionary<int, AIDestination> aiDestinationsByLevelNumber = new Dictionary<int, AIDestination>();
    }
}
