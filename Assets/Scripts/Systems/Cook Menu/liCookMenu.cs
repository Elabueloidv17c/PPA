using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using Text = TMPro.TextMeshProUGUI;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.InteropServices.ComTypes;

public class liCookMenu : BaseUIManager
{
    public static liCookMenu instance;

    GameObject m_mainPanel;

    GameObject m_craftPanel;

    GameObject m_resultPanel;

    GameObject m_cookButton;

    List<liItemSlot> itemSlots;

    List<liItemSlot> craftPanelSlots;

    Transform itemSlotPanel;

    int activeSlotIndex = -1;

    int activeTabIndex = -1;

    int activeCraftPanelSlotIndex = -1;

    Text itemNameTxt;

    Text itemDescTxt;

    Button[] tabBtns;

    Button[] itemSlotBtns;

    Image backgroundImg;

    Image itemImg;

    [SerializeField]
    Color inactiveColor;

    [SerializeField]
    Color activeColor;

    [SerializeField]
    Color inventoryColor;

    void Awake()
    {
        instance = this;
    }

    void Start()
  {
    inactiveColor = Color.white;
    activeColor = Color.white;
    inventoryColor = Color.white;

    m_mainPanel = transform.GetChild(0).gameObject;
    m_craftPanel = transform.GetChild(1).gameObject;
    m_resultPanel = transform.GetChild(2).gameObject;
    m_cookButton = transform.GetChild(3).gameObject;

    tabBtns = m_mainPanel.transform.Find("Tabs").GetComponentsInChildren<Button>();
    var background = m_mainPanel.transform.Find("Background");
    itemSlotPanel = background.GetChild(0);
    itemSlots = itemSlotPanel.GetComponentsInChildren<liItemSlot>().ToList<liItemSlot>();

    var itemDetails = background.GetChild(1);
    itemNameTxt = itemDetails.Find("Item Name").GetComponent<Text>();
    itemImg = itemDetails.Find("Item Image").GetChild(0).GetComponent<Image>();
    var scrollView = itemDetails.Find("Item Description").GetChild(0);
    itemDescTxt = scrollView.Find("Viewport").GetChild(0).GetComponent<Text>();

    //tabBtns = m_mainPanel.transform.Find("Tabs").GetComponentsInChildren<Button>();

    var craftPanelBackground = m_craftPanel.GetChildWithName("Background");
    craftPanelSlots = craftPanelBackground.GetComponentsInChildren<liItemSlot>().ToList();
    foreach (liItemSlot slot in craftPanelSlots )
    {
      slot.itemID = -1;
    }

    for (int i = 0; i < tabBtns.Length; i++)
    {
      int index = i; // needed by lambda so it's capture by value
      tabBtns[i].onClick.AddListener(() => { TabBtnCallback(index); });
    }

    for (int i = 0; i < itemSlots.Count; i++)
    {
      int index = i; // needed by lambda so it's capture by value
      itemSlots[i].button.onClick.AddListener(() => { SlotBtnCallback(index); });
    }

    for(int i = 0; i < craftPanelSlots.Count; i++)
    {
      int index = i; // needed by lambda so it's capture by value
      craftPanelSlots[i].button.onClick.AddListener(() => { craftPanelBtnCallBack(index); });
    }
    //depositBtns = m_mainPanel.transform.Find("Deposit Buttons").
    //                        GetComponentsInChildren<Button>();
    //backgroundImg = background.GetComponent<Image>();
    OpenUI();
    CloseUI();
  }

  // Update is called once per frame
  void Update()
  {
        if (liGameManager.instance &&
            !liGameManager.instance.menuActive &&
            Input.GetKeyDown(KeyCode.C))
        {
            OpenUI();
        }
        else if (IsOpen && IsMaximized &&
                (Input.GetKeyDown(KeyCode.Escape) ||
                 Input.GetKeyDown(KeyCode.C)))
        {
            CloseUI();
        }
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
    m_craftPanel.SetActive(false);
    m_resultPanel.SetActive(false);
    m_cookButton.SetActive(false);

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

    //Debug.Log("Works");
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

    clearsItemSlots();

    AddItemsToItemSlots(index);

    UpdateItemUI();

  }


  private void AddItemsToItemSlots(int index)
  {
    SubType currentSubType = SubType.SOMETHING_IS_WRONG;

    switch (index)
    {
      case 0:
        currentSubType = SubType.Fish;
        break;
      case 1:
        currentSubType = SubType.Condiment;
        break;
      case 2:
        currentSubType = SubType.Garnish;
        break;
    }

    if (SubType.SOMETHING_IS_WRONG == currentSubType)
    {
      Debug.LogWarning(" Trying to access non existent or non accounted for tab ");
      return;
    }


    List<int> ValidListOfItemIDs = new List<int>();

    var inventory = liInventory.instance;
    var playerItems = liInventory.s_currentItems;
    foreach (var item in playerItems)
    {
      if (currentSubType == inventory.GetItemSubType(item.id))
      {
        ValidListOfItemIDs.Add(item.id);
      }
    }

    for (int i = 0; i < ValidListOfItemIDs.Count; ++i)
    {
      int validID = ValidListOfItemIDs[i];
      Item possibleValidItem = liInventory.instance.GetItemById(validID);
      if (-1 != possibleValidItem.id)
      {
        itemSlots[i].image.color = Color.white;
        itemSlots[i].button.interactable = true;
        itemSlots[i].image.sprite = possibleValidItem.icon;
        itemSlots[i].itemID = possibleValidItem.id;
      }
    }
  }

  private void clearsItemSlots()
  {

    foreach (var itemSlot in itemSlots)
    {
      itemSlot.image.color = Color.clear;
      itemSlot.button.interactable = true;
      itemSlot.image.sprite = null;
      itemSlot.itemID = -1;
    }

  }


  private void clearActiveCraftPanelSlot(int index)
  {
    craftPanelSlots[index].image.color = Color.clear;
    craftPanelSlots[index].itemID = -1;
    craftPanelSlots[index].button.interactable = true;
    craftPanelSlots[index].text.text = "";
  }
  public override void OpenUI()
    {
        m_mainPanel.SetActive(true);
        m_craftPanel.SetActive(true);
        m_resultPanel.SetActive(false); //Needs a condition to open
        m_cookButton.SetActive(true);

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
    Item item = liInventory.instance.GetItemById(itemSlots[index].itemID);

    itemNameTxt.text = "Name: " + item.name;
    itemImg.color = Color.white;
    itemImg.sprite = item.largeImage;

    string text = "<size=40>Description:</size>\n" +
                       item.desc + "\n";

    itemDescTxt.text = text;

    foreach (liItemSlot slot in craftPanelSlots )
    {
      if (-1 == slot.itemID)
      {
        slot.itemID = item.id;
        slot.image.sprite = item.icon;
        slot.image.color = Color.white;
        slot.text = itemNameTxt;

        break;
      }

    }

    Invoke("DelayScrollBarCorrection", 0.05f);
  }

  void craftPanelBtnCallBack(int index)
  {
    activeCraftPanelSlotIndex = index;
    clearActiveCraftPanelSlot(index);
  }

}
