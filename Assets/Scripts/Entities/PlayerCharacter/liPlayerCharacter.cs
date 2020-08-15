using UnityEngine;


public class liPlayerCharacter : MonoBehaviour
{
    GameObject Shirt;
    GameObject Hair;
    GameObject Skin;
    GameObject Shoes;
    GameObject Pants;

    /// <summary>
    /// en esta seccion se crean get and set de todos las 
    /// propiedades que tiene el player
    /// </summary>
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

    public Animator animator_shirt;
    public Animator animator_hair;
    public Animator animator_skin;
    public Animator animator_shoes;
    public Animator animator_pants;

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

    /// <summary>
    /// aqui genero todos los estados  
    /// que existen que son run, walk, idle
    /// ademas de que se crea la maquina la cual es la que mueve a todos los estados
    /// </summary>
    StateMachine<liPlayerCharacter> machine;
    StateRUN run;
    StateWalk walk;
    StateIdle idle;
    private void Awake()
    {
        Shirt = transform.GetChild(0).gameObject;
        Pants = transform.GetChild(1).gameObject;
        Skin = transform.GetChild(2).gameObject;
        Hair = transform.GetChild(3).gameObject;
        Shoes = transform.GetChild(4).gameObject;
        /*
         * en esta seccion inicializamos todas las variables y los estados y empezamos a correr
         * la maquina de estados con el estado de idle
         */
        machine = new StateMachine<liPlayerCharacter>();
        run = new StateRUN(machine,this);
        walk = new StateWalk(machine, this);
        idle = new StateIdle(machine, this);
        animator = GetComponent<Animator>();

        animator_hair = Hair.GetComponent<Animator>();
        animator_pants = Pants.GetComponent<Animator>();
        animator_shoes = Shoes.GetComponent<Animator>();
        animator_skin = Skin.GetComponent<Animator>();
        animator_shirt = Shirt.GetComponent<Animator>();
        //Initializing position facing down
        animSetFloats("Y", -1);
    }
    void Start() {
        m_body = GetComponent<Rigidbody2D>();
      
        machine.Init(this, idle);

    }
    /// <summary>
    /// en fix update va hacer toda la logica del player para que se mueva y cambie de estados
    /// en este caso se mueve y cambia a caminar y si preciona 'sprint ' se cambia a correr
    /// y si no presiona nada se queda en idle
    /// </summary>
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
    /// <summary>
    /// aqui vemos que la funcion isMoving me dice si esta moviendo o no
    /// 
    /// </summary>
    /// retorna un true si se mueve 
    /// <returns></returns>
  private bool isMoving()
    {
        return (Input.GetKey((KeyCode)GameInput.MoveUp) || Input.GetKey((KeyCode)GameInput.MoveDown) ||
            Input.GetKey((KeyCode)GameInput.MoveRight) || Input.GetKey((KeyCode)GameInput.MoveLeft));
        

    }

    internal void animSetFloats(string field, float value)
    {
        animator.SetFloat(field, value);
        animator_hair.SetFloat(field, value);
        animator_pants.SetFloat(field, value);
        animator_shoes.SetFloat(field, value);
        animator_skin.SetFloat(field, value);
        animator_shirt.SetFloat(field, value);
    }
}
