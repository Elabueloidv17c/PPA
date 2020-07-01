using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// esta clase es la que realiza toda la logica de la maquina de estados 
/// </summary>
/// <typeparam name="T"></typeparam>

public class StateMachine<T> 
{
    private IState<T> m_previus;
    private IState<T> m_current;
    private T m_entity;
    /// <summary>
    /// en esta clase se inicializa todas la variables que necesita para que funcione 
    /// la maquina de estados
    /// </summary>
    /// entidad se refiere a cualquier entidad que vaya utilizar 
    /// la maquina de estados
    /// <param name="entity"></param>
    /// esta variable es el estado con el cual va inicializar
    /// <param name="initState"></param>
    public void Init(T entity, IState<T> initState) {

        m_entity = entity;
        m_current = initState;
        m_previus = initState;
    }
    /// <summary>
    /// aqui se desarrolla la logica del estado cuando esta encendido 
    /// 
    /// </summary>
    public void onState() {

        m_current.onStateUpdate();
        m_current.onComputeNextState();
    }
    /// <summary>
    /// en esta parte la maquina cambia de estado y hace una transicion 
    /// </summary>
    /// <param name="nextState"></param>
    public void toState(IState<T> nextState) {
        m_current.onExit();
        m_previus = m_current;
        m_current = nextState;
        m_current.onEnter();
    }
}
