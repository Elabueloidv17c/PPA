using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class liCookMenu : BaseUIManager
{
    GameObject m_mainPanel;
    List<liItemSlot> itemSlots;

    void Start()
    {
        m_mainPanel = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShrinkSlots()
    {
        int index;
        for (index = 0; index < itemSlots.Count; index++)
        {
            if (itemSlots[index].itemID == -1)
            {
                break;
            }
        }

        int slotCount = Math.Max(4 - index % 4, 12);

        if (slotCount < itemSlots.Count)
        {
            for (int i = itemSlots.Count - 1; i >= 0; i--)
            {
                if (i >= slotCount)
                {
                    Destroy(itemSlots[i].gameObject);
                }
            }

            itemSlots.RemoveRange(slotCount, itemSlots.Count - slotCount);
        }
    }

    private void ExpandSlots()
    {
        //for (int i = 0; i < 4; i++)
        //{
        //    int index = itemSlots.Count; // needed by lambda so it's capture by value
        //    var GO = Instantiate(itemSlots[0].gameObject, itemSlotPanel);
        //    itemSlots.Add(GO.GetComponent<liItemSlot>());
        //    itemSlots[index].button.onClick.AddListener(() => {
        //        SlotBtnCallback(index);
        //    });

        //    itemSlots[index].image.color = Color.clear;
        //    itemSlots[index].text.text = "";
        //    itemSlots[index].button.interactable = false;
        //    itemSlots[index].itemID = -1;
        //    itemSlots[index].itemInstIndex = -1;
        //}
    }

    public override void CloseUI()
    {
        //ShrinkSlots();
        //mainPanel.SetActive(false);
        //IsOpen = false;
        //IsMaximized = false;

        //liGameManager.instance.RegisterCloseUI(this);
    }

    public override void MaximizeUI()
    {
        //if (!IsOpen) { return; }
        //IsMaximized = true;
        //mainPanel.SetActive(true);

        //UpdateItemUI();
    }

    public override void MinimizeUI()
    {
        //if (!IsOpen) { return; }
        //IsMaximized = false;
        //mainPanel.SetActive(false);
    }

    public override void OpenUI()
    {
        //mainPanel.SetActive(true)
        //IsOpen = true;
        //IsMaximized = true;

        //liGameManager.instance.RegisterOpenUI(this);

        //if (activeTabIndex == -1)
        //{
        //    TabBtnCallback(tabBtns.Length - 1);
        //}
        //UpdateItemUI();
    }
}
