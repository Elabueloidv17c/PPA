using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IState<T>
{
    private StateMachine<T> m_pStateMachine;
    private T m_pEntity;
    public IState(StateMachine<T> stateMachine,T entity)
    {
        m_pEntity = entity;
        m_pStateMachine = stateMachine;
    }
   public virtual void onEnter() {

    }
    public virtual void onExit() {

    }
    public abstract void onStateUpdate();
    public abstract void onComputeNextState();
}
