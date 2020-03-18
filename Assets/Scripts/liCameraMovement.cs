using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class liCameraMovement : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    private float offsetZ;
    public float speed = 3;
    [SerializeField]
    Vector2 border;

    void Awake()
    {
        if(!target) {
            target = FindObjectOfType<liPlayerCharacter>().transform;
        }
    }

    void Start()
    {
        offsetZ = transform.position.z - target.position.z;
    }

    void FixedUpdate()
    {
        Vector2 distance = transform.position - target.position;
        distance = new Vector2(Mathf.Abs(distance.x), Mathf.Abs(distance.y));

        if (distance.x > border.x && distance.y > border.y)
        {
            transform.position = Vector3.Lerp(transform.position,
                target.position + Vector3.forward * offsetZ, speed * Time.fixedDeltaTime);
        }
    }
    void Update()
    {
        
    }
}
