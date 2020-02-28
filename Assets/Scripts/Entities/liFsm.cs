using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Ifsm<T> 
{
    private Istate<T> m_previus;
    private Istate<T> m_current;
    private T m_entity;

   void Init(T entity, Istate<T> initState) {

        m_entity = entity;
        m_current = initState;
        m_previus = initState;
    }   
}
