using SpaceTrain.Player;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NpcAi
{
    public abstract class State
    {
        public abstract State UpdateState(ref NpcStateMachine _stateMachine);
        private void UpdateSuspicionOfPlayer(ref NpcStateMachine _stateMachine)
        {
            foreach(NpcStateMachine npc in _stateMachine.VisibleNPCs)
            {
                if(npc.isAlerted)
                {
                    _stateMachine.isAlerted = true;
                }
            }
            if(_stateMachine.PlayerIsVisible)
            {
                if(NpcStateMachine.playerIdentity.recentlyChangedIdentities)
                {
                    _stateMachine.isAlerted = true;
                }
            }
        } 
    }
    
    public class GuardIdle : State
    {
        public override State UpdateState(ref NpcStateMachine _stateMachine)
        {
            return this;
        }
    }
    
    public class WorkerIdle : State
    {
        public override State UpdateState(ref NpcStateMachine _stateMachine)
        {
            return this;
        }
    }

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
                //needs work
                return this;
            }
        }
    }
}