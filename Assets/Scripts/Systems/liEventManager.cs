using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

[Serializable]
public class SaveFile
{
    public EventsBank m_gameEvents;
    public int m_unixTime;
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
    public static liEventManager instance;

    EventsBank eventsBank;

    static string folderPath;

    void Awake() 
    {
        instance = this;
        eventsBank = new EventsBank();
        folderPath = Application.persistentDataPath + "/liSaveFiles/";
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

    public void SaveGame() {
        // Create Save File
        var saveFile = new SaveFile();
        saveFile.m_gameEvents = eventsBank;
        
        // Compute Unix Time
        TimeSpan t = DateTime.Now - new DateTime(1970, 1, 1);
        saveFile.m_unixTime = (int)t.TotalSeconds;

        // Format fileName
        String fileName = folderPath + "Save_" + saveFile.m_unixTime + ".lisv";

        // Save the File
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(fileName);
        bf.Serialize(file, saveFile);
        file.Close();
    }

    public List<SaveFile> LoadAllSaveFiles() {
        List<SaveFile> saveFiles = new List<SaveFile>();

        foreach (string fileName in Directory.EnumerateFiles(folderPath, "*.lisv"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(fileName, FileMode.Open);
            saveFiles.Add((SaveFile)bf.Deserialize(file));
            file.Close();
        }

        return saveFiles;
    }
}