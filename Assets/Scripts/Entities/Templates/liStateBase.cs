using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// este es la interfaz que utilizamos para realizar los estados
/// en esta clase tenemos las funciones que realizan 
/// y muchas cosas mas
/// </summary>
/// los estados necesitan tener una referencia a la maquina de estados 
/// y tambien a la entidad que se modifica 
/// <typeparam name="T"></typeparam>
public abstract class IState<T>
{
    private StateMachine<T> m_pStateMachine;
    private T m_pEntity;
    //aqui se inicializa las variables
    public IState(StateMachine<T> stateMachine,T entity)
    {
        m_pEntity = entity;
        m_pStateMachine = stateMachine;
    }
    //este es para que las variables que entran si necesitan modificarse se modifique
   public virtual void onEnter() {
        
    }
    //este es para que las variables que salen y necesitan ser cambiadas
    public virtual void onExit() {

    }
    //logica del juego movimientos etc
    public abstract void onStateUpdate();

    /// <summary>
    /// aqui ejecuta el estado que sigue
    /// </summary>
    public abstract void onComputeNextState();
}
