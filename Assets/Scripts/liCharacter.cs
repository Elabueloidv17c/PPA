using UnityEngine;

public class liCharacter : MonoBehaviour
{
    Rigidbody2D m_body;
    Vector2 m_movement;
    public Animator m_animator;
    public float m_verticalRatio;
    public float m_walkSpeed;
    public float m_runSpeed;

    public bool m_isInteracting;

    void Start() {
        m_body = GetComponent<Rigidbody2D>();
        m_verticalRatio = 0.75f;
        m_runSpeed = 2.8f;
        m_walkSpeed = 1.8f;
        
    }

    private void Update() {
        
        if (!m_isInteracting) 
        {
            Move();
        }
    }

    void Move()
    {
        Vector2 delta = Vector2.zero;
        
        if (Input.GetKey((KeyCode)GameInput.MoveUp))
        {
            m_animator.SetBool("Mover_atras", true);
            delta.y += m_verticalRatio;
        }
        if (Input.GetKey((KeyCode)GameInput.MoveDown))
        {
            delta.y -= m_verticalRatio;
        }
        if (Input.GetKey((KeyCode)GameInput.MoveLeft))
        {
            delta.x -= 1;
            m_animator.SetBool("Mover_izquierda", true);
        }
        if (Input.GetKey((KeyCode)GameInput.MoveRight))
        {
            delta.x += 1;
            m_animator.SetBool("Mover_derecha",true);
        }
        if (!Input.GetKey((KeyCode)GameInput.MoveLeft) && !Input.GetKey((KeyCode)GameInput.MoveRight)) {
            m_animator.SetBool("Mover_izquierda", false);
            m_animator.SetBool("Mover_derecha", false);
        }

        m_body.MovePosition(m_body.position + (delta * Time.deltaTime *
                           ((Input.GetKey((KeyCode)GameInput.Sprint)) ?
                           m_runSpeed : m_walkSpeed)));
    }
}
