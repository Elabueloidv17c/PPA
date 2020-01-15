using UnityEngine;

public class liCharacter : MonoBehaviour
{
    Rigidbody2D m_body;
    Vector2 m_movement;

    public float m_verticalRatio;
    public float m_walkSpeed;
    public float m_runSpeed;

    public bool m_canInteract;
    public bool m_isInteracting;

    void Start() {
        m_body = GetComponent<Rigidbody2D>();
        m_verticalRatio = 0.75f;
        m_runSpeed = 2.8f;
        m_walkSpeed = 1.8f;
        m_isInteracting = false;
        m_canInteract = false;
    }

    private void Update() {

        if(m_canInteract && Input.GetKeyDown((KeyCode)GameInput.Interact)) {
            m_isInteracting = true;
        }
        if(!m_isInteracting && Input.GetKeyDown((KeyCode)GameInput.Exit)) {
            m_isInteracting = false;
        }
        if (!m_isInteracting) {
            Move();
        }
    }

    void Move()
    {
        Vector2 delta = new Vector2();
        if (Input.GetKey((KeyCode)GameInput.MoveUp))
        {
            delta.y += m_verticalRatio;
        }
        if (Input.GetKey((KeyCode)GameInput.MoveDown))
        {
            delta.y -= m_verticalRatio;
        }
        if (Input.GetKey((KeyCode)GameInput.MoveLeft))
        {
            delta.x -= 1;
        }
        if (Input.GetKey((KeyCode)GameInput.MoveRight))
        {
            delta.x += 1;
        }
        m_body.MovePosition(m_body.position + (delta * Time.deltaTime *
                           ((Input.GetKey((KeyCode)GameInput.Sprint)) ?
                           m_runSpeed : m_walkSpeed)));
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        m_canInteract = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        m_canInteract = false;
    }
}
