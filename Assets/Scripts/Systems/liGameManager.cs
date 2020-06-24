﻿using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class PlayerData
{
    public string m_name;

    public Vector3 m_torsoColor;
    public Vector3 m_featColor;
    public Vector3 m_headColor;
    public Vector3 m_skinColor;
    public Vector3 m_armColor;
}

public class liGameManager : MonoBehaviour
{
    public static liGameManager instance;

    public static float m_time;
    public static int m_day;
    public static DialogSceneData m_sceneDialogs;

    public static PlayerData m_playerData;

    private List<BaseUIManager> m_OpenUIManagers = new List<BaseUIManager>();

    /// <summary>
    /// signals if any window ui is currently active.
    /// </summary>
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

    void Start()
    {
        if(!EventSystem.current) {
            Instantiate(Resources.Load<GameObject>("Prefabs/UI/EventSystem"));
        }

        if(!liDialogManager.instance) {
            Instantiate(Resources.Load<GameObject>("Prefabs/UI/Conversation"));
        }

        if(!liInventory.instance) {
            Instantiate(Resources.Load<GameObject>("Prefabs/UI/Inventory"));
        }
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
