using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


[Serializable]
public class liCraftableItem : IComparable<liCraftableItem>, IComparable<int[]>
{
  public liCraftableItem(int in_resultingItem, int[] in_requiredItems)
  {
    m_resultingItemID = in_resultingItem;
    m_requiredItemIDs = (int[])in_requiredItems.Clone();
    Array.Sort(m_requiredItemIDs);
  }
  public int resultingItemID { get { return m_resultingItemID; } }
  public int[] requiredItemIDs { get { return m_requiredItemIDs; } }


  protected int m_resultingItemID;
  protected int[] m_requiredItemIDs;

  /// <summary>
  /// Sorts craftable items first by the amount of required items each one has,
  /// then by the value of each item.
  /// </summary>
  /// <param name="otherItem"></param>
  /// <returns>'0' when they are equal, '1' when current liCraftableItem is greater than otherItem,
  /// '-1' when otherItem is greater then the current liCraftableItem. </returns>
  public int CompareTo(liCraftableItem otherItem)
  {
    // use the array comparison
    return CompareTo(otherItem.m_requiredItemIDs);
  }

  /// <summary>
  /// Sorts craftable items first by the amount of required items each one has,
  /// then by the value of each item.
  /// </summary>
  /// <param name="ArrayItemIDs"></param>
  /// <returns>'0' when they are equal, '1' when current liCraftableItem is greater than otherItem,
  /// '-1' when otherItem is greater then the current liCraftableItem. </returns>
  public int CompareTo(int[] ArrayItemIDs)
  {

    int arrayCompare = m_requiredItemIDs.Length.CompareTo(ArrayItemIDs.Length);

    if (0 == arrayCompare)
    {
      int arrayLength = m_requiredItemIDs.Length;
      int compareResult = 0;
      for (int i = 0; i < arrayLength; ++i)
      {
        compareResult = m_requiredItemIDs[i].CompareTo(ArrayItemIDs[i]);
        if (0 != compareResult)
          return compareResult;
      }
    }

    return arrayCompare;
  }
}
