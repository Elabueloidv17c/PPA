using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class liGameManager : MonoBehaviour
{
    [SerializeField]
    static public float m_time;
    [SerializeField]
    static public int m_day;
    [SerializeField]
    static public Scene m_scene;
    [SerializeField]
    public Vector3 m_torsoColor;
    [SerializeField]
    public Vector3 m_featColor;
    [SerializeField]
    public Vector3 m_headColor;
    [SerializeField]
    public Vector3 m_skinColor;
    [SerializeField]
    public Vector3 m_armColor;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        Instantiate(this);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
