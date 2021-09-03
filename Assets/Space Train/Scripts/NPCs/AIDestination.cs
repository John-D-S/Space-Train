using SpaceTrain.Player;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NpcAi
{
    public class AIDestination : MonoBehaviour
    {
        private List<CharacterIdentity> destinationIdentity;
        public static Dictionary<CharacterIdentity, List<AIDestination>> aiDestinationsByAllowedCharacters = new Dictionary<CharacterIdentity, List<AIDestination>>();

        private void Start()
        {
            foreach(CharacterIdentity identity in destinationIdentity)
            {
                aiDestinationsByAllowedCharacters[identity].Add(this);
            }
        }
    }
}
