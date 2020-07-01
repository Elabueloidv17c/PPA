using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTalk : IState<liPlayerCharacter>
{
    public StateTalk(StateMachine<liPlayerCharacter> stateMachine, liPlayerCharacter entity) : base(stateMachine, entity)
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
