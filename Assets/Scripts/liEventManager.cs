using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class SaveFile
{
    public EventsBank m_gameEvents;
    // ...
}

[Serializable]
public class EventsBank
{
    public Dictionary<string, bool> m_bEvents;
    public Dictionary<string, int> m_iEvents;
    public Dictionary<string, float> m_fEvents;
}

public class liEventManager : MonoBehaviour
{
    public liEventManager instance;

    EventsBank eventsBank;

    void Start() 
    {
        instance = this;
        eventsBank = new EventsBank();
    }

    public bool GetFlag(string name)
    {
        return eventsBank.m_bEvents[name];
    }

    public int GetInt(string name)
    {
        return eventsBank.m_iEvents[name];
    }

    public float GetFloat(string name)
    {
        return eventsBank.m_fEvents[name];
    }

    public void SetFlag(string name, bool flag)
    {
        eventsBank.m_bEvents[name] = flag;
    }

    public void SetInt(string name, int integer)
    {
        eventsBank.m_iEvents[name] = integer;
    }

    public void SetFloat(string name, float real)
    {
        eventsBank.m_fEvents[name] = real;
    }
}