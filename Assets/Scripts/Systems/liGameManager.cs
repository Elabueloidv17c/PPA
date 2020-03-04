using System;
using System.Collections.Generic;

using UnityEngine;

public class liGameManager : MonoBehaviour
{
    public static liGameManager instance;

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

    private List<BaseUIManager> m_OpenUIManagers = new List<BaseUIManager>();

    public bool menuActive { 
        get
        {
            return m_OpenUIManagers.Count > 0;
        }
    }

    void Awake()
    {
        instance = this;
    }

    public void OpenInventory() {
        liInventory.instance.OpenUI();
    }

    public void RegisterCloseUI(BaseUIManager UIManager)
    {
        if(m_OpenUIManagers.Remove(UIManager))
        {
            if(m_OpenUIManagers.Count > 0)
            {
                m_OpenUIManagers[m_OpenUIManagers.Count - 1].MaximizeUI();
            }
        }
    }

    public void RegisterOpenUI(BaseUIManager UIManager)
    {
        foreach(var ui in m_OpenUIManagers)
        {
            ui.MinimizeUI();
        }

        m_OpenUIManagers.Add(UIManager);
    }
}
