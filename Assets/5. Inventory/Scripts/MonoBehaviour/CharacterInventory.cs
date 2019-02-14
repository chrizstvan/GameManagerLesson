using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInventory : MonoBehaviour
{
    #region Variable Declarations
    public static CharacterInventory instance;

    public CharacterStats charStats;

    public Image[] hotBarDisplayHolders = new Image[4];
    public GameObject inventoryDisplayHolder;
    public Image[] inventoryDisplaySlots = new Image[30];

    int inventoryItemCap = 20; //number of item to carry
    int idCount = 1; //set for any id for dictionary
    bool addedItem = true;

    public Dictionary<int, InventoryEntry> itemInInventory = new Dictionary<int, InventoryEntry>(); //storage inventory
    public InventoryEntry itemEntry;
    #endregion

    #region Initializations
    void Start()
    {
        instance = this;

        itemEntry = new InventoryEntry(0, null, null);
        itemInInventory.Clear();

        inventoryDisplaySlots = inventoryDisplayHolder.GetComponentsInChildren<Image>();

        charStats = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>();
    }
    #endregion

    private void Update()
    {
        #region Watch fot Hot Keypress - called by character controller Latter
        //Checking for hotbar key to be pressed
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TriggerItemUse(101);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TriggerItemUse(102);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TriggerItemUse(103);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TriggerItemUse(104);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            DisplayInventory();
        }
        #endregion

        //Check to see if the item has already been added - Prevent duplicate adds for 1 item
        if (!addedItem)
        {
            TryPickUp();
        }
    }

    public void StoreItem(ItemPickUp itemToStore)
    {
        addedItem = false;

        if ((charStats.characterDefinition.currentEncumbrance + itemToStore.itemDefinition.itemWeight) <= charStats.characterDefinition.maxEncumbrance)
        {
            itemEntry.invEntry = itemToStore;
            itemEntry.stackSize = 1;
            itemEntry.hbSprite = itemToStore.itemDefinition.itemIcon;

            itemToStore.gameObject.SetActive(false);
        }
    }

    void TryPickUp()
    {
        bool itsInInv = true;

        //Chek to see if the item to be stored was properly submitted to the inventory and is not null - Continue if tes otherwise do nothing
        if (itemEntry.invEntry)
        {
            //check to see if any items exist in the inventory already - if not, add this item
            if (itemInInventory.Count == 0)
            {
                addedItem = AddItemToInv(addedItem);
            }
            //if item exist in inventory
            else
            {
                //check to see if the item is stackable - Continue if stackable
                if (itemEntry.invEntry.itemDefinition.isStackable)
                {
                    foreach (KeyValuePair<int, InventoryEntry> ie in itemInInventory)
                    {
                        //Does this item already exist in inventory? - Contimue if yes
                        if (itemEntry.invEntry.itemDefinition == ie.Value.invEntry.itemDefinition)
                        {
                            //Add 1 to stack and destroy the new instance
                            ie.Value.stackSize += 1;
                            AddItemToHotBar(ie.Value);
                            itsInInv = true;
                            DestroyObject(itemEntry.invEntry.gameObject);
                            break;
                        }
                        // if item does not exist already in inventory than continue here
                        else
                        {
                            itsInInv = false;
                        }
                    }
                }
                //if item is not stackable then continue here
                else
                {
                    itsInInv = false;

                    //if no space and item is not stackable - say inventory full
                    if (itemInInventory.Count == inventoryItemCap)
                    {
                        itemEntry.invEntry.gameObject.SetActive(true);
                        Debug.Log("Inventory is Full");
                    }
                }

                //Check if there is space in inventory - if yes, continue here
                if (!itsInInv)
                {
                    addedItem = AddItemToInv(addedItem);
                    itsInInv = true;
                }
            }
        }
    }

    bool AddItemToInv(bool finishedAdding)
    {
        itemInInventory.Add(idCount, new InventoryEntry(itemEntry.stackSize, Instantiate(itemEntry.invEntry), itemEntry.hbSprite));

        DestroyObject(itemEntry.invEntry.gameObject);

        FillInventoryDisplay();
        AddItemToHotBar(itemInInventory[idCount]);

        idCount = IncreaseID(idCount);

        #region Reset ItemEntry
        itemEntry.invEntry = null;
        itemEntry.stackSize = 0;
        itemEntry.hbSprite = null;
        #endregion

        finishedAdding = true;

        return finishedAdding;
    }

    int IncreaseID(int currentID)
    {
        int newID = 1;

        for (int itemCount = 1; itemCount <= itemInInventory.Count; itemCount++)
        {
            if (itemInInventory.ContainsKey(newID))
            {
                newID += 1;
            }
            else return newID;
        }

        return newID;
    }

    void AddItemToHotBar(InventoryEntry itemForHotBar)
    {
        int hotBarCounter = 0;
        bool increaseCount = false;

        //Check for open hotbar slot
        foreach (Image images in hotBarDisplayHolders)
        {
            hotBarCounter += 1;

            if (itemForHotBar.hotBarSlots == 0)
            {
                if (images.sprite == null)
                {
                    //Add item to open hotbar slots
                    itemForHotBar.hotBarSlots = hotBarCounter;
                    //Change hotbar sprite to show item
                    images.sprite = itemForHotBar.hbSprite;
                    increaseCount = true;
                    break;
                }
            }
            else if (itemForHotBar.invEntry.itemDefinition.isStackable)
            {
                increaseCount = true;
            }
        }

        if (increaseCount)
        {
            hotBarDisplayHolders[itemForHotBar.hotBarSlots - 1].GetComponentInChildren<Text>().text = itemForHotBar.stackSize.ToString();
        }

        increaseCount = false;
    }

    void DisplayInventory()
    {
        if (inventoryDisplayHolder.activeSelf == true)
        {
            inventoryDisplayHolder.SetActive(false);
        }
        else
        {
            inventoryDisplayHolder.SetActive(true);
        }
    }

    void FillInventoryDisplay()
    {
        int slotCounter = 9;

        foreach (KeyValuePair<int, InventoryEntry> ie in itemInInventory)
        {
            slotCounter += 1;
            inventoryDisplaySlots[slotCounter].sprite = ie.Value.hbSprite;
            ie.Value.inventorySlot = slotCounter - 9;
        }

        while (slotCounter < 29)
        {
            slotCounter++;
            inventoryDisplaySlots[slotCounter].sprite = null;
        }
    }

    public void TriggerItemUse(int itemToUseID)
    {
        bool trigerItem = false;

        foreach (KeyValuePair<int, InventoryEntry> ie in itemInInventory)
        {
            if (itemToUseID > 100)
            {
                itemToUseID -= 100;

                if (ie.Value.hotBarSlots == itemToUseID)
                {
                    trigerItem = true;
                }
            }
            else
            {
                if (ie.Value.hotBarSlots == itemToUseID)
                {
                    trigerItem = true;
                }
            }

            if (trigerItem)
            {
                if (ie.Value.stackSize == 1)
                {
                    if (ie.Value.invEntry.itemDefinition.isStackable)
                    {
                        if (ie.Value.hotBarSlots != 0)
                        {
                            hotBarDisplayHolders[ie.Value.hotBarSlots - 1].sprite = null;
                            hotBarDisplayHolders[ie.Value.hotBarSlots - 1].GetComponentInChildren<Text>().text = "0";
                        }

                        ie.Value.invEntry.UseItem();
                        itemInInventory.Remove(ie.Key);
                        break;
                    }
                    else
                    {
                        ie.Value.invEntry.UseItem();
                        if (!ie.Value.invEntry.itemDefinition.isIndestructable)
                        {
                            itemInInventory.Remove(ie.Key);
                            break;
                        }
                    }
                }
                else
                {
                    ie.Value.invEntry.UseItem();
                    ie.Value.stackSize -= 1;
                    hotBarDisplayHolders[ie.Value.hotBarSlots - 1].GetComponentInChildren<Text>().text = ie.Value.stackSize.ToString();
                    break;
                }
            }
        }

        FillInventoryDisplay();
    }
}