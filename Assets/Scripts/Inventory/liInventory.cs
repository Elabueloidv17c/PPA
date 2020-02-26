using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

using Text = TMPro.TextMeshProUGUI;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#pragma warning disable CS0649

public class liInventory : MonoBehaviour
{
    static Item[] itemDataBase = null;

    static List<ItemInstance> currentItems = new List<ItemInstance>(); 

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

    bool isOpen;
    Transform mainPanel;
    
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
        }

        mainPanel = transform.GetChild(0);
        tabBtns = mainPanel.Find("Tabs").GetComponentsInChildren<Button>();

        for (int i = 0; i < tabBtns.Length; i++)
        {
            int index = i; // needed by lambda so it's capture by value
            tabBtns[i].onClick.AddListener(() => { TabBtnCallback(index); });
        }

        var background = mainPanel.Find("Background");
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

        TabBtnCallback(tabBtns.Length - 1);
    }

    void UpdateItemUI()
    {
        int index = 0;

        for (int i = 0; i < currentItems.Count; i++)
        {
            Item item = itemDataBase[currentItems[i].id];

            if(ItemTypeToTabIndex(item.type) == activeTabIndex)
            {
                itemSlots[index].image.color = Color.white;
                itemSlots[index].button.interactable = true;
                itemSlots[index].image.sprite = item.icon;
                if(currentItems[i].count != 1) {
                    itemSlots[index].text.text = currentItems[i].count.ToString();
                }
                else {
                    itemSlots[index].text.text = "";
                }
    
                itemSlots[index].itemID = currentItems[i].id;
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
        ItemInstance itemInst = currentItems[itemSlots[index].itemInstIndex];

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

    #if UNITY_EDITOR
    [SerializeField]
    int addItemID;

    [InspectorButton("AddItem")]
    public bool addItem;
    #endif

    public bool AddItem()
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
        
        if(addItemID < 0 || addItemID >= itemDataBase.Length) {
            Debug.LogWarning("Added Item ID out of Range.");
            return false;
        }
        #endif

        for(int i = 0; i < currentItems.Count ; ++i) {
            if(currentItems[i].id == addItemID) {
                currentItems[i].count++;
                currentItems[i].dateTimes.Add(DateTime.Now);

                if(ItemTypeToTabIndex(itemDataBase[addItemID].type) == activeTabIndex)
                {
                    UpdateItemUI();
                }

                return true;
            }
        }

        if(currentItems.Count >= 12) {
            Debug.LogWarning("All item slots are full.");
            return false;
        }

        var item = new ItemInstance();
        item.id = addItemID;
        item.dateTimes = new List<DateTime>();
        item.dateTimes.Add(DateTime.Now);
        item.count = 1;
        currentItems.Add(item);

        if(ItemTypeToTabIndex(itemDataBase[item.id].type) == activeTabIndex)
        {
            UpdateItemUI();
        }

        return true;
    }    

}

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