using UnityEngine;

public class liCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;
    
    [SerializeField]
    private float m_positionZ;

    float distance;
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
        distance = (transform.position - m_player.transform.position).magnitude;
        if (distance > 9.2)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_player.transform.position
                + Vector3.forward * offsetZ, speed * Time.fixedDeltaTime);
           // transform.position = Vector3.Lerp(transform.position,
           //     m_player.transform.position + Vector3.forward * offsetZ, speed * Time.fixedDeltaTime);
          //  transform.position = new Vector3(m_player.transform.position.x,
          //                                   m_player.transform.position.y,
          //                                   m_positionZ);
        }
    }
}