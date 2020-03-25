using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

using Text = TMPro.TextMeshProUGUI;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#pragma warning disable CS0649

public class liInventory : BaseUIManager
{
    public static liInventory instance;

    static Item[] itemDataBase = null;

    public static List<ItemInstance> m_currentItems;

    Button[] tabBtns;
    liItemSlot[] itemSlots;

    Text itemNameTxt;
    Image itemImg;
    Text itemDescTxt;
    Scrollbar descScrollbar;

    int activeTabIndex = -1;

    int activeSlotIndex = -1;

    [SerializeField]
    Color inactiveColor;
    
    [SerializeField]
    Color activeColor;

    GameObject mainPanel;

    void Awake() {
        instance = this;
    }
    
    void Start()
    {
        if(null == itemDataBase)
        {
            string data = File.ReadAllText(Application.streamingAssetsPath + "/Items.json");

            itemDataBase = JsonConvert.DeserializeObject<Item[]>(data);

            for (int i = 0; i < itemDataBase.Length; i++)
            {
                if(itemDataBase[i].id != i)
                {
                    Debug.LogWarning("Wrong item id assignment at index: " + i);
                    itemDataBase[i].id = i;
                }

                itemDataBase[i].icon =
                    Resources.Load<Sprite>("Sprites/Items/Icons/" + itemDataBase[i].name);

                if(itemDataBase[i].icon == null)
                {
                    Debug.LogError("Missing Items Icon " + itemDataBase[i].name);
                }

                itemDataBase[i].largeImage =
                    Resources.Load<Sprite>("Sprites/Items/Large/" + itemDataBase[i].name);

                if(itemDataBase[i].largeImage == null)
                {
                    Debug.LogWarning("Missing Items Large Image " + itemDataBase[i].name);
                    itemDataBase[i].largeImage = itemDataBase[i].icon;
                }
            }

            m_currentItems  = new List<ItemInstance>();         
        }

        mainPanel = transform.GetChild(0).gameObject;
        tabBtns = mainPanel.transform.Find("Tabs").GetComponentsInChildren<Button>();

        for (int i = 0; i < tabBtns.Length; i++)
        {
            int index = i; // needed by lambda so it's capture by value
            tabBtns[i].onClick.AddListener(() => { TabBtnCallback(index); });
        }

        var background = mainPanel.transform.Find("Background");
        itemSlots = background.GetChild(0).GetComponentsInChildren<liItemSlot>();

        for (int i = 0; i < itemSlots.Length; i++)
        {
            int index = i; // needed by lambda so it's capture by value
            itemSlots[i].button.onClick.AddListener(() => { SlotBtnCallback(index); });
        }

        var itemDetails = background.GetChild(1);
        itemNameTxt = itemDetails.Find("Item Name").GetComponent<Text>();
        itemImg = itemDetails.Find("Item Image").GetChild(0).GetComponent<Image>();
        var scrollView = itemDetails.Find("Item Description").GetChild(0);
        itemDescTxt = scrollView.Find("Viewport").GetChild(0).GetComponent<Text>();
        descScrollbar = scrollView.Find("Scrollbar Vertical").GetComponent<Scrollbar>();

        CloseUI();
    }

    public void Update()
    {
        if(liGameManager.instance && 
           !liGameManager.instance.menuActive &&
           Input.GetKeyDown(KeyCode.I))
        {
            OpenUI();
        }
        else if(IsOpen && IsMaximized && 
                (Input.GetKeyDown(KeyCode.Escape) || 
                 Input.GetKeyDown(KeyCode.I)))
        {
            CloseUI();
        }
    }

    public override void OpenUI()
    {
        mainPanel.SetActive(true);
        IsOpen = true;
        IsMaximized = true;

        liGameManager.instance.RegisterOpenUI(this);

        if(activeTabIndex == -1)
        {
            TabBtnCallback(tabBtns.Length - 1);
        }

        UpdateItemUI();
    }

    public override void CloseUI()
    {
        mainPanel.SetActive(false);
        IsOpen = false;
        IsMaximized = false;

        liGameManager.instance.RegisterCloseUI(this);
    }

    public override void MinimizeUI()
    {
        if(!IsOpen) { return; }
        IsMaximized = false;
        mainPanel.SetActive(false);
    }

    public override void MaximizeUI()
    {
        if(!IsOpen) { return; }
        IsMaximized = true;
        mainPanel.SetActive(true);
    }

    void UpdateItemUI()
    {
        if(!IsOpen) return;

        int index = 0;

        for (int i = 0; i < m_currentItems.Count; i++)
        {
            Item item = itemDataBase[m_currentItems[i].id];

            if(ItemTypeToTabIndex(item.type) == activeTabIndex)
            {
                itemSlots[index].image.color = Color.white;
                itemSlots[index].button.interactable = true;
                itemSlots[index].image.sprite = item.icon;
                if(m_currentItems[i].count != 1) {
                    itemSlots[index].text.text = m_currentItems[i].count.ToString();
                }
                else {
                    itemSlots[index].text.text = "";
                }
    
                itemSlots[index].itemID = m_currentItems[i].id;
                itemSlots[index].itemInstIndex = i;
    
                index++;
            }
        }
        
        for (; index < itemSlots.Length; index++)
        {
            itemSlots[index].image.color = Color.clear;
            itemSlots[index].text.text = "";
            itemSlots[index].button.interactable = false;
            itemSlots[index].itemID = -1;
            itemSlots[index].itemInstIndex = -1;
        }

        ClearActiveSlot();
    }

    void TabBtnCallback(int index)
    {
        Vector3 offset = Vector3.up * 20;

        if(activeTabIndex >= 0)
        {
            tabBtns[activeTabIndex].transform.localPosition -= offset;
            tabBtns[activeTabIndex].GetComponent<Image>().color = inactiveColor;
            tabBtns[activeTabIndex].GetComponentInChildren<Text>().color = activeColor;
        }

        activeTabIndex = index;
        tabBtns[index].transform.localPosition += offset;
        tabBtns[index].GetComponent<Image>().color = activeColor;
        tabBtns[index].GetComponentInChildren<Text>().color = inactiveColor;

        UpdateItemUI();
    }

    void SlotBtnCallback(int index)
    {
        if(itemSlots[index].itemID < 0) { return; }

        activeSlotIndex = index;

        Item item = itemDataBase[itemSlots[index].itemID];
        ItemInstance itemInst = m_currentItems[itemSlots[index].itemInstIndex];

        itemNameTxt.text = "Name: " + item.name;
        itemImg.color = Color.white;
        itemImg.sprite = item.largeImage;

        string text = "<size=40>Description:</size>\n" + 
                           item.desc + "\n";
        
        foreach(var dateTime in itemInst.dateTimes)
        {
            text += " -" + dateTime + "\n";
        }

        itemDescTxt.text = text;
        Invoke("DelayScrollBarCorrection", 0.05f);
    }

    void DelayScrollBarCorrection()
    {
        descScrollbar.value = 1;
    }

    void ClearActiveSlot()
    {
        activeSlotIndex = -1;
        itemNameTxt.text = "";
        itemImg.color = Color.clear;
        itemDescTxt.text = "";
    }

    int ItemTypeToTabIndex(ItemType type)
    {
        return tabBtns.Length - ((int)type + 1);
    }

    public bool AddItem(int itemID)
    {
        #if UNITY_EDITOR
        if(!Application.isPlaying) {
            Debug.LogWarning("Calling method without running the game... you fool.");
            return false;
        }
        
        if(gameObject.IsPrefab()) {
            Debug.LogWarning("Calling method from prefab... you fool.");
            return false;
        }
        
        if(itemID < 0 || itemID >= itemDataBase.Length) {
            Debug.LogWarning("Added Item ID out of Range.");
            return false;
        }
        #endif

        for(int i = 0; i < m_currentItems.Count ; ++i) {
            if(m_currentItems[i].id == itemID) {
                m_currentItems[i].count++;
                m_currentItems[i].dateTimes.Add(DateTime.Now);

                if(ItemTypeToTabIndex(itemDataBase[itemID].type) == activeTabIndex)
                {
                    UpdateItemUI();
                }

                return true;
            }
        }

        if(m_currentItems.Count >= 12) {
            Debug.LogWarning("All item slots are full.");
            return false;
        }

        var item = new ItemInstance();
        item.id = itemID;
        item.dateTimes = new List<DateTime>();
        item.dateTimes.Add(DateTime.Now);
        item.count = 1;
        m_currentItems.Add(item);

        if(ItemTypeToTabIndex(itemDataBase[item.id].type) == activeTabIndex)
        {
            UpdateItemUI();
        }

        return true;
    }    

    public bool HasItem(int itemID) {

        #if UNITY_EDITOR
        if(itemID < 0 || itemID >= itemDataBase.Length) {
            Debug.LogWarning("Check Item ID out of Range.");
            return false;
        }
        #endif

        foreach(var itemInst in m_currentItems) {
            if(itemInst.id == itemID) {
                return true;
            }
        }

        return false;
    }

    public bool RemoveItem(int itemID) {
        #if UNITY_EDITOR
        if(!Application.isPlaying) {
            Debug.LogWarning("Calling method without running the game... you fool.");
            return false;
        }
        
        if(gameObject.IsPrefab()) {
            Debug.LogWarning("Calling method from prefab... you fool.");
            return false;
        }
        
        if(itemID < 0 || itemID >= itemDataBase.Length) {
            Debug.LogWarning("Remove Item ID out of Range.");
            return false;
        }
        #endif

        foreach(var itemInst in m_currentItems) {
            if(itemInst.id == itemID) {
                if(itemInst.count <= 1) {
                    m_currentItems.Remove(itemInst);
                }
                else {
                    itemInst.count--;
                }

                if(IsOpen) {
                    UpdateItemUI();
                }

                return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    [SerializeField]
    int itemID;

    [InspectorButton("InspectorAddItem")]
    public bool addItem;

    private void InspectorAddItem() {
        AddItem(itemID);
    }

    [InspectorButton("InspectorRemoveItem")]
    public bool removeItem;

    private void InspectorRemoveItem() {
        RemoveItem(itemID);
    }
#endif

}

[Serializable]
public class ItemInstance
{
    public int id;
    public int count;
    public List<DateTime> dateTimes;
}

public struct Item
{
    public int id;
    public string name;
    public ItemType type;
    public string desc;
    public Sprite icon;
    public Sprite largeImage;
}

[JsonConverter(typeof(StringEnumConverter))]  
public enum ItemType {
    General,
    Recipies,
    Books,
    Photos,
    Notes
}