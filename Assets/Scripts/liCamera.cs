using UnityEngine;

public class liCamera : MonoBehaviour
{
    public GameObject m_player;
    public float m_positionZ;

    private void Start()
    {
        m_positionZ = -10.0f;   
    }

    void LateUpdate()
    {
        transform.position = new Vector3(m_player.transform.position.x, 
                                         m_player.transform.position.y,
                                         m_positionZ);
    }
}