using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class liCraftableItem : MonoBehaviour,IComparer<liCraftableItem>
{
  public liCraftableItem(int in_resultingItem, int[] in_requiredItems)
  {
    resultingItemID = in_resultingItem;
    requiredItemIDs = (int[])in_requiredItems.Clone();
    Array.Sort(requiredItemIDs);
  }
  protected int resultingItemID;
  protected int[] requiredItemIDs;

  /// <summary>
  /// Sorts the Craftable items by the first element in their array.
  /// </summary>
  /// <param name="leftElem"></param>
  /// <param name="rightElem"></param>
  /// <returns>'0' when they are equal, '1' when leftElem is greater than rightElem,
  /// '-1' when rightElem is greater then leftElem </returns>
  public int Compare(liCraftableItem leftElem, liCraftableItem rightElem)
  {
    if (leftElem.requiredItemIDs[0] < rightElem.requiredItemIDs[0])
    {
      return 1;  
    }
    else if(leftElem.requiredItemIDs[0] > rightElem.requiredItemIDs[0])
    {
      return -1;
    }

      return 0;
  }

}
