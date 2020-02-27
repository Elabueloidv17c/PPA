using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateMachine<T> 
{
    private IState<T> m_previus;
    private IState<T> m_current;
    private T m_entity;

   void Init(T entity, IState<T> initState) {

        m_entity = entity;
        m_current = initState;
        m_previus = initState;
    }   
}
