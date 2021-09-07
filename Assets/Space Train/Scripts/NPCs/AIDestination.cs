using SpaceTrain.Player;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NpcAi
{
    public class AIDestination : MonoBehaviour
    {
        [SerializeField, Tooltip("The list of types of NPCs that can use this AIDestination")] private List<CharacterIdentity> destinationIdentity = new List<CharacterIdentity>();
        //the dictionary that contains all the ai destinations, sorted by the characters that can visit them.
        public static Dictionary<CharacterIdentity, List<AIDestination>> aiDestinationsByAllowedCharacters = new Dictionary<CharacterIdentity, List<AIDestination>>();

        private void Awake()
        {
            //wipe this every time the level is loaded anew
            aiDestinationsByAllowedCharacters.Clear();
        }

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
