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

  public static liCrafting instance;

  private void Awake()
  {
    instance = this;
  }

  private void Start()
  {
    s_possibleCraftableItems = new List<liCraftableItem>();
  }


  /// <summary>
  /// Takes an array of item IDs and check to see if an item can be crafted
  /// </summary>
  /// <param name="itemIDsUseToCraft"> Contains the ids of the items used for crafting </param>
  /// <returns>'-1' when the combination of IDs is invalid, 
  /// otherwise gives corresponding  </returns>
  public int GetItemIDForCraftableItem(int[] itemIDsUseToCraft)
  {
    Array.Sort(itemIDsUseToCraft);
    if (CheckForDuplicates(itemIDsUseToCraft))
      return -1;

    var inventory = liInventory.instance;
    if (false == checkPlayerHasItems(itemIDsUseToCraft, inventory))
      return -1;


    int index =
      s_possibleCraftableItems.FindIndex(items => 0 == items.CompareTo(itemIDsUseToCraft));
    if (-1 != index)
    {
      removeItemsFromInventory(itemIDsUseToCraft, inventory);
      return s_possibleCraftableItems[index].resultingItemID;
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

  #region InspectorButtons

#if UNITY_EDITOR // This code is ONLY for debugging in the editor.

  [InspectorButton("Inspector_CreateDefaultItem")]
  public bool i_CreateDefaultItem;

  private void Inspector_CreateDefaultItem()
  {
    if (!Application.isPlaying)
    {
      Debug.LogWarning("Calling method without running the game... you fool.");
      return;
    }

    if (gameObject.IsPrefab())
    {
      Debug.LogWarning("Calling method from prefab... you fool.");
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
  public int[] InspectorRequiredItems;
  public int InspectorResultingID;


  private void Inspector_AddItem()
  {
    if (!Application.isPlaying)
    {
      Debug.LogWarning("Calling method without running the game... you fool.");
      return;
    }

    if (gameObject.IsPrefab())
    {
      Debug.LogWarning("Calling method from prefab... you fool.");
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
  public int[] test_arrayItems;


  private void Inspector_CraftItem()
  {
    if (!Application.isPlaying)
    {
      Debug.LogWarning("Calling method without running the game... you fool.");
      return;
    }

    if (gameObject.IsPrefab())
    {
      Debug.LogWarning("Calling method from prefab... you fool.");
      return;
    }

    int ItemID = GetItemIDForCraftableItem(test_arrayItems);
    if (-1 != ItemID)
    {
      liInventory.instance.AddItem(ItemID);
      Debug.Log("successfully Created Item");
    }
    else
    {
      Debug.Log("Item was not valid");
    }

  }


  [InspectorButton("Inspector_PrintItems")]
  [SerializeField]
  private bool i_printItems;

  private void Inspector_PrintItems()
  {
    if (!Application.isPlaying)
    {
      Debug.LogWarning("Calling method without running the game... you fool.");
      return;
    }

    if (gameObject.IsPrefab())
    {
      Debug.LogWarning("Calling method from prefab... you fool.");
      return;
    }

    string result = "";

    foreach (liCraftableItem item in s_possibleCraftableItems)
    {
      result += "Resulting item =" +
       item.resultingItemID.ToString();


      result += "\n Items for Creating Resulting Item =[";
      foreach (int i in item.requiredItemIDs)
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
  private bool i_serializeItems;

  /// <summary>
  /// Serializes the craftable items.
  /// </summary>
  private void Inspector_SerialzeItems()
  {
    string Path = Application.streamingAssetsPath + "/craftable_items.json";
    if (false == File.Exists(Path))
    {
      File.Create(Path);
    }

    FileStream fileStream = File.OpenWrite(Path);

    string[] data = new string[s_possibleCraftableItems.Count];

    for (int i =0; i < s_possibleCraftableItems.Count; ++i)
    {
      data[i] = JsonConvert.SerializeObject(s_possibleCraftableItems[i]) + "\n";
    }

    foreach(string s in data)
    {
      byte[] info = new UTF8Encoding(true).GetBytes(s);
      fileStream.Write(info, 0, info.Length);
    }

    fileStream.Close();
  }



#endif

  #endregion
}

