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
        
    }

    public override void onStateUpdate()
    {
        
    }
}
