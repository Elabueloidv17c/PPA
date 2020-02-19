using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Text = TMPro.TextMeshProUGUI;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class liInventory : MonoBehaviour
{
    static Item[] itemDataBase = null;

    static List<ItemInstance> currentItems = new List<ItemInstance>(); 

    Button[] tabBtns;
    liItemSlot[] itemSlots;

    Text itemNameTxt;
    Image itemImg;
    Text itemDescTxt;

    int activeTabIndex = -1;

    int activeSlotIndex = -1;

    [SerializeField]
    Color inactiveColor;
    
    [SerializeField]
    Color activeColor;
    
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

                itemDataBase[i].sprite =
                    Resources.Load<Sprite>("Sprites/Items/" + itemDataBase[i].name);

                if(itemDataBase[i].sprite == null)
                {
                    Debug.LogError("Missing Items Sprite " + itemDataBase[i].name);
                }
            }
        }

        tabBtns = transform.GetChild(0).Find("Tabs")
                           .GetComponentsInChildren<Button>();

        for (int i = 0; i < tabBtns.Length; i++)
        {
            int index = i; // needed by lambda so it's capture by value
            tabBtns[i].onClick.AddListener(() => { TabBtnCallback(index); });
        }

        var background = transform.GetChild(0).Find("Background");
        itemSlots = background.GetChild(0).GetComponentsInChildren<liItemSlot>();

        for (int i = 0; i < itemSlots.Length; i++)
        {
            int index = i; // needed by lambda so it's capture by value
            itemSlots[i].button.onClick.AddListener(() => { SlotBtnCallback(index); });
        }

        var itemDetails = background.GetChild(1);
        itemNameTxt = itemDetails.Find("Item Name").GetComponent<Text>();
        itemImg = itemDetails.Find("Item Image").GetChild(0).GetComponent<Image>();
        itemDescTxt = itemDetails.Find("Item Description").GetChild(0).GetComponent<Text>();

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
                itemSlots[index].image.sprite = item.sprite;
    
                itemSlots[index].itemID = currentItems[i].id;
                itemSlots[index].itemInstIndex = i;
    
                index++;
            }
        }
        
        for (; index < itemSlots.Length; index++)
        {
            itemSlots[index].image.color = Color.clear;
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
        itemImg.sprite = item.sprite;
        itemDescTxt.text = "<size=40>Description:</size>\n" + 
                           item.desc + "\n" + 
                           itemInst.dateTime;
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

    void AddItem()
    {
        var item = new ItemInstance();
        item.id = addItemID;
        item.dateTime = DateTime.Now;
        currentItems.Add(item);

        if(ItemTypeToTabIndex(itemDataBase[item.id].type) == activeTabIndex)
        {
            UpdateItemUI();
        }
    }    

    #endif
}

public struct ItemInstance
{
    public int id;
    public DateTime dateTime;
}

public struct Item
{
    public int id;
    public string name;
    public ItemType type;
    public string desc;
    public Sprite sprite;
}

[JsonConverter(typeof(StringEnumConverter))]  
public enum ItemType {
    General,
    Recipies,
    Books,
    Photos,
    Notes
}