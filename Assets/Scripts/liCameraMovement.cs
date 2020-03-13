using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class liCameraMovement : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    private float offsetZ;
    public float speed = 3;
    [SerializeField]
    Vector2 border;
    void Start()
    {
        offsetZ = transform.position.z - player.position.z;
    }

    void FixedUpdate()
    {
        Vector2 distance = transform.position - player.position;
        distance = new Vector2(Mathf.Abs(distance.x), Mathf.Abs(distance.y));

        if (distance.x > border.x && distance.y > border.y)
        {
            transform.position = Vector3.Lerp(transform.position,
                player.position + Vector3.forward * offsetZ, speed * Time.fixedDeltaTime);
        }
    }
    void Update()
    {
        
    }
}
