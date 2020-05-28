using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using UnityEngine;

public class liCrafting : MonoBehaviour
{

  // TODO : Only use this for testing 
  [SerializeField]
  liCraftableItem m_testDummyItem;

  static List<liCraftableItem> s_possibleCraftableItems;

  /// <summary>
  /// Takes an array of item IDs and check to see if an item can be crafted
  /// </summary>
  /// <param name="itemIDsUseToCraft"> Contains the ids of the items used for crafting </param>
  /// <returns>'-1' when the combination of IDs is invalid, 
  /// otherwise give the </returns>
  public int tryToCraftItem(int[] itemIDsUseToCraft)
  {
    Array.Sort(itemIDsUseToCraft);
    if (checkForDuplicates(itemIDsUseToCraft))
      return -1;

    var inventory = liInventory.instance;
    foreach (int i in itemIDsUseToCraft)
    {
      if (false == inventory.HasItem(i))
      { return -1; }
    }

    return -1;
  }

  /// <summary>
  /// check for duplicates Item. Requires that the array be sorted first.
  /// </summary>
  /// <param name="itemUsedToCraft"></param>
  /// <returns>'false' when there are no duplicates, 'true' otherwise.</returns>
  private bool checkForDuplicates(int[] itemIDsUseToCraft)
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
  private void AddCraftableItem(int[] requiredIDs, int resultID)
  {
    liCraftableItem temp = new liCraftableItem(resultID, requiredIDs);

    s_possibleCraftableItems.Add(temp );
  }

#if UNITY_EDITOR // This code is ONLY for debugging in the editor.
    [InspectorButton("Inspector_AddItem")]
  public bool UseFunc_AddItem;
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

    AddCraftableItem(InspectorRequiredItems, InspectorResultingID);

  }

#endif

}

/// <summary>
/// Defines the id of a item that can be crafted  
/// and what items are necessary in order to craft it. 
/// </summary>
