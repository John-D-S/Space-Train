using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceTrain.Player
{
	/// <summary>
	/// the tag given to floor tiles which determines whether somehting is in a given zone.
	/// </summary>
	public class ZoneTag : MonoBehaviour
	{
		[SerializeField] private List<CharacterIdentity> allowedIdentities = new List<CharacterIdentity>();
		
		public static readonly Dictionary<GameObject, List<CharacterIdentity>> allowedIdentitiesByFloorGO = new Dictionary<GameObject, List<CharacterIdentity>>();
		
		private void Start()
		{
			allowedIdentitiesByFloorGO[gameObject] = allowedIdentities;
		}
	}
}
