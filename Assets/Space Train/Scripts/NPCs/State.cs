using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NpcAi
{
    public abstract class State
    {
        public abstract State Function(ref NPCStateMachine _stateMachine);
    }
    
    public class GuardWander : State
    {
        public override State Function(ref NPCStateMachine _stateMachine)
        {
            return new GuardWander();
        }
    }
    
    public class WorkerWander : State
    {
        public override State Function(ref NPCStateMachine _stateMachine)
        {
            return new WorkerWander();
        }
    }

    public class PassengerWander : State
    {
        public override State Function(ref NPCStateMachine _stateMachine)
        {
            return new PassengerWander();
        }
    }
}