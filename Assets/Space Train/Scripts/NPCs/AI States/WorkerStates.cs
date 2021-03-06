using SpaceTrain.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NpcAi
{
	/// <summary>
	/// The Worker will stand for a random amount of time, occasionally talking to nearby Workers and will start moving.
	/// If the Worker becomes alert, it will switch to the PassengerAlert state
	/// </summary>
	public class WorkerIdle : State
	{
		private float timeSinceLastTalked = 0;
		private float timeUntilNextTalk = 1;
		private float timeSinceLastMoved = 0;
		private float timeUntilNextMovement = 1;
		public WorkerIdle(ref NpcStateMachine _stateMachine)
		{
			timeUntilNextTalk = Random.Range(_stateMachine.AverageTimeBetweenTalks * 0.5f, _stateMachine.AverageTimeBetweenTalks * 0.5f + _stateMachine.AverageTimeBetweenTalks);
			timeUntilNextMovement = Random.Range(_stateMachine.AverageTimeBetweenTalks * 0.5f, _stateMachine.AverageTimeBetweenTalks * 0.5f + _stateMachine.AverageTimeBetweenTalks); 
		}
        
		private void UpdateTimes()
		{
			timeSinceLastMoved += Time.fixedDeltaTime;
			timeSinceLastTalked += Time.fixedDeltaTime;
		}
		
		private void Talk(ref NpcStateMachine _stateMachine)
		{
			if(Random.Range(0,2) == 1)
			{
				_stateMachine.emote.ShowEmote(EmoteType.Talk, 3f);
			}
			else
			{
				_stateMachine.emote.ShowEmote(EmoteType.Listen, 3f);
			}
		}
        
		public override State UpdateState(ref NpcStateMachine _stateMachine)
		{
			if(_stateMachine.isAlerted)
			{
				return new WorkerAlert();
			}
			UpdateTimes();
			if(timeSinceLastMoved > timeUntilNextMovement)
			{
				return new WorkerWalk();
			}
			if(timeSinceLastTalked > timeUntilNextTalk)
			{
				NpcStateMachine closestNpc = null;
				float closestNpcDistance = Mathf.Infinity;
				foreach(NpcStateMachine npc in _stateMachine.VisibleNPCs)
				{
					//passengers only talk to other passengers.
					if(npc.NpcIdentity == CharacterIdentity.Worker)
					{
						float distanceToNpc = Vector3.Distance(npc.transform.position, _stateMachine.transform.position);
						if(distanceToNpc < closestNpcDistance)
						{
							closestNpcDistance = distanceToNpc;
							closestNpc = npc;
						}
					}
				}
				if(closestNpc)
				{
					_stateMachine.agentController.TurnToFace(closestNpc.transform.position, 1.5f);
					Talk(ref _stateMachine);
				}
			}
			return this;
		}
	}
	
	/// <summary>
	/// The worker will stand for a random amount of time
	/// If the passenger becomes alert, it will switch to the PassengerAlert state
	/// </summary>
	public class WorkerWalk : State
	{
		private AIDestination currentDestination;
        
		public override State UpdateState(ref NpcStateMachine _stateMachine)
		{
			if(_stateMachine.isAlerted)
			{
				return new WorkerAlert();
			}
			
			if(!currentDestination)
			{
				List<AIDestination> availableDestinations = AIDestination.aiDestinationsByAllowedCharacters[_stateMachine.NpcIdentity];
				AIDestination attemptedDestination = availableDestinations[Random.Range(0, availableDestinations.Count)];
				if(attemptedDestination != null && _stateMachine.agentController.TryWalkToPosition(attemptedDestination.transform.position))
				{
					currentDestination = attemptedDestination;
				}
			}
			else
			{
				if(_stateMachine.agentController.HasArrived)
				{
					return new WorkerIdle(ref _stateMachine);
				}
			}
			return this;
		}
	}
	
	/// <summary>
	/// the Worker will run to a random destination and switch to the hiding state when it arrives
	/// </summary>
	public class WorkerAlert : State
	{
		private AIDestination currentDestination;
        
		public override State UpdateState(ref NpcStateMachine _stateMachine)
		{
			if(!currentDestination)
			{
				List<AIDestination> availableDestinations = AIDestination.aiDestinationsByAllowedCharacters[_stateMachine.NpcIdentity];
				AIDestination attemptedDestination = availableDestinations[Random.Range(0, availableDestinations.Count)];
				if(_stateMachine.agentController.TryRunToPosition(attemptedDestination.transform.position))
				{
					currentDestination = attemptedDestination;
				}
			}
			else
			{
				if(_stateMachine.agentController.HasArrived)
				{
					return new WorkerHide();
				}
			}
			return this;
		}
	}
	
	/// <summary>
	/// the worker will stand still in the hiding spot until it can see the player, at which point it will run away to a new hiding spot.
	/// </summary>
	public class WorkerHide : State
	{
		public override State UpdateState(ref NpcStateMachine _stateMachine)
		{
			if(_stateMachine.PlayerIsVisible)
			{
				return new WorkerAlert();
			}
			return this;
		}
	}
}
