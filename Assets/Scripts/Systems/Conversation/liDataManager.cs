using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// TODO: Document file

public class liDataManager : MonoBehaviour
{
    public static uint s_currentDay = 0;
    public static liScene s_currentScene = liScene.TownCenter;
    public static float s_time = 0.0f;
    public static Scene m_data;
    private List<GameObject> m_objectsOnLayer;

    void Start() 
    {
        s_currentDay++;
        s_time = 0.0f;
        m_data = ParseConversation();
    }

    void Update() 
    {
        s_time += Time.deltaTime;
    }

    private Scene ParseConversation() {
        string data = File.ReadAllText(Application.streamingAssetsPath + "/" + 
                                       s_currentScene.ToString() + "_" + s_currentDay + ".json");
        return JsonConvert.DeserializeObject<Scene>(data);
    }

    public enum liScene {
        House,
        Library,
        TownCenter
    }
}

public struct Scene
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
    public LogAction LogAction;
    public Option[] Options;
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

public struct Option
{
    public string Text;
    public int Next;
    public int Value;
}

public struct LogAction
{
    public ActionType ActionType;
    public int Next;
    public int Value;
}

[JsonConverter(typeof(StringEnumConverter))]  
public enum ActionType {
    None,
    JumpToNext,
    Buttons,
    End,
    GiveItem,

    //... 
}