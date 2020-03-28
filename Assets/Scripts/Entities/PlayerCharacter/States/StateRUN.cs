using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRUN : IState<liPlayerCharacter>
{
    public StateRUN(StateMachine<liPlayerCharacter> stateMachine, liPlayerCharacter entity) : base(stateMachine, entity)
    {

    }

    public override void onComputeNextState()
    {
        throw new System.NotImplementedException();
    }

    public override void onStateUpdate()
    {
        throw new System.NotImplementedException();
    }
}

