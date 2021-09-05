using SpaceTrain.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NpcAi
{
	public class GuardIdle : State
	{
		private float timeSinceLastMoved = 0;
		private float timeUntilNextMovement = 1;
		public GuardIdle(ref NpcStateMachine _stateMachine)
		{
			timeUntilNextMovement = Random.Range(_stateMachine.AverageTimeBetweenTalks * 0.5f, _stateMachine.AverageTimeBetweenTalks * 0.5f + _stateMachine.AverageTimeBetweenTalks); 
		}
        
		private void UpdateTimes()
		{
			timeSinceLastMoved += Time.fixedDeltaTime;
		}
        
		public override State UpdateState(ref NpcStateMachine _stateMachine)
		{
			if(_stateMachine.isAlerted)
			{
				return new GuardAlert();
			}
			UpdateTimes();
			if(timeSinceLastMoved > timeUntilNextMovement)
			{
				return new GuardPatrol();
			}
			return this;
		}
        
		public class GuardPatrol : State
		{
			private AIDestination currentDestination;
			
			public override State UpdateState(ref NpcStateMachine _stateMachine)
			{
				if(_stateMachine.isAlerted || _stateMachine.PlayerIsVisible)
				{
					return new GuardAlert();
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
						return new GuardIdle(ref _stateMachine);
					}
				}
				return this;
			}
		}
		
		public class GuardAlert : State
		{
			private AIDestination currentDestination;
			private Vector3 lastSeenPlayerPosition;
			
			public override State UpdateState(ref NpcStateMachine _stateMachine)
			{
				Vector3 playerPosition = NpcStateMachine.playerIdentity.transform.position;
				if(_stateMachine.PlayerIsVisible)
				{
					_stateMachine.emote.ShowEmote(EmoteType.Exclaimation);
					_stateMachine.agentController.TryRunToPosition(playerPosition);
					if(Vector3.Distance(_stateMachine.transform.position, playerPosition) < 1)
					{
						AIDestination.aiDestinationsByAllowedCharacters.Clear();
						SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
					}
				}
				else
				{
					_stateMachine.emote.ShowEmote(EmoteType.Question);
					_stateMachine.agentController.TryWalkToPosition(playerPosition);
				}
				return this;
				if(_stateMachine.PlayerIsVisible)
				{
					_stateMachine.agentController.TryRunToPosition(NpcStateMachine.playerIdentity.transform.position);
				}
				_stateMachine.emote.ShowEmote(EmoteType.Question);
				return new GuardPatrol();
			}
		}
	}
}