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
