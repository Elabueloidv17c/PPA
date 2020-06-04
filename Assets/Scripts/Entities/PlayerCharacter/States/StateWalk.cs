using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateWalk : IState<liPlayerCharacter>
{
    liPlayerCharacter m_entity;
   
       float verticalRatio = 1.0f;
    public float m_runSpeed = 2.8f;
    public float m_walkSpeed = 1.8f;
    public StateWalk(StateMachine<liPlayerCharacter> stateMachine, liPlayerCharacter entity) : base(stateMachine, entity)
    {
        m_entity = entity;
    }

    public override void onComputeNextState()
    {
      
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
        //anima = m_entity.GetComponent<Animator>();

        //anima.SetFloat("Y", -1);
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
            
          //anima.SetFloat("X", delta.x);
            //anima.SetFloat("Y", delta.y);
            //anima.SetFloat("Speed", 0.5f);

            delta.y *= verticalRatio;

            m_entity.body.MovePosition(m_entity.body.position + (delta * Time.deltaTime *
                           ((Input.GetKey((KeyCode)GameInput.Sprint)) ?
                           m_runSpeed : m_walkSpeed)));
            

        }
        else
        {
            //anima.SetFloat("Speed", 0f);
        }
    }
   }
