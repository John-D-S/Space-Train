using SpaceTrain.Player;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NpcAi
{
    public class AIDestination : MonoBehaviour
    {
        [SerializeField] private List<CharacterIdentity> destinationIdentity = new List<CharacterIdentity>();
        public static Dictionary<CharacterIdentity, List<AIDestination>> aiDestinationsByAllowedCharacters = new Dictionary<CharacterIdentity, List<AIDestination>>();

        private void Start()
        {
            foreach(CharacterIdentity identity in destinationIdentity)
            {
                if(!aiDestinationsByAllowedCharacters.ContainsKey(identity))
                {
                    aiDestinationsByAllowedCharacters.Add(identity, new List<AIDestination>());
                }
                aiDestinationsByAllowedCharacters[identity].Add(this);
            }
        }
    }
}
