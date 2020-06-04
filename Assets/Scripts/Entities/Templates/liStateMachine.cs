using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateMachine<T> 
{
    private IState<T> m_previus;
    private IState<T> m_current;
    private T m_entity;

    public void Init(T entity, IState<T> initState) {

        m_entity = entity;
        m_current = initState;
        m_previus = initState;
    }
    public void onState() {

        m_current.onStateUpdate();
        m_current.onComputeNextState();
    }
    public void toState(IState<T> nextState) {
        m_current.onExit();
        m_previus = m_current;
        m_current = nextState;
        m_current.onEnter();
    }
}
