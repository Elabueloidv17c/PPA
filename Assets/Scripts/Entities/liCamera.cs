using UnityEngine;

public class liCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject m_player;
    
    [SerializeField]
    private float m_positionZ;

    private void Start()
    {
        m_positionZ = -10.0f;  

        if(null == m_player)
        {
            m_player = FindObjectOfType<liPlayerCharacter>().gameObject;
        } 
    }

    void LateUpdate()
    {
        transform.position = new Vector3(m_player.transform.position.x, 
                                         m_player.transform.position.y,
                                         m_positionZ);
    }
}