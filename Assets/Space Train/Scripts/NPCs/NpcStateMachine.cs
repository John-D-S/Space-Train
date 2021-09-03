using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
using SpaceTrain.Player;

namespace NpcAi
{	
	public abstract class NpcStateMachine : MonoBehaviour
	{
		private static List<NpcStateMachine> allNPCs;
		private List<NpcStateMachine> nearbyNPCs;
		public List<NpcStateMachine> NearbyNPCs => nearbyNPCs;
		private List<NpcStateMachine> visibleNPCs;
		public List<NpcStateMachine> VisibleNPCs => visibleNPCs;
		
		public Emote emote;
		public AgentController agentController;
		[SerializeField] private float maxSuspicion;
		[SerializeField, Tooltip("The Npc will only be aware of other npcs and the player if they are within this distance.")] private float nearbyDistance = 16;
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
			get => ObjectIsVisible(playerIdentity.gameObject);
		}
		
		public bool ObjectIsVisible(GameObject _gameObject)
		{
			if(_gameObject)
			{
				if(Vector3.Angle(transform.forward, playerIdentity.transform.position) < .5f * fieldOfView)
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

		private void UpdateNearbyNpCs()
		{
			List<NpcStateMachine> currentNearbyNPCs = new List<NpcStateMachine>();
			foreach(NpcStateMachine npc in allNPCs)
			{
				//just using taxicab distance to find nearby npcs for performance
				if(Mathf.Abs(transform.position.x - npc.transform.position.x) + Mathf.Abs(transform.position.z - npc.transform.position.z) < nearbyDistance)
				{
					currentNearbyNPCs.Add(npc);
				}
			}
			nearbyNPCs = currentNearbyNPCs;
		}

		private IEnumerator ContinuallyUpdateNearbyNpCs()
		{
			while(alive)
			{
				UpdateNearbyNpCs();
				yield return new WaitForSeconds(Random.Range(0.5f, 1f));
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
					if(ObjectIsVisible(npc.gameObject))
					{
						currentlyVisibleNpCs.Add(npc);
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
					currentState = new WorkerIdle();
					break;
				case CharacterIdentity.Guard:
					currentState = new GuardIdle();
					break;
			}
		}

		private void FixedUpdate()
		{
			NpcStateMachine thisNpc = this;
			currentState = currentState.UpdateState(ref thisNpc);
		}
	}
}