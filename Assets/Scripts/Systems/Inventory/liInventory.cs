using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

using Text = TMPro.TextMeshProUGUI;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#pragma warning disable CS0649

public class liInventory : BaseUIManager
{
    #region ItemData
    public static liInventory instance;

    static Item[] s_itemDataBase = null;

    public static List<ItemInstance> s_currentItems;
    public static List<ItemInstance> s_depositItems;

    List<ItemInstance> currentItems {
        get {
            return depositMode == DepositMode.Active ? 
                                  s_depositItems : 
                                  s_currentItems;
        }
    }

    List<ItemInstance> hiddenItems {
        get {
            return depositMode == DepositMode.Active ? 
                                  s_currentItems : 
                                  s_depositItems;
        }
    }

    #endregion

    #region StateData
    int activeTabIndex = -1;

    int activeSlotIndex = -1;

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

    [SerializeField]
    DepositMode depositMode;

    #endregion

    #region UIObjects

    Button[] tabBtns;
    List<liItemSlot> itemSlots;

    Text itemNameTxt;
    Image itemImg;
    Text itemDescTxt;
    Scrollbar descScrollbar;

    GameObject mainPanel;

    Transform itemSlotPanel;

    Button[] depositBtns;

    Button takeDepositBtn;
    Image backgroundImg;

    #endregion
    /// <summary>
    /// Good old fashion single-ton.
    /// </summary>
    void Awake() {
        instance = this;
    }
    
    /// <summary>
    ///  Initializes all the items and makes sure non of them are out 
    ///  of place or have wrong information.
    /// </summary>
    void Start()
    {
        if(null == s_itemDataBase)
        {
            string data = File.ReadAllText(Application.streamingAssetsPath + "/Items.json");

            s_itemDataBase = JsonConvert.DeserializeObject<Item[]>(data);

            for (int i = 0; i < s_itemDataBase.Length; i++)
            {
                if(s_itemDataBase[i].id != i)
                {
                    Debug.LogWarning("Wrong item id assignment at index: " + i);
                    s_itemDataBase[i].id = i;
                }

                s_itemDataBase[i].icon =
                    Resources.Load<Sprite>("Sprites/Items/Icons/" + s_itemDataBase[i].name);

                if(s_itemDataBase[i].icon == null)
                {
                    Debug.LogError("Missing Items Icon " + s_itemDataBase[i].name);
                }

                s_itemDataBase[i].largeImage =
                    Resources.Load<Sprite>("Sprites/Items/Large/" + s_itemDataBase[i].name);

                if(s_itemDataBase[i].largeImage == null)
                {
                    Debug.LogWarning("Missing Items Large Image " + s_itemDataBase[i].name);
                    s_itemDataBase[i].largeImage = s_itemDataBase[i].icon;
                }
            }

            s_currentItems  = new List<ItemInstance>();
            s_depositItems  = new List<ItemInstance>();            
        }

        mainPanel = transform.GetChild(0).gameObject;
        tabBtns = mainPanel.transform.Find("Tabs").GetComponentsInChildren<Button>();

        for (int i = 0; i < tabBtns.Length; i++)
        {
            int index = i; // needed by lambda so it's capture by value
            tabBtns[i].onClick.AddListener(() => { TabBtnCallback(index); });
        }

        /* Initializes the UI for the inventory */
        var background = mainPanel.transform.Find("Background");
        backgroundImg = background.GetComponent<Image>();
        itemSlotPanel = background.GetChild(0).GetChild(0).GetChild(0);
        itemSlots = itemSlotPanel.GetComponentsInChildren<liItemSlot>().ToList();

        depositBtns = mainPanel.transform.Find("Deposit Buttons").
                                GetComponentsInChildren<Button>();

        takeDepositBtn = mainPanel.transform.Find("Take Button").
                                GetComponentInChildren<Button>();

        for (int i = 0; i < itemSlots.Count; i++)
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

        OpenUI();
        CloseUI();
    }

    /// <summary>
    ///  Updates every frame
    /// </summary>
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

    /// <summary>
    ///  Opens the UI in deposit mode.
    /// </summary>
    public void OpenUIDepositMode()
    {
        mainPanel.SetActive(true);
        IsOpen = true;
        IsMaximized = true;

        liGameManager.instance.RegisterOpenUI(this);

        if(activeTabIndex == -1)
        {
            TabBtnCallback(tabBtns.Length - 1);
        }

        depositMode = DepositMode.Active;
        UpdateItemUI();
    }

    /// <summary>
    /// Opens the inventory UI.
    /// </summary>
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

        depositMode = DepositMode.Disabled;
        UpdateItemUI();
    }


    /// <summary>
    /// Closes the inventory UI.
    /// </summary>
    public override void CloseUI()
    {
        if(depositMode == DepositMode.Active) {
            ShrinkSlots();
        }

        mainPanel.SetActive(false);
        IsOpen = false;
        IsMaximized = false;

        liGameManager.instance.RegisterCloseUI(this);
    }

    /// <summary>
    /// Hides the dialog manager UI without closing.
    /// </summary>
    public override void MinimizeUI()
    {
        if(!IsOpen) { return; }
        IsMaximized = false;
        mainPanel.SetActive(false);
    }

    /// <summary>
    /// Unhides the dialog manager UI.
    /// </summary>
    public override void MaximizeUI()
    {
        if(!IsOpen) { return; }
        IsMaximized = true;
        mainPanel.SetActive(true);

        UpdateItemUI();
    }

    /// <summary>
    /// Refreshes the UI visual elements to match the internal data.
    /// </summary>
    void UpdateItemUI()
    {
        if(!IsOpen || !IsMaximized) return;

        int index = 0;// index to iterate though all slots in the UI.

        for (int i = 0; i < currentItems.Count; i++)
        {
            Item item = s_itemDataBase[currentItems[i].id];

            if(ItemTypeToTabIndex(item.type) == activeTabIndex)
            {
                if(depositMode == DepositMode.Active && 
                   index >= itemSlots.Count)
                {
                    ExpandSlots();
                }

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
       
        /** Initialize empty item slots default values*/
        for (; index < itemSlots.Count; index++)
        {
            itemSlots[index].image.color = Color.clear;
            itemSlots[index].text.text = "";
            itemSlots[index].button.interactable = false;
            itemSlots[index].itemID = -1;
            itemSlots[index].itemInstIndex = -1;
        }

        switch(depositMode)
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

    /// <summary>
    ///  Reduces the amount of item slot's and makes sure 
    ///  there is a minimum of 12 available.
    /// </summary>
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

        if(slotCount < itemSlots.Count)
        {
            for (int i = itemSlots.Count - 1; i >= 0 ; i--)
            {
                if(i >= slotCount)
                {
                    Destroy(itemSlots[i].gameObject);
                }
            }

            itemSlots.RemoveRange(slotCount, itemSlots.Count - slotCount);
        }
    }

    /// <summary>
    ///  Expands the available item slot's by 4.
    /// </summary>
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

    /// <summary>
    /// Controls what each tab button does every time it's used.
    /// </summary>
    /// <param name="index">the index of the particular button to use</param>
    void TabBtnCallback(int index)
    {
        Vector3 offset = Vector3.up * 20;

        if(activeTabIndex >= 0)
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

    /// <summary>
    ///  Controls what each slot button does when used.
    /// </summary>
    /// <param name="index">the index of the particular button to use</param>
    void SlotBtnCallback(int index)
    {
        if(itemSlots[index].itemID < 0) { return; }

        activeSlotIndex = index;
        takeDepositBtn.interactable = true;

        Item item = s_itemDataBase[itemSlots[index].itemID];
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

    /// <summary>
    /// Makes it so NO item slot is currently selected.
    /// </summary>
    void ClearActiveSlot()
    {
        activeSlotIndex = -1;
        itemNameTxt.text = "";
        itemImg.color = Color.clear;
        itemDescTxt.text = "";
        takeDepositBtn.interactable = false;
    }

    /// <summary>
    /// Convert the value of ItemType to it's equivalent index in the
    /// tab buttons array.
    /// </summary>
    /// <param name="type"></param>
    /// <returns> A index into an tab slot </returns>
    int ItemTypeToTabIndex(ItemType type)
    {
        return tabBtns.Length - ((int)type + 1);
    }

    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns>'true' if it succeeds to add an item,
    /// returns 'false' otherwise </returns>
    public bool AddItem(int itemID)
    {
        #if UNITY_EDITOR // This code is ONLY for debugging in the editor.
        if(itemID < 0 || itemID >= s_itemDataBase.Length) {
            Debug.LogWarning("Added Item ID out of Range.");
            return false;
        }
        #endif
        
        var itemType = s_itemDataBase[itemID].type;

        // update the count of the item
        for(int i = 0; i < currentItems.Count ; ++i) {
            if(currentItems[i].id == itemID) {
                currentItems[i].count++;
                currentItems[i].dateTimes.Add(DateTime.Now);

                if(ItemTypeToTabIndex(itemType) == activeTabIndex)
                {
                    UpdateItemUI();
                }

                return true;
            }
        }

        if(depositMode != DepositMode.Active)
        {
            var list = s_currentItems.Where(
                x => s_itemDataBase[x.id].type == itemType
            );

            if(list.Count() >= 12)
            {
                Debug.LogWarning("Item Slots are full.");
                return false;
            }
        }

        var item = new ItemInstance();
        item.id = itemID;
        item.dateTimes = new List<DateTime>();
        item.dateTimes.Add(DateTime.Now);
        item.count = 1;
        currentItems.Add(item);

        if(ItemTypeToTabIndex(itemType) == activeTabIndex)
        {
            UpdateItemUI();
        }

        return true;
    }

    /// <summary>
    ///  Used to find-out if the inventory has a particular item.
    /// </summary>
    /// <param name="itemID">The id of the item to check for.</param>
    /// <returns>'true' if it has the item otherwise returns 'false' </returns>
    public bool HasItem(int itemID) {

        #if UNITY_EDITOR// This code is ONLY for debugging in the editor. 
        if(itemID < 0 || itemID >= s_itemDataBase.Length) {
            Debug.LogWarning("Check Item ID out of Range.");
            return false;
        }
        #endif

        foreach(var itemInst in currentItems) {
            if(itemInst.id == itemID) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Remove an item from the inventory if there is one.
    /// </summary>
    /// <param name="itemID">Which type of item to remove</param>
    /// <returns>'true' if an item was removed, returns false otherwise. </returns>
    public bool RemoveItem(int itemID) {
        #if UNITY_EDITOR // This code is ONLY for debugging in the editor.
        if(itemID < 0 || itemID >= s_itemDataBase.Length) {
            Debug.LogWarning("Remove Item ID out of Range.");
            return false;
        }
        #endif

        foreach(var itemInst in currentItems) {
            if(itemInst.id == itemID) {
                if(itemInst.count <= 1) {
                    currentItems.Remove(itemInst);
                }
                else {
                    itemInst.dateTimes.RemoveAt(0);
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

    /// <summary>
    /// Controls if the buttons for depositing are active or not.
    /// <param name="active">Used to Activate or Deactivate the buttons.</param>
    /// </summary>
    void DepositBtnsSetActive(bool active)
    {
        depositBtns[0].gameObject.SetActive(active);
        depositBtns[1].gameObject.SetActive(active);
    }

    /// <summary>
    /// Set's the deposit mode to 'Active'.
    /// </summary>
    public void SetDepositModeActive() {
        depositMode = DepositMode.Active;
        if(IsOpen && IsMaximized) {
            UpdateItemUI();
        }
    }

    /// <summary>
    /// Set's the deposit mode to 'Enabled'.
    /// </summary>
    public void SetDepositModeEnabled() {
        depositMode = DepositMode.Enabled;
        if(IsOpen && IsMaximized) {
            UpdateItemUI();
        }
    }

    /// <summary>
    /// Set's the deposit mode to 'Disabled'.
    /// </summary>
    public void SetDepositModeDisabled() {
        depositMode = DepositMode.Disabled;
        if(IsOpen && IsMaximized) {
            UpdateItemUI();
        }
    }

    /// <summary>
    /// Adds an item from the deposit box to the inventory 
    /// assuming one exist and the inventory has room for that item.
    /// </summary>
    public void TakeDepositItem() {
        int slotIndex = activeSlotIndex;
        Item item = s_itemDataBase[itemSlots[slotIndex].itemID];
        ItemInstance itemInst = currentItems[itemSlots[slotIndex].itemInstIndex];
        DateTime dateTime = itemInst.dateTimes[0];
        bool itemAdded = false;

        for(int i = 0; i < hiddenItems.Count ; ++i) {
            if(hiddenItems[i].id == item.id) {
                hiddenItems[i].count++;
                hiddenItems[i].dateTimes.Add(dateTime);

                itemAdded = true;
                break;
            }
        }

        if(!itemAdded)
        {
            if(depositMode == DepositMode.Active)
            {
                var list = hiddenItems.Where(
                    x => s_itemDataBase[x.id].type == item.type
                );

                if(list.Count() >= 12)
                {
                    Debug.LogWarning("Item Slots are full.");
                    return;
                }
            }

            var newitemInst = new ItemInstance();
            newitemInst.id = item.id;
            newitemInst.dateTimes = new List<DateTime>();
            newitemInst.dateTimes.Add(dateTime);
            newitemInst.count = 1;
            hiddenItems.Add(newitemInst);
        }

        if(itemInst.count <= 1) {
            currentItems.Remove(itemInst);
            UpdateItemUI();
        }
        else {
            itemInst.dateTimes.RemoveAt(0);
            itemInst.count--;
            UpdateItemUI();
            SlotBtnCallback(slotIndex);
        }
    }

#if UNITY_EDITOR // This code is ONLY for debugging in the editor.
    [SerializeField]
    int itemID;

    [InspectorButton("InspectorAddItem")]
    public bool addItem;

    /// <summary>
    /// Let's the user add an item to the inventory using the inspector
    /// </summary>
    private void InspectorAddItem() {
        if(!Application.isPlaying) {
            Debug.LogWarning("Calling method without running the game... you fool.");
            return;
        }
        
        if(gameObject.IsPrefab()) {
            Debug.LogWarning("Calling method from prefab... you fool.");
            return;
        }
        
        AddItem(itemID);
    }

    [InspectorButton("InspectorRemoveItem")]
    public bool removeItem;

    /// <summary>
    /// Let's the user remove an item to the inventory using the inspector
    /// </summary>
    private void InspectorRemoveItem() {
        if(!Application.isPlaying) {
            Debug.LogWarning("Calling method without running the game... you fool.");
            return;
        }
        
        if(gameObject.IsPrefab()) {
            Debug.LogWarning("Calling method from prefab... you fool.");
            return;
        }
        
        RemoveItem(itemID);
    }

    [InspectorButton("InspectorExpandSlots")]
    public bool expandSlots;

    /// <summary>
    /// Let's the user give more item slots to the inventory.
    /// </summary>
    private void InspectorExpandSlots() {
        if(!Application.isPlaying) {
            Debug.LogWarning("Calling method without running the game... you fool.");
            return;
        }
        
        if(gameObject.IsPrefab()) {
            Debug.LogWarning("Calling method from prefab... you fool.");
            return;
        }
        
        ExpandSlots();
    }

    [InspectorButton("InspectorShrinkSlots")]
    public bool shrinkSlots;

    /// <summary>
    /// Let's the user reduce the amount of item slot .
    /// </summary>
    private void InspectorShrinkSlots() {
        if(!Application.isPlaying) {
            Debug.LogWarning("Calling method without running the game... you fool.");
            return;
        }
        
        if(gameObject.IsPrefab()) {
            Debug.LogWarning("Calling method from prefab... you fool.");
            return;
        }
        
        ShrinkSlots();
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

public enum DepositMode
{
    Disabled,// default 
    Enabled,// you can see your items and put them in the deposit box.
    Active// you can pass the items in the deposit box to your inventory.
}
