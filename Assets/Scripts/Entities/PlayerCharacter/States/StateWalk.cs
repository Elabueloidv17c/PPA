using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// es la clase del estado en movimiento en esta 
/// se realiza todo lo que tiene que ver con movimiento del personaje
/// </summary>
public class StateWalk : IState<liPlayerCharacter>
{
   /*
       float verticalRatio = 1.0f;
    public float m_runSpeed = 2.8f;
    public float m_walkSpeed = 1.8f;
    */

    public StateWalk(StateMachine<liPlayerCharacter> stateMachine, liPlayerCharacter entity) : base(stateMachine, entity)
    {

    }

    public override void onComputeNextState()
    {

      //
    }

    public override void onStateUpdate()
    {
        
        if (!liGameManager.instance.menuActive)
        {
            Move();
        }
    }
    /// <summary>
    /// se realiza la logica del movimiento
    /// </summary>
    void Move()
    {
       
        Vector2 delta = Vector2.zero;

        if (Input.GetKey((KeyCode)GameInput.MoveUp))
        {
            delta.y += 1;
            Debug.Log("presionando arriba");
        }
        if (Input.GetKey((KeyCode)GameInput.MoveDown))
        {
            delta.y -= 1;
            Debug.Log("presionando abajo");
        }
        if (Input.GetKey((KeyCode)GameInput.MoveLeft))
        {
            delta.x -= 1;
            Debug.Log("presionando izquierda");
        }
        if (Input.GetKey((KeyCode)GameInput.MoveRight))
        {
            delta.x += 1;
            Debug.Log("presionando derecha");
        }
        if (delta != Vector2.zero)
        {
            delta.Normalize();

            m_pEntity.animSetFloats("X", delta.x);
            m_pEntity.animSetFloats("Y", delta.y);
            m_pEntity.animSetFloats("Speed", 0.5f);

              delta.y *= m_pEntity.verticalRatio;

              m_pEntity.body.MovePosition(m_pEntity.body.position + (delta * Time.deltaTime *
                             ((Input.GetKey((KeyCode)GameInput.Sprint)) ?
                             m_pEntity.m_runSpeed : m_pEntity.m_walkSpeed)));

                   
        }

        else
        {
            m_pEntity.animSetFloats("Speed", 0f);
        }
    }
    
   }
