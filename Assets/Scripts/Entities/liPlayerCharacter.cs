using UnityEngine;

public class liPlayerCharacter : MonoBehaviour
{
    Rigidbody2D m_body;
    Vector2 m_movement;
    Animator m_animator;
    public float m_verticalRatio = 0.75f;
    public float m_walkSpeed = 1.8f;
    public float m_runSpeed = 2.8f;
    

    void Start() {
        m_body = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }

    private void Update() {
        
        if (liGameManager.instance && 
            !liGameManager.instance.menuActive) 
        {
            if(Input.GetKey(KeyCode.I))
            {
                liInventory.instance.OpenUI();
            }

            Move();
        }
    }

    void Move()
    {
        Vector2 delta = Vector2.zero;
        
        if (Input.GetKey((KeyCode)GameInput.MoveUp))
        {
            delta.y += 1;
        }
        if (Input.GetKey((KeyCode)GameInput.MoveDown))
        {
            delta.y -= 1;
        }
        if (Input.GetKey((KeyCode)GameInput.MoveLeft))
        {
            delta.x -= 1;
        }
        if (Input.GetKey((KeyCode)GameInput.MoveRight))
        {
            delta.x += 1;
        }

        if (delta != Vector2.zero) {
            delta.Normalize();
            delta.y *= m_verticalRatio;

            m_animator.SetFloat("X", delta.x);
            m_animator.SetFloat("Y", delta.y);

            m_body.MovePosition(m_body.position + (delta * Time.deltaTime *
                                ((Input.GetKey((KeyCode)GameInput.Sprint)) ?
                                m_runSpeed : m_walkSpeed)));
        }
       
        
    }
}
