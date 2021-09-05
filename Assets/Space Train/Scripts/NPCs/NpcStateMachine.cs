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
		[SerializeField] private float maxSuspicion;
		[SerializeField, Tooltip("The Npc will only be aware of other npcs and the player if they are within this distance.")] private float nearbyDistance = 16;
		public float NearbyDistance => nearbyDistance;
		[SerializeField] private CharacterIdentity npcIdentity = CharacterIdentity.Passenger;
		public CharacterIdentity NpcIdentity => npcIdentity;
		[SerializeField] private float averageTimeBetweenTalks;
		public float AverageTimeBetweenTalks => averageTimeBetweenTalks;
		[SerializeField] private float timeBetweenWalks;
		public float TimeBetweenWalks => timeBetweenWalks;
		
		[Header("-- Ai Sight Settings --")]
		[SerializeField] private float fieldOfView;
		[SerializeField] private float eyeHeight;

		[System.NonSerialized] public float suspicionOfPlayer;
		[SerializeField] private float timeUntilAlert;
		public float TimeUntilAlert => timeUntilAlert;
		public static IdentityHandler playerIdentity;
		private bool alive = true;
		public bool Alive => alive;
		[System.NonSerialized] public bool isAlerted;
		private State currentState;
		public State CurrentState => currentState;

		public bool PlayerIsVisible
		{
			get
			{
				bool playerVisible = ObjectVisibleInFov(playerIdentity.gameObject);
				return playerVisible;
			}
		}
		
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

		public bool ObjectIsVisibleFromPos(GameObject _gameObject)
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
			return false;
		}

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

		private IEnumerator ContinuallyUpdateNearbyNpCs()
		{
			while(alive)
			{
				UpdateNearbyNpCs();
				yield return new WaitForSeconds(0.2f);
			}
			nearbyNPCs.Clear();
		}

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