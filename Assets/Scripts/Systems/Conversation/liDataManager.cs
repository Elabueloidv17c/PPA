using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
/// Loads and stores data from json files.
/// </summary>
public class liDataManager : MonoBehaviour
{
    public static uint s_currentDay = 0;
    public static EDialogScene s_currentScene = EDialogScene.WatermelonTree;
    public static float s_time = 0.0f;
    public static DialogSceneData m_data;
    private List<GameObject> m_objectsOnLayer;

    void Start() 
    {
        s_currentDay++;
        s_time = 0.0f;
        m_data = LoadConversationsFile();
    }

    void Update() 
    {
        s_time += Time.deltaTime; // ???
    }

    /// <summary>
    /// Loads conversation data for the current scene
    /// </summary>
    /// <returns>the loaded scene data</returns>
    private DialogSceneData LoadConversationsFile() {
        string data = File.ReadAllText(Application.streamingAssetsPath + "/" + 
                                       s_currentScene.ToString() + "_" + s_currentDay + ".json");
        return JsonConvert.DeserializeObject<DialogSceneData>(data);
    }
}

public struct DialogSceneData
{
    public string InComment;
    public string OutComment;
    public Conversation[] Conversations;
}

public struct Conversation
{
    public liCharacter Character;
    public Dialog[] Dialogs;
}

public struct Dialog
{
    public DialogCharacter[] LeftCharacter;
    public DialogCharacter[] RightCharacter;
    public int ActiveCharacterLeft;
    public int ActiveCharacterRight;
    public bool ActiveSide; // false if left, true if right
    public string Text;
    public string Thought;
    public LogActionData LogActionData;
    public LogOption[] Options;
}

[JsonConverter(typeof(StringEnumConverter))]  
public enum liCharacter {
    Tree,
    River,
    Nero,
    Izzy,
    Shiro,
    Hiroshi,
    La_Tia
}

public struct DialogCharacter
{
    public liCharacter CharacterID;
    public int Expression;
}

/// <summary>
/// Contains data as loaded from file about a dialog action.
/// </summary>
public struct LogOption
{
    public string Text;
    public int Next;
    public int Value;
}