using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
using SpaceTrain.Player;

using System.Security.Cryptography.X509Certificates;

namespace NpcAi
{	
	public class NpcStateMachine : MonoBehaviour
	{
		private static List<NpcStateMachine> allNPCs = new List<NpcStateMachine>();
		private List<NpcStateMachine> nearbyNPCs = new List<NpcStateMachine>();
		public List<NpcStateMachine> NearbyNPCs => nearbyNPCs;
		private List<NpcStateMachine> visibleNPCs = new List<NpcStateMachine>();
		public List<NpcStateMachine> VisibleNPCs => visibleNPCs;
		
		public Emote emote;
		public AgentController agentController;
		[SerializeField, Tooltip("The Npc will only be aware of other npcs and the player if they are within this distance.")] private float nearbyDistance = 16;
		public float NearbyDistance => nearbyDistance;
		[SerializeField, Tooltip("The Identity of the NPC. This determines how it will behave")] private CharacterIdentity npcIdentity = CharacterIdentity.Passenger;
		public CharacterIdentity NpcIdentity => npcIdentity;
		[SerializeField, Tooltip("How often NPC talks")] private float averageTimeBetweenTalks;
		public float AverageTimeBetweenTalks => averageTimeBetweenTalks;
		[SerializeField, Tooltip("How long between walking does the npc stand still at their destination.")] private float timeBetweenWalks;
		public float TimeBetweenWalks => timeBetweenWalks;
		
		[Header("-- Ai Sight Settings --")]
		[SerializeField, Tooltip("determines what the npc can see infront of them.")] private float fieldOfView;
		[SerializeField, Tooltip("how high the eye is from the origin of the NPC")] private float eyeHeight;

		[System.NonSerialized] public float suspicionOfPlayer;
		[SerializeField] private float timeUntilAlert;
		public float TimeUntilAlert => timeUntilAlert;
		public static IdentityHandler playerIdentity;
		private bool alive = true;
		public bool Alive => alive;
		[System.NonSerialized] public bool isAlerted;
		private State currentState;
		public State CurrentState => currentState;

		/// <summary>
		/// whether the player is visible from the NPC's current position
		/// </summary>
		public bool PlayerIsVisible
		{
			get
			{
				bool playerVisible = ObjectVisibleInFov(playerIdentity.gameObject);
				return playerVisible;
			}
		}
		
		/// <summary>
		/// whether the given object is visible from the NPC's current position, and is within their field of view
		/// </summary>
		public bool ObjectVisibleInFov(GameObject _gameObject)
		{
			if(_gameObject)
			{
				if(Vector3.Angle(transform.forward, playerIdentity.transform.position - transform.position) < .5f * fieldOfView)
				{
					return ObjectIsVisibleFromPos(_gameObject);
				}
			}
			return false;
		}

		/// <summary>
		/// whether the given gameObject is visible from the NPC's current position
		/// </summary>
		public bool ObjectIsVisibleFromPos(GameObject _gameObject)
		{
			if(Vector3.Distance(transform.position, _gameObject.transform.position) < nearbyDistance)
			{
				RaycastHit hit = new RaycastHit();
				Physics.Raycast(transform.position + Vector3.up * eyeHeight,_gameObject.transform.position - transform.position, out hit);
				if(hit.collider)
				{
					if(hit.collider.gameObject == _gameObject)
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// updates all the Npcs within nearbyDistance from this NPC
		/// </summary>
		private void UpdateNearbyNpCs()
		{
			List<NpcStateMachine> currentNearbyNPCs = new List<NpcStateMachine>();
			foreach(NpcStateMachine npc in allNPCs)
			{
				//just using taxicab distance to find nearby npcs for performance
				if(npc)
				{
					if(Mathf.Abs(transform.position.x - npc.transform.position.x) + Mathf.Abs(transform.position.z - npc.transform.position.z) < nearbyDistance)
					{
						currentNearbyNPCs.Add(npc);
					}
				}
			}
			nearbyNPCs = currentNearbyNPCs;
		}

		/// <summary>
		/// continually calles UpdateNearbyNpcs after a bit, instead of every frame
		/// </summary>
		private IEnumerator ContinuallyUpdateNearbyNpCs()
		{
			while(alive)
			{
				UpdateNearbyNpCs();
				yield return new WaitForSeconds(0.2f);
			}
			nearbyNPCs.Clear();
		}

		/// <summary>
		/// out of nearbyNPCs, update currentlyVisibleNpcs to include the ones that are visible at this point in time.
		/// </summary>
		/// <returns></returns>
		private IEnumerator ContinuallyUpdateVisibleNpCs()
		{
			while(alive)
			{
				List<NpcStateMachine> currentlyVisibleNpCs = new List<NpcStateMachine>();
				foreach(NpcStateMachine npc in allNPCs)
				{
					if(npc)
					{
						if(ObjectVisibleInFov(npc.gameObject))
						{
							currentlyVisibleNpCs.Add(npc);
						}
					}
				}
				visibleNPCs = currentlyVisibleNpCs;
				yield return new WaitForSeconds(0.1f);
			}
			visibleNPCs.Clear();
		}
		
		private void Start()
		{
			allNPCs.Add(this);
			if(!playerIdentity)
			{
				playerIdentity = GameObject.FindWithTag("Player").GetComponent<IdentityHandler>();
			}
			//start the coroutines that continually update 
			StartCoroutine(ContinuallyUpdateNearbyNpCs());
			StartCoroutine(ContinuallyUpdateVisibleNpCs());
			NpcStateMachine thisNpc = this;
			switch(npcIdentity)
			{
				case CharacterIdentity.None:
					currentState = new PassengerIdle(ref thisNpc);
					break;
				case CharacterIdentity.Passenger:
					currentState = new PassengerIdle(ref thisNpc);
					break;
				case CharacterIdentity.Worker:
					currentState = new WorkerIdle(ref thisNpc);
					break;
				case CharacterIdentity.Guard:
					currentState = new GuardIdle(ref thisNpc);
					break;
			}
		}

		private void FixedUpdate()
		{
			NpcStateMachine thisNpc = this;
			currentState = currentState.UpdateState(ref thisNpc);
			currentState.UpdateSuspicionOfPlayer(ref thisNpc);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = new Color(0, 1, 0, 0.25f);
			foreach(NpcStateMachine nearbyNpC in nearbyNPCs)
			{
				Gizmos.DrawSphere(nearbyNpC.transform.position, 0.25f);
			}
			Gizmos.color = new Color(1, 0, 0, 0.25f);
			foreach(NpcStateMachine visibleNpC in visibleNPCs)
			{
				Gizmos.DrawSphere(visibleNpC.transform.position, 0.3f);
			}
		}
	}
}