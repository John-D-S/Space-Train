using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceTrain.Player
{
	public class ZoneTag : MonoBehaviour
	{
		public static readonly Dictionary<GameObject, List<CharacterIdentity>> allowedIdentitiesByFloorGO = new Dictionary<GameObject, List<CharacterIdentity>>();

		[SerializeField] private List<CharacterIdentity> allowedIdentities = new List<CharacterIdentity>();
		private void Start()
		{
			allowedIdentitiesByFloorGO[gameObject] = allowedIdentities;
		}
	}
}
