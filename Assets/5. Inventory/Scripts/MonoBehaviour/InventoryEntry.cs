using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEntry
{
    public ItemPickUp invEntry; //store the item being pick up
    public int stackSize;
    public int inventorySlot;
    public int hotBarSlots;
    public Sprite hbSprite;

    public InventoryEntry(int stackSize, ItemPickUp invEntry, Sprite hbSprite)
    {
        this.invEntry = invEntry;

        this.stackSize = stackSize;
        hotBarSlots = 0;
        inventorySlot = 0;
        this.hbSprite = hbSprite;
    }
}
