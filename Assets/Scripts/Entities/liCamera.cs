using UnityEngine;

public class liCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;
    
    [SerializeField]
    private float m_positionZ;

    float distance_x;
    float distance_y;
    public float speed = 3;
    private float offsetZ;
    private void Start()
    {
        m_positionZ = -10.0f;
        
        if(null == m_player)
        {
            m_player = FindObjectOfType<liPlayerCharacter>().gameObject;
        }
        offsetZ = transform.position.z - m_player.transform.position.z;
    }

    void LateUpdate()
    {
        distance_x = Mathf.Abs((transform.position.x - m_player.transform.position.x));
        distance_y = Mathf.Abs((transform.position.y - m_player.transform.position.y));
        if (distance_x > 2 || distance_y > 1)
        {
           transform.position = Vector3.Lerp(transform.position,
               m_player.transform.position + Vector3.forward * offsetZ, Time.fixedDeltaTime);
        }
    }
}