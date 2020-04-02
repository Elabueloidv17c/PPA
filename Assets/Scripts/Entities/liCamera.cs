using UnityEngine;

using Lau_Utilities;

public class liCamera : MonoBehaviour
{
    [SerializeField]
    private Transform m_target;

    [SerializeField]
    float minDistance_x = 2;
    [SerializeField]
    float minDistance_y = 1;

    [SerializeField, DraggablePoint]
    Vector2 maxCoord;
    
    [SerializeField, DraggablePoint]
    Vector2 minCoord;

    [SerializeField]
    public float speed = 3;
    private float offsetZ;
    
    private void Start()
    {   
        if(null == m_target)
        {
            m_target = FindObjectOfType<liPlayerCharacter>().transform;
        }
        offsetZ = transform.position.z - m_target.transform.position.z;
    }

    void LateUpdate()
    {
        Vector3 targetPos = ((Vector2)m_target.position).MinComp(maxCoord).MaxComp(minCoord);

        float distance_x = Mathf.Abs((transform.position.x - targetPos.x));
        float distance_y = Mathf.Abs((transform.position.y - targetPos.y));
        if (distance_x > minDistance_x || distance_y > minDistance_y)
        {
           transform.position = Vector3.Lerp(transform.position,
               targetPos + Vector3.forward * offsetZ, Time.fixedDeltaTime);
        }
    }
}