using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

[Serializable]
public class SaveFile
{
    public PlayerData m_playerData;
    public EventsBank m_gameEvents;
    public DateTime m_timeStamp;
    public ItemInstance[] m_items;
    public ItemInstance[] m_depotItems;
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

    public void SaveGame(int slot) {
        // Create Save File
        var saveFile = new SaveFile();
        saveFile.m_gameEvents = eventsBank;
        saveFile.m_items = liInventory.s_currentItems.ToArray();
        saveFile.m_depotItems = liInventory.s_depositItems.ToArray();
        
        // Get time stamp
        saveFile.m_timeStamp = DateTime.Now;

        // Create Directory in case it doesn't exist
        Directory.CreateDirectory(folderPath);

        // Format fileName
        String fileName = folderPath + "Save_" + slot + ".lisv";

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

    public void RestoreSaveFile(SaveFile saveFile) {
        
        eventsBank = saveFile.m_gameEvents;
        liInventory.s_currentItems = new List<ItemInstance>(saveFile.m_items);
        liInventory.s_depositItems = new List<ItemInstance>(saveFile.m_depotItems);
    }

#if UNITY_EDITOR
    [InspectorButton("InspectorSaveFile")]
    public bool saveFile;

    public void InspectorSaveFile() {
        if(!Application.isPlaying) {
            Debug.LogWarning("Calling method without running the game... you fool.");
            return;
        }
        
        if(gameObject.IsPrefab()) {
            Debug.LogWarning("Calling method from prefab... you fool.");
            return;
        }
        
        SaveGame(0);
    }

    [InspectorButton("InspectorRestoreFile")]
    public bool restoreFile;

    public void InspectorRestoreFile() {
        if(!Application.isPlaying) {
            Debug.LogWarning("Calling method without running the game... you fool.");
            return;
        }
        
        if(gameObject.IsPrefab()) {
            Debug.LogWarning("Calling method from prefab... you fool.");
            return;
        }

        var saveFiles = LoadAllSaveFiles();

        if(saveFiles.Count > 0) {
            RestoreSaveFile(saveFiles[0]);
        }
        else {
            Debug.LogWarning("Save File not Found.");
        }
    }
#endif
}