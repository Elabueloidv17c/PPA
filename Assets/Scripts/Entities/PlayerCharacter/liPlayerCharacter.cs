using UnityEngine;


public class liPlayerCharacter : MonoBehaviour
{


     Rigidbody2D m_body;
    public Rigidbody2D body {
        get { return m_body; }
        set { m_body = value; }

    }
    public Vector2 m_movement;
    public Vector2 movement{
        get { return m_movement; }
        set { m_movement = value; }
    }
    
    public Animator animator { get ; private set; }
    
    public float m_verticalRatio;
    public float verticalRatio {

        get { return m_verticalRatio; }
        set { m_verticalRatio = value; }
    }
        
    public float m_walkSpeed = 1.8f;
    public float walkSpeed {
        get { return m_walkSpeed; }
        set { m_walkSpeed = value; }
    }

    public float m_runSpeed = 2.8f;
    public float runSpeed
    {
        get { return m_runSpeed; }
        set { m_runSpeed = value; }
    }

    public liPlayerCharacter Entity => entity;

    private readonly liPlayerCharacter entity;
    StateMachine<liPlayerCharacter> machine;

    StateRUN run;
    StateWalk walk;
    StateIdle idle;
    private void Awake()
    {
        
        machine = new StateMachine<liPlayerCharacter>();
        run = new StateRUN(machine,Entity);
        walk = new StateWalk(machine, Entity);
        idle = new StateIdle(machine, Entity);
        animator = GetComponent<Animator>();
        animator.SetFloat("Y", -1);
    }
    void Start() {
        m_body = GetComponent<Rigidbody2D>();
      
        machine.Init(Entity, idle);
       
    }
    private void FixedUpdate()
    {
        machine.onState();
        if (isMoving()) {

            if (Input.GetKey((KeyCode)GameInput.Sprint))
            {

                machine.toState(run);
            }
            machine.toState(walk);
        }
        else
        {
            machine.toState(idle);
        }
    }

    private void Update() {
        
        
    }

  private bool isMoving()
    {
        return (Input.GetKey((KeyCode)GameInput.MoveUp) || Input.GetKey((KeyCode)GameInput.MoveDown) ||
            Input.GetKey((KeyCode)GameInput.MoveRight) || Input.GetKey((KeyCode)GameInput.MoveLeft));
        

    }
}
