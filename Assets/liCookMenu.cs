using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Text = TMPro.TextMeshProUGUI;

public class liCookMenu : BaseUIManager
{
    GameObject m_mainPanel;

    List<liItemSlot> itemSlots;

    Transform itemSlotPanel;

    int activeSlotIndex = -1;

    Text itemNameTxt;

    Image itemImg;

    Text itemDescTxt;

    int activeTabIndex = -1;

    Button[] tabBtns;

    Button[] itemSlotBtns;
  
    Image backgroundImg;

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
    tabBtns = m_mainPanel.transform.Find("Tabs").GetComponentsInChildren<Button>();
    var background = m_mainPanel.transform.Find("Background");
    itemSlotPanel = background.GetChild(0);
    itemSlots = itemSlotPanel.GetComponentsInChildren<liItemSlot>().ToList<liItemSlot>();

    //var itemDetails = background.GetChild(1);
    //itemNameTxt = itemDetails.Find("Item Name").GetComponent<Text>();
    //itemImg = itemDetails.Find("Item Image").GetChild(0).GetComponent<Image>();
    //var scrollView = itemDetails.Find("Item Description").GetChild(0);
    //itemDescTxt = scrollView.Find("Viewport").GetChild(0).GetComponent<Text>();
    //tabBtns = m_mainPanel.transform.Find("Tabs").GetComponentsInChildren<Button>();

    for (int i = 0; i < tabBtns.Length; i++)
    {
      int index = i; // needed by lambda so it's capture by value
      tabBtns[i].onClick.AddListener(() => { TabBtnCallback(index); });
    }

    //depositBtns = m_mainPanel.transform.Find("Deposit Buttons").
    //                        GetComponentsInChildren<Button>();
    //backgroundImg = background.GetComponent<Image>();

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
        itemSlotBtns[0].gameObject.SetActive(active);
        itemSlotBtns[1].gameObject.SetActive(active);
    }

    void ClearActiveSlot()
    {
        activeSlotIndex = -1;
        itemNameTxt.text = "";
        itemImg.color = Color.clear;
        itemDescTxt.text = "";
    }

    void UpdateItemUI()
    {

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
            Text textt = tabBtns[activeTabIndex].GetComponentInChildren<Text>();
            textt.color = activeColor;
            tabBtns[activeTabIndex].interactable = true;
        }

        activeTabIndex = index;
        tabBtns[index].transform.localPosition += offset;
        tabBtns[index].GetComponent<Image>().color = activeColor;
        Text text = tabBtns[index].GetComponentInChildren<Text>();
        text.color = inactiveColor;
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

        Invoke("DelayScrollBarCorrection", 0.05f);
    }

}
