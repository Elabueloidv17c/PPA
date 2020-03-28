using UnityEngine;

public class liCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;

    [SerializeField]
    float minDistance_x = 2;
    [SerializeField]
    float minDistance_y = 1;

    [SerializeField]
    public float speed = 3;
    private float offsetZ;
    
    private void Start()
    {   
        if(null == m_player)
        {
            m_player = FindObjectOfType<liPlayerCharacter>().gameObject;
        }
        offsetZ = transform.position.z - m_player.transform.position.z;
    }

    void LateUpdate()
    {
        float distance_x = Mathf.Abs((transform.position.x - m_player.transform.position.x));
        float distance_y = Mathf.Abs((transform.position.y - m_player.transform.position.y));
        if (distance_x > minDistance_x || distance_y > minDistance_y)
        {
           transform.position = Vector3.Lerp(transform.position,
               m_player.transform.position + Vector3.forward * offsetZ, Time.fixedDeltaTime);
        }
    }
}