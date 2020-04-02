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
    Vector2 maxMapCoord;
    
    [SerializeField, DraggablePoint]
    Vector2 minMapCoord;

    [SerializeField]
    public float speed = 3;
    private float offsetZ;

    Camera camComp;
    
    private void Start()
    {   
        if(null == m_target)
        {
            m_target = FindObjectOfType<liPlayerCharacter>().transform;
        }
        offsetZ = transform.position.z;

        camComp = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector2 camRectSize;
        camRectSize.x = camComp.orthographicSize * camComp.aspect;
        camRectSize.y = camComp.orthographicSize;

        Vector2 maxCoord = maxMapCoord.MaxComp(minMapCoord) - camRectSize;
        Vector2 minCoord = minMapCoord.MinComp(maxMapCoord) + camRectSize;

        Vector3 targetPos = ((Vector2)m_target.position).MinComp(maxCoord).MaxComp(minCoord);
        targetPos.z = offsetZ;

        float distance_x = Mathf.Abs((transform.position.x - targetPos.x));
        float distance_y = Mathf.Abs((transform.position.y - targetPos.y));

        if (distance_x > minDistance_x || distance_y > minDistance_y)
        {
           transform.position = Vector3.Lerp(transform.position,
                                             targetPos, speed * Time.fixedDeltaTime);
        }

        Vector3 tfrmPos = ((Vector2)transform.position).MinComp(maxCoord).MaxComp(minCoord);
        tfrmPos.z = offsetZ;

        transform.position = tfrmPos;
    }
}