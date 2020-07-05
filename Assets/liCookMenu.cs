using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class liCookMenu : BaseUIManager
{
    GameObject m_mainPanel;
    List<liItemSlot> itemSlots;
    Transform itemSlotPanel;
    Button takeDepositBtn;
    int activeSlotIndex = -1;
    static Item[] s_itemDataBase = null;
    public static List<ItemInstance> s_currentItems;
    public static List<ItemInstance> s_depositItems;
    Text itemNameTxt;
    Image itemImg;
    Text itemDescTxt;
    int activeTabIndex = -1;
    Button[] tabBtns;
    Button[] depositBtns;
    Image backgroundImg;

    List<ItemInstance> currentItems
    {
        get
        {
            return depositMode == DepositMode.Active ?
                                  s_depositItems :
                                  s_currentItems;
        }
    }

    [SerializeField]
    DepositMode depositMode;

    [SerializeField]
    Color inactiveColor;

    [SerializeField]
    Color activeColor;

    [SerializeField]
    Color depositEnabledColor;

    [SerializeField]
    Color depositActiveColor;

    [SerializeField]
    Color inventoryColor;

    void Start()
    {
        m_mainPanel = transform.GetChild(0).gameObject;
        var background = m_mainPanel.transform.Find("Background");
        itemSlotPanel = background.GetChild(0).GetChild(0).GetChild(0);
        takeDepositBtn = m_mainPanel.transform.Find("Take Button").
                                GetComponentInChildren<Button>();
        var itemDetails = background.GetChild(1);
        itemNameTxt = itemDetails.Find("Item Name").GetComponent<Text>();
        itemImg = itemDetails.Find("Item Image").GetChild(0).GetComponent<Image>();
        var scrollView = itemDetails.Find("Item Description").GetChild(0);
        itemDescTxt = scrollView.Find("Viewport").GetChild(0).GetComponent<Text>();
        tabBtns = m_mainPanel.transform.Find("Tabs").GetComponentsInChildren<Button>();

        for (int i = 0; i < tabBtns.Length; i++)
        {
            int index = i; // needed by lambda so it's capture by value
            tabBtns[i].onClick.AddListener(() => { TabBtnCallback(index); });
        }

        depositBtns = m_mainPanel.transform.Find("Deposit Buttons").
                                GetComponentsInChildren<Button>();
        backgroundImg = background.GetComponent<Image>();
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
        for (int i = 0; i < 4; i++)
        {
            int index = itemSlots.Count; // needed by lambda so it's capture by value
            var GO = Instantiate(itemSlots[0].gameObject, itemSlotPanel);
            itemSlots.Add(GO.GetComponent<liItemSlot>());
            itemSlots[index].button.onClick.AddListener(() => {
                SlotBtnCallback(index);
            });
           
            itemSlots[index].image.color = Color.clear;
            itemSlots[index].text.text = "";
            itemSlots[index].button.interactable = false;
            itemSlots[index].itemID = -1;
            itemSlots[index].itemInstIndex = -1;
        }
    }

    int ItemTypeToTabIndex(ItemType type)
    {
        return tabBtns.Length - ((int)type + 1);
    }

    void DepositBtnsSetActive(bool active)
    {
        depositBtns[0].gameObject.SetActive(active);
        depositBtns[1].gameObject.SetActive(active);
    }

    void ClearActiveSlot()
    {
        activeSlotIndex = -1;
        itemNameTxt.text = "";
        itemImg.color = Color.clear;
        itemDescTxt.text = "";
        takeDepositBtn.interactable = false;
    }

    void UpdateItemUI()
    {
        if (!IsOpen || !IsMaximized) return;

        int index = 0;// index to iterate though all slots in the UI.

        for (int i = 0; i < currentItems.Count; i++)
        {
            Item item = s_itemDataBase[currentItems[i].id];

            if (ItemTypeToTabIndex(item.type) == activeTabIndex)
            {
                if (depositMode == DepositMode.Active &&
                   index >= itemSlots.Count)
                {
                    ExpandSlots();
                }

                itemSlots[index].image.color = Color.white;
                itemSlots[index].button.interactable = true;
                itemSlots[index].image.sprite = item.icon;
                if (currentItems[i].count != 1)
                {
                    itemSlots[index].text.text = currentItems[i].count.ToString();
                }
                else
                {
                    itemSlots[index].text.text = "";
                }

                itemSlots[index].itemID = currentItems[i].id;
                itemSlots[index].itemInstIndex = i;

                index++;
            }
        }

        /** Initialize empty item slots default values*/
        for (; index < itemSlots.Count; index++)
        {
            itemSlots[index].image.color = Color.clear;
            itemSlots[index].text.text = "";
            itemSlots[index].button.interactable = false;
            itemSlots[index].itemID = -1;
            itemSlots[index].itemInstIndex = -1;
        }

        switch (depositMode)
        {
            case DepositMode.Active:
                DepositBtnsSetActive(true);
                depositBtns[0].GetComponent<Image>().color = inactiveColor;
                depositBtns[0].GetComponentInChildren<Text>().color = activeColor;
                depositBtns[0].interactable = true;

                depositBtns[1].GetComponent<Image>().color = activeColor;
                depositBtns[1].GetComponentInChildren<Text>().color = inactiveColor;
                depositBtns[1].interactable = false;

                takeDepositBtn.gameObject.SetActive(true);

                backgroundImg.color = depositActiveColor;
                ShrinkSlots();
                break;

            case DepositMode.Enabled:
                DepositBtnsSetActive(true);
                depositBtns[0].GetComponent<Image>().color = activeColor;
                depositBtns[0].GetComponentInChildren<Text>().color = inactiveColor;
                depositBtns[0].interactable = false;

                depositBtns[1].GetComponent<Image>().color = inactiveColor;
                depositBtns[1].GetComponentInChildren<Text>().color = activeColor;
                depositBtns[1].interactable = true;

                takeDepositBtn.gameObject.SetActive(true);

                backgroundImg.color = depositEnabledColor;
                break;

            case DepositMode.Disabled:// how the inventory is normally used
                DepositBtnsSetActive(false);
                takeDepositBtn.gameObject.SetActive(false);

                backgroundImg.color = inventoryColor;
                break;
        }

        ClearActiveSlot();
    }

    public override void CloseUI()
    {
        ShrinkSlots();
        m_mainPanel.SetActive(false);
        IsOpen = false;
        IsMaximized = false;
        
        liGameManager.instance.RegisterCloseUI(this);
    }

    public override void MaximizeUI()
    {
        if (!IsOpen) { return; }
        IsMaximized = true;
        m_mainPanel.SetActive(true);
        
        UpdateItemUI();
    }

    public override void MinimizeUI()
    {
        if (!IsOpen) { return; }
        IsMaximized = false;
        m_mainPanel.SetActive(false);
    }

    void TabBtnCallback(int index)
    {
        Vector3 offset = Vector3.up * 20;

        if (activeTabIndex >= 0)
        {
            tabBtns[activeTabIndex].transform.localPosition -= offset;
            tabBtns[activeTabIndex].GetComponent<Image>().color = inactiveColor;
            tabBtns[activeTabIndex].GetComponentInChildren<Text>().color = activeColor;
            tabBtns[activeTabIndex].interactable = true;
        }

        activeTabIndex = index;
        tabBtns[index].transform.localPosition += offset;
        tabBtns[index].GetComponent<Image>().color = activeColor;
        tabBtns[index].GetComponentInChildren<Text>().color = inactiveColor;
        tabBtns[index].interactable = false;

        UpdateItemUI();
    }

    public override void OpenUI()
    {
        m_mainPanel.SetActive(true);
        IsOpen = true;
        IsMaximized = true;
        
        liGameManager.instance.RegisterOpenUI(this);
        
        if (activeTabIndex == -1)
        {
            TabBtnCallback(tabBtns.Length - 1);
        }
        UpdateItemUI();
    }

    void SlotBtnCallback(int index)
    {
        if (itemSlots[index].itemID < 0) { return; }

        activeSlotIndex = index;
        takeDepositBtn.interactable = true;

        Item item = s_itemDataBase[itemSlots[index].itemID];
        ItemInstance itemInst = currentItems[itemSlots[index].itemInstIndex];

        itemNameTxt.text = "Name: " + item.name;
        itemImg.color = Color.white;
        itemImg.sprite = item.largeImage;

        string text = "<size=40>Description:</size>\n" +
                           item.desc + "\n";

        foreach (var dateTime in itemInst.dateTimes)
        {
            text += " -" + dateTime + "\n";
        }

        itemDescTxt.text = text;
        Invoke("DelayScrollBarCorrection", 0.05f);
    }

}
