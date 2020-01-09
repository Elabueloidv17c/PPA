using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
  public static uint s_currentDay = 0;
  public static uint s_currentScene = 2;
  public static float s_time = 0.0f;
  public static Scene m_data;
  private List<GameObject> m_objectsOnLayer;

  public static bool m_showingConversationUI;
  public GameObject m_conversationUI;

  void FindRenderersOnLayer(int layer) {
      var objectsInScene = FindObjectsOfType<GameObject>();
      var objectsInLayer = new List<GameObject>();
      for (var i = 0; i < objectsInScene.Length; i++) {
          if (objectsInScene[i].layer == layer) {
              objectsInLayer.Add(objectsInScene[i]);
          }
      }
      m_objectsOnLayer = objectsInLayer;
  }

  void Start() {
      s_currentDay++;
      s_time = 0.0f;
      m_data = ParseConversation();
      //Load Objects Here

      //Then Load Renderers
      FindRenderersOnLayer(9);
  }

  void Update() {
      s_time += Time.deltaTime;
    }

  private void FixedUpdate()
  {
    DrawOrder();
  }

  void DrawOrder()
  {
    foreach (var currentGO in m_objectsOnLayer)
    {
        float sort = (currentGO.transform.position.y < 0) ?
                     -currentGO.transform.position.y * 10 :
                      currentGO.transform.position.y * 10;
    
        currentGO.GetComponent<Renderer>().sortingOrder = (int)sort;
    }
  }

  private Scene ParseConversation() {
    string data = File.ReadAllText(Application.streamingAssetsPath + "/" + 
                                   getSceneName() + "_" + s_currentDay + ".json");
    return JsonConvert.DeserializeObject<Scene>(data);
  }

  private string getSceneName() {
    if (0 == s_currentScene){
        return "House";
    }
    if (1 == s_currentScene){
        return "Library";
    }
    if (2 == s_currentScene)
    {
        return "TownCenter";
    }
        
    return "";
  }

  public static string getCharacterName(int ID)
  {
    switch (ID)
    {        
        case 0:
            return "Tree";
        case 1:
            return "River";
        case 2:
            return "Nero";
        case 3:
            return "Izzy";
        case 4:
            return "Shiro";
        case 5:
            return "Hiroshi";
        case 6:
            return "La tia";
        default:
            return "";
    }
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
    public int Character;
    public Dialog[] Dialogs;
}

public struct Dialog
{
    public int Expression;
    public string ThoughtA;
    public string Text;
    public string ThoughtB;
    public int Character;
    public int Next;
    public Log Action;
    public Item Inventory;
    public Option[] Options;
}

public struct Option
{
    public string Text;
    public int Next;
    public int Value;
}

public struct Log
{
    public int ID;
    public int Next;
    public int Value;
}

public struct Item
{
    public int ID;
    public int Next;
    public int Value;
}

public enum GameInput
{
    MoveUp = KeyCode.UpArrow,
    MoveDown = KeyCode.DownArrow,
    MoveLeft = KeyCode.LeftArrow,
    MoveRight = KeyCode.RightArrow,
    Sprint = KeyCode.Space,
    Interact = KeyCode.A,
    Exit = KeyCode.Escape
}