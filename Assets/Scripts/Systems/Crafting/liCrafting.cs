using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using UnityEngine;

public class liCrafting : MonoBehaviour
{
  /// <summary>
  /// Contains all the valid craftable items
  /// </summary>
  protected static List<liCraftableItem> s_possibleCraftableItems;

  protected static List<liCraftableItem> s_knownRecipesByPlayer;

  public static liCrafting instance;

  private void Awake()
  {
    instance = this;
  }

  private void Start()
  {
    s_possibleCraftableItems = new List<liCraftableItem>();
    s_knownRecipesByPlayer = new List<liCraftableItem>();
  }


  /// <summary>
  /// Takes an array of item IDs and checks to see if an item can be crafted.
  /// </summary>
  /// <param name="itemIDsUseToCraft"> Contains the ids of the items used for crafting </param>
  /// <returns>'-1' when the combination of IDs is invalid, 
  /// otherwise gives corresponding resulting item ID. </returns>
  public int GetItemIDForCraftableItem(int[] itemIDsUseToCraft)
  {
    Array.Sort(itemIDsUseToCraft);
    if (CheckForDuplicates(itemIDsUseToCraft))
      return -1;

    var inventory = liInventory.instance;
    removeItemsFromInventory(itemIDsUseToCraft, inventory);

    int index =
      s_possibleCraftableItems.FindIndex(items => 0 == items.CompareTo(itemIDsUseToCraft));
    if (-1 != index)
    {
      givePlayerCraftableItemRecipe(itemIDsUseToCraft);
      return s_possibleCraftableItems[index].m_resultingItemID;
    }

    return -1;
  }



  /// <summary>
  /// check for duplicates Item. Requires that the array be sorted first.
  /// </summary>
  /// <param name="itemUsedToCraft"></param>
  /// <returns>'false' when there are no duplicates, 'true' otherwise.</returns>
  private bool CheckForDuplicates(int[] itemIDsUseToCraft)
  {
    for (int i = 1; i < itemIDsUseToCraft.Length; ++i)
    {
      if (itemIDsUseToCraft[i] == itemIDsUseToCraft[i - 1])
        return true;
    }

    return false;
  }

  /// <summary>
  /// Adds a new CraftableItem to the list of Craftable items, after performing
  /// a few check to make sure the item is valid.
  /// </summary>
  /// <param name="requiredIDs"></param>
  /// <param name="resultID"></param>
  /// <returns>'true' when an item was added successfully,'false' otherwise.</returns>
  private bool AddCraftableItem(int[] requiredIDs, int resultID)
  {
    liCraftableItem possiblyNewItem = new liCraftableItem(resultID, requiredIDs);

    if (0 > s_possibleCraftableItems.BinarySearch(possiblyNewItem))
    {
      s_possibleCraftableItems.Add(possiblyNewItem);
      s_possibleCraftableItems.Sort();
      return true;
    }
    return false;
  }

  /// <summary>
  /// Checks to make sure the player has every they pass to the function.
  /// </summary>
  /// <param name="itemIDsUseToCraft"> The items to be removed from the inventory</param>
  /// <param name="inventory"></param>
  /// <returns>'true' if the player does have every item, 'false' when they don't.</returns>
  private bool checkPlayerHasItems(int[] itemIDsUseToCraft, liInventory inventory)
  {
    foreach (int i in itemIDsUseToCraft)
    {
      if (false == inventory.HasItem(i))
        return false;
    }

    return true;
  }

  /// <summary>
  /// Removes item to simulate using them to craft an item.
  /// </summary>
  /// <param name="itemIDsUseToCraft">Item that are going to be used</param>
  /// <param name="inventory"></param>
  private void removeItemsFromInventory(int[] itemIDsUseToCraft, liInventory inventory)
  {
    foreach (int i in itemIDsUseToCraft)
    {
      Debug.Assert(inventory.RemoveItem(i) == true, "Trying to remove an item that the inventory does not have");
    }

  }

  /// <summary>
  /// Gets the file-path of the .JSON that stores all the valid craftable items.
  /// </summary>
  /// <returns>The file-path to the .JSON file. </returns>
  private static string getPathToJsonForCraftableItems()
  {
    return Application.streamingAssetsPath + "/craftable_items.json";
  }

  /// <summary>
  /// 
  /// </summary>
  /// <returns>The Path to the ".Json" file.</returns>
  public static string getPathToJsonForCraftableItemsKnownByPlayer()
  {
    return Application.streamingAssetsPath + "/craftable_items_known_by_player.json";
  }

  public bool LoadFromJson<T>(string path, ref T dataContainer)
  {
    if (File.Exists(path))
    {
      string data = File.ReadAllText(path);
      dataContainer = JsonConvert.DeserializeObject<T>(data);
      return true;
    }
    else
    {
      int lastIndexOfForwardSlash = path.LastIndexOf('/');
      string nameOfFile = path.Substring(lastIndexOfForwardSlash);
      Debug.LogWarning("The File '" + nameOfFile + "' does not exist or cant be found.");
    }

    return false;
  }

  public void SaveToJson<T>(string Path,ref T dataContainer)
  {
    FileStream fileStream = null; 

    if (false == File.Exists(Path))
    {
      fileStream = File.Create(Path);
    }
    else
    {
      fileStream = File.OpenWrite(Path);
    }
    
    string data = JsonConvert.SerializeObject(dataContainer, Formatting.Indented);

    byte[] info = new UTF8Encoding(true).GetBytes(data);
    fileStream.Write(info, 0, info.Length);

    fileStream.Close();
  }


  /// <summary>
  /// Save the items in ' s_possibleCraftableItems ' to a JSON file.
  /// </summary>
  public void SaveCurrentItems()
  {
    string Path = getPathToJsonForCraftableItems();
    FileStream fileStream = null; 

    if (false == File.Exists(Path))
    {
      fileStream = File.Create(Path);
    }
    else
    {
      fileStream = File.OpenWrite(Path);
    }
    
    string data = JsonConvert.SerializeObject(s_possibleCraftableItems, Formatting.Indented);

    byte[] info = new UTF8Encoding(true).GetBytes(data);
    fileStream.Write(info, 0, info.Length);

    fileStream.Close();
  }

  /// <summary>
  /// Loads a data base of the valid craftable items from a '.Json' file.
  /// </summary>
  /// <returns> "true" when the file was loaded successfully , "false" otherwise. </returns>
  public bool LoadItem()
  {
    string path = getPathToJsonForCraftableItems();
    if (File.Exists(path))
    {
      string data = File.ReadAllText(path);
      s_possibleCraftableItems = JsonConvert.DeserializeObject<List<liCraftableItem>>(data);
      return true;
    }
    else
    {
      int lastIndexOfForwardSlash = path.LastIndexOf('/');
      string nameOfFile = path.Substring(lastIndexOfForwardSlash);
      Debug.LogWarning("The File '" + nameOfFile + "' does not exist or cant be found.");
    }

    return false;
  }

  /// <summary>
  /// Give the player knowledge of a recipe when the recipe is valid.
  /// </summary>
  /// <param name="possiblyValidRecipe"> The Recipe the player entered </param>
  /// <returns> 'true' if the player gains the knowledge of the recipe , 'false' otherwise </returns>
  bool givePlayerCraftableItemRecipe(int[] possiblyValidRecipe)
  {
    int index = s_possibleCraftableItems.FindIndex(x => (0 == x.CompareTo(possiblyValidRecipe)));

    if (-1 != index)
    {
      liCraftableItem itemAtIndex = s_possibleCraftableItems[index];
      if (0 > s_knownRecipesByPlayer.BinarySearch(s_possibleCraftableItems[index]))
      {
        s_knownRecipesByPlayer.Add(itemAtIndex);
        s_knownRecipesByPlayer.Sort();
      }
      else
      {
        return false;
      }
      return true;
    }
    return false;
  }


  #region InspectorButtons

#if UNITY_EDITOR // This code is ONLY for debugging in the editor.

  private bool IsItOkToUseInspectorFunction()
  {
    if (!Application.isPlaying)
    {
      Debug.LogWarning("Calling method without running the game... you fool.");
      return false;
    }

    if (gameObject.IsPrefab())
    {
      Debug.LogWarning("Calling method from prefab... you fool.");
      return false;
    }

    return true;
  }

  [InspectorButton("Inspector_CreateDefaultItem")]
  public bool addDefaultItem_;

  /// <summary>
  /// Adds Something default items to the list of valid craftable items.
  /// </summary>
  private void Inspector_CreateDefaultItem()
  {
    if (false == IsItOkToUseInspectorFunction())
    {
      return;
    }

    int squidward = 10;

    int[] squidwardRecepi = new int[] { 1, 5, 9 };


    int something = 1;

    int[] somethingRequiredItem = new int[] { 2, 6, 10 };//required

    int something2 = 2;

    int[] somethingRequiredItem2 = new int[] { 3, 6, 10 };//required

    if (AddCraftableItem(squidwardRecepi, squidward))
    {
      Debug.Log("successfully made squidward/calamardo");//squid-world
    }

    if (AddCraftableItem(somethingRequiredItem, something))
    {
      Debug.Log("successfully Add something item with the id of " + something.ToString());
    }

    if (AddCraftableItem(somethingRequiredItem2, something2))
    {
      Debug.Log("successfully Add something item with the id of " + something2.ToString());
    }

  }


  [InspectorButton("Inspector_AddItem")]
  [SerializeField]
  private bool AddItem_;
  [SerializeField]
  private int[] InspectorRequiredItems;
  [SerializeField]
  private int InspectorResultingID;

  /// <summary>
  /// Adds items to the container thats used to check to see if a
  /// combination of items can form a new item.
  /// </summary>
  private void Inspector_AddItem()
  {

    if (false == IsItOkToUseInspectorFunction())
    {
      return;
    }

    if (AddCraftableItem(InspectorRequiredItems, InspectorResultingID))
    {
      Debug.Log("successfully Added Craftable item"); ;
    }
    else
    {
      Debug.Log("Failed to Add craftable item");
    }

  }


  [InspectorButton("Inspector_CraftItem")]
  [SerializeField]
  private bool CraftItem_;

  private void Inspector_CraftItem()
  {

    if (false == IsItOkToUseInspectorFunction())
    {
      return;
    }

    int ItemID = GetItemIDForCraftableItem(InspectorRequiredItems);
    if (-1 != ItemID)
    {
      if (liInventory.instance.AddItem(ItemID))
      {
        Debug.Log("Successfully created and added Item");
      }
      else
      {
        Debug.Log("Could not add item");
      }
    }
    else
    {
      Debug.Log("Item was not valid");
    }

  }


  [InspectorButton("Inspector_PrintItems")]
  [SerializeField]
  private bool PrintItems_;

  /// <summary>
  ///  Prints the values that every valid craftable items has.
  /// </summary>
  private void Inspector_PrintItems()
  {

    if (false == IsItOkToUseInspectorFunction())
    {
      return;
    }

    string result = "";

    foreach (liCraftableItem item in s_possibleCraftableItems)
    {
      result += "Resulting item =" +
       item.m_resultingItemID.ToString();


      result += "\n Items for Creating Resulting Item =[";
      foreach (int i in item.m_requiredItemIDs)
      {
        result += i.ToString() + ", ";
      }
      result += "]\n\n";

      Debug.Log(result);
      result = "";
    }


  }


  [InspectorButton("Inspector_SerialzeItems")]
  [SerializeField]
  private bool SaveItems_;

  /// <summary>
  /// Serializes the craftable items, in other words put them in a .JSON file.
  /// </summary>
  private void Inspector_SerialzeItems()
  {
    if (false == IsItOkToUseInspectorFunction())
    {
      return;
    }

    SaveCurrentItems();
  }

  [InspectorButton("Inspector_DeserialzeItems")]
  [SerializeField]
  private bool LoadItems_;

  private void Inspector_DeserialzeItems()
  {
    if (false == IsItOkToUseInspectorFunction())
    {
      return;
    }

    LoadItem();

  }

  [InspectorButton("Inspector_LoadPlayerKnowledge")]
  [SerializeField]
  private bool loadPlayerKnowledge_;
  private void Inspector_LoadPlayerKnowledge()
  {
    if (false == IsItOkToUseInspectorFunction())
    {
      return;
    }
    string path = getPathToJsonForCraftableItemsKnownByPlayer();
    LoadFromJson<List<liCraftableItem>>(path, ref s_knownRecipesByPlayer);

    string result = "";
    foreach (liCraftableItem item in s_knownRecipesByPlayer)
    {
      result += "Resulting item =" +
       item.m_resultingItemID.ToString();


      result += "\n Items for Creating Resulting Item =[";
      foreach (int i in item.m_requiredItemIDs)
      {
        result += i.ToString() + ", ";
      }
      result += "]\n\n";

      Debug.Log(result);
      result = "";
    }

  }

  [InspectorButton("Inspector_SaveKnowledge")]
  [SerializeField]
  private bool givePlayerKnowledge_;
  private void Inspector_SaveKnowledge()
  {
    if (false == IsItOkToUseInspectorFunction())
    {
      return;
    }

    string path = getPathToJsonForCraftableItemsKnownByPlayer();
    SaveToJson<List<liCraftableItem>>(path, ref s_knownRecipesByPlayer);
    s_knownRecipesByPlayer.Sort();
  }

#endif

  #endregion
}

