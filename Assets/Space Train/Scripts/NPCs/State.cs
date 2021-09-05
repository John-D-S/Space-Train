using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NpcAi
{
    public abstract class State
    {
        public abstract State UpdateState(ref NpcStateMachine _stateMachine);
        public void UpdateSuspicionOfPlayer(ref NpcStateMachine _stateMachine)
        {
            if(!_stateMachine.isAlerted)
            {
                foreach(NpcStateMachine npc in _stateMachine.VisibleNPCs)
                {
                    if(npc.isAlerted)
                    {
                        _stateMachine.emote.ShowEmote(EmoteType.Exclaimation);
                        _stateMachine.isAlerted = true;
                    }
                }
                if(_stateMachine.PlayerIsVisible)
                {
                    _stateMachine.suspicionOfPlayer = Mathf.Clamp01(_stateMachine.suspicionOfPlayer + Time.fixedDeltaTime / _stateMachine.TimeUntilAlert);
                    if(_stateMachine.suspicionOfPlayer >= 1)
                    {
                        _stateMachine.isAlerted = true;
                        foreach(NpcStateMachine nearbyNpC in _stateMachine.VisibleNPCs)
                        {
                            if(nearbyNpC != null && nearbyNpC.isAlerted)
                            {
                                _stateMachine.isAlerted = true;
                            }
                        }
                        if(_stateMachine.isAlerted)
                        {
                            _stateMachine.emote.ShowEmote(EmoteType.Exclaimation);
                        }
                        return;
                    }
                }
                else
                {
                    _stateMachine.suspicionOfPlayer = Mathf.Clamp01(_stateMachine.suspicionOfPlayer - Time.fixedDeltaTime / _stateMachine.TimeUntilAlert);
                }
                if(_stateMachine.suspicionOfPlayer > 0)
                {
                    _stateMachine.emote.ShowEmote(EmoteType.Question, 0.1f);
                }
                    
                    
                    //the ai suspiscioin system was greatly simplified due to time
                    /*
                    // if the NPC is fully suspicious of the player, make them alert.
                    if(_stateMachine.suspicionOfPlayer > 1)
                    {
                        _stateMachine.emote.ShowEmote(EmoteType.Exclaimation, 2.5f);
                        _stateMachine.isAlerted = true;
                        return;
                    }
                    if(NpcStateMachine.playerIdentity.recentlyChangedIdentities)
                    {
                        _stateMachine.emote.ShowEmote(EmoteType.Exclaimation, 2.5f);
                        _stateMachine.isAlerted = true;
                        return;
                    }
                    if(!NpcStateMachine.playerIdentity.IsAllowedInLocation)
                    {
                        _stateMachine.suspicionOfPlayer += Time.fixedDeltaTime / _stateMachine.TimeUntilAlert;
                    }
                    else
                    {
                        _stateMachine.suspicionOfPlayer -= Time.fixedDeltaTime / _stateMachine.TimeUntilAlert;
                    }
                    // if the npc is more than half suspicious of the player, show the question mark emote.
                    if(_stateMachine.suspicionOfPlayer > 0.5f)
                    {
                        _stateMachine.emote.ShowEmote(EmoteType.Question);
                    }
                    */
            }
        } 
    }
}