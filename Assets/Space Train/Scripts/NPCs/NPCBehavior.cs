using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public enum AIState
{
	Casual,
	Alerted
}

public abstract class NPCBehavior : MonoBehaviour
{
	private static List<NPCBehavior> allNPCs;
	private List<NPCBehavior> nearbyNPCs;
	private List<NPCBehavior> visibleNPCs;
	
	[SerializeField] private Emote emote;
	[SerializeField] private float maxSuspicion;
	[SerializeField, Tooltip("The Npc will only be aware of other npcs and the player if they are within this distance.")] private float nearbyDistance = 16;
	
	[Header("-- Ai Sight Settings --")]
	[SerializeField] private float fieldOfView;
	[SerializeField] private float eyeHeight;

	private float suspicionOfPlayer;
	private AIState currentState;
	private static GameObject player;
	private bool alive = true;
	public bool Alive => alive;

	private bool PlayerIsVisible
	{
		get
		{
			return ObjectIsVisible(player);
		}
	}

	private bool ObjectIsVisible(GameObject _gameObject)
	{
		if(_gameObject)
		{
			if(Vector3.Angle(transform.forward, player.transform.position) < .5f * fieldOfView)
			{
				RaycastHit hit = new RaycastHit();
				Physics.Raycast(transform.position + Vector3.up * eyeHeight, _gameObject.transform.position, out hit);
				if(hit.collider.gameObject == _gameObject)
				{
					return true;
				}
			}	
		}
		return false;
	}

	private void UpdateNearbyNPCs()
	{
		List<NPCBehavior> currentNearbyNPCs = new List<NPCBehavior>();
		foreach(NPCBehavior NPC in allNPCs)
		{
			//just using taxicab distance to find nearby npcs for performance
			if(Mathf.Abs(transform.position.x - NPC.transform.position.x) + Mathf.Abs(transform.position.z - NPC.transform.position.z) < nearbyDistance)
			{
				currentNearbyNPCs.Add(NPC);
			}
		}
		nearbyNPCs = currentNearbyNPCs;
	}

	private IEnumerator ContinuallyUpdateNearbyNPCs()
	{
		while(alive)
		{
			UpdateNearbyNPCs();
			yield return new WaitForSeconds(Random.Range(0.5f, 1f));
		}
		nearbyNPCs.Clear();
	}

	private IEnumerator ContinuallyUpdateVisibleNPCs()
	{
		while(alive)
		{
			List<NPCBehavior> currentlyVisibleNPCs = new List<NPCBehavior>();
			foreach(NPCBehavior NPC in allNPCs)
			{
				if(ObjectIsVisible(NPC.gameObject))
				{
					currentlyVisibleNPCs.Add(NPC);
				}
			}
			visibleNPCs = currentlyVisibleNPCs;
			yield return new WaitForSeconds(0.1f);
		}
		visibleNPCs.Clear();
	}
	
	private void Start()
	{
		allNPCs.Add(this);
		if(!player)
		{
			player = GameObject.FindWithTag("Player");
		}
		StartCoroutine(ContinuallyUpdateNearbyNPCs());
		StartCoroutine(ContinuallyUpdateVisibleNPCs());
	}
}
