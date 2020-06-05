using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateWalk : IState<liPlayerCharacter>
{
    liPlayerCharacter m_entity;
   /*
       float verticalRatio = 1.0f;
    public float m_runSpeed = 2.8f;
    public float m_walkSpeed = 1.8f;
    */

    public StateWalk(StateMachine<liPlayerCharacter> stateMachine, liPlayerCharacter entity) : base(stateMachine, entity)
    {

        // GameObject player = GameObject.FindGameObjectWithTag("Player");

        m_entity = entity;
            //player.GetComponent<liPlayerCharacter>();
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

            m_entity.animator.SetFloat("X", delta.x);
            m_entity.animator.SetFloat("Y", delta.y);
            m_entity.animator.SetFloat("Speed", 0.5f);

              delta.y *= m_entity.verticalRatio;

              m_entity.body.MovePosition(m_entity.body.position + (delta * Time.deltaTime *
                             ((Input.GetKey((KeyCode)GameInput.Sprint)) ?
                             m_entity.m_runSpeed : m_entity.m_walkSpeed)));

                   
        }

        else
        {
            m_entity.animator.SetFloat("Speed", 0f);
        }
    }
    
   }
