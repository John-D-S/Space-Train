﻿using SpaceTrain.Player;

using System.Collections.Generic;

using UnityEngine;

namespace NpcAi
{
	public class PassengerIdle : State
	{
		private float timeSinceLastTalked = 0;
		private float timeUntilNextTalk = 1;
		private float timeSinceLastMoved = 0;
		private float timeUntilNextMovement = 1;
		public PassengerIdle(ref NpcStateMachine _stateMachine)
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
				return new PassengerAlert();
			}
			UpdateTimes();
			if(timeSinceLastMoved > timeUntilNextMovement)
			{
				return new PassengerWalk();
			}
			if(timeSinceLastTalked > timeUntilNextTalk)
			{
				NpcStateMachine closestNpc = null;
				float closestNpcDistance = Mathf.Infinity;
				foreach(NpcStateMachine npc in _stateMachine.VisibleNPCs)
				{
					//passengers only talk to other passengers.
					if(npc.NpcIdentity == CharacterIdentity.Passenger)
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
        
		public class PassengerWalk : State
		{
			private AIDestination currentDestination;
            
			public override State UpdateState(ref NpcStateMachine _stateMachine)
			{
				Debug.Log("walk");
				if(_stateMachine.isAlerted)
				{
					return new PassengerAlert();
				}
				
				if(!currentDestination)
				{
					List<AIDestination> availableDestinations = AIDestination.aiDestinationsByAllowedCharacters[_stateMachine.NpcIdentity];
					AIDestination attemptedDestination = availableDestinations[Random.Range(0, availableDestinations.Count)];
					if(_stateMachine.agentController.TryWalkToPosition(attemptedDestination.transform.position))
					{
						currentDestination = attemptedDestination;
					}
				}
				else
				{
					if(_stateMachine.agentController.HasArrived)
					{
						return new PassengerIdle(ref _stateMachine);
					}
				}
				return this;
			}
		}
		
		public class PassengerAlert : State
		{
			private AIDestination currentDestination;
            
			public override State UpdateState(ref NpcStateMachine _stateMachine)
			{
				Debug.Log("alert");
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
						return new PassengerHide();
					}
				}
				return this;
			}
		}
		
		public class PassengerHide : State
		{
			public override State UpdateState(ref NpcStateMachine _stateMachine)
			{
				Debug.Log("hide");
				if(_stateMachine.PlayerIsVisible)
				{
					return new PassengerAlert();
				}

				return this;
			}
		}
	}
}