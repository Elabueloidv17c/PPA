using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : IState<liPlayerCharacter>
{
    public StateIdle(StateMachine<liPlayerCharacter> stateMachine, liPlayerCharacter entity) : base(stateMachine, entity)
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
