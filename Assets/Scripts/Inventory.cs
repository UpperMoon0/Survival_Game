using UnityEngine;

public class Inventory
{
    private int slotNum;
    private Slot[] slots;
    public Inventory(int slotNum)
    {
        this.slotNum = slotNum;
        slots = new Slot[slotNum];
        for (int i = 0; i < slotNum; i++)
        {
            slots[i] = new Slot();
        }
    }
    public bool AddItem(int itemId, int quantity)
    {
        // Check if there is enough space in the inventory to hold the specified quantity of items
        int totalSpaceLeft = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].ItemId == itemId || slots[i].ItemId == -1)
            {
                totalSpaceLeft += slots[i].Capacity - slots[i].Quantity;
            }
        }
        if (totalSpaceLeft < quantity) return false;

        // Find all slots that contain the same itemId and add the items to them
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].ItemId == itemId)
            {
                int spaceLeft = slots[i].Capacity - slots[i].Quantity;
                int quantityToAdd = Mathf.Min(spaceLeft, quantity);
                slots[i].Quantity += quantityToAdd;
                quantity -= quantityToAdd;
                if (quantity == 0) return true;
            }
        }

        // If no slot has the same itemId or they are full, find the first empty slot
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].ItemId == -1)
            {
                int spaceLeft = slots[i].Capacity;
                int quantityToAdd = Mathf.Min(spaceLeft, quantity);
                slots[i].ItemId = itemId;
                slots[i].Quantity = quantityToAdd;
                quantity -= quantityToAdd;
                if (quantity == 0) return true;
            }
        }

        return false;
    }
    public bool AddItemToSlot(int slotIndex, int itemId, int quantity)
    {
        // Check if the slot is empty or contains the same item
        if (slots[slotIndex].ItemId != -1 && slots[slotIndex].ItemId != itemId) return false;

        // Check if there is enough space in the slot to hold the specified quantity of items
        int spaceLeft = slots[slotIndex].Capacity - slots[slotIndex].Quantity;
        if (spaceLeft < quantity) return false;

        // Add the item to the slot
        slots[slotIndex].ItemId = itemId;
        slots[slotIndex].Quantity += quantity;
        return true;
    }
    public bool RemoveItem(int itemId, int quantity)
    {
        // Check if the total quantity of the specified item in the inventory is enough
        int totalQuantity = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].ItemId == itemId)
            {
                totalQuantity += slots[i].Quantity;
            }
        }
        if (totalQuantity < quantity) return false;

        // Find all slots that contain the specified itemId and remove the items from them
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].ItemId == itemId)
            {
                int quantityToRemove = Mathf.Min(slots[i].Quantity, quantity);
                slots[i].Quantity -= quantityToRemove;
                quantity -= quantityToRemove;
                if (slots[i].Quantity <= 0)
                {
                    slots[i].ItemId = -1; // Make the slot empty
                    slots[i].Quantity = 0;
                }
                if (quantity == 0) return true;
            }
        }

        return false;
    }
    public bool RemoveItemFromSlot(int slotIndex, int itemId, int quantity)
    {
        // Check if the slot contains the specified item
        if (slots[slotIndex].ItemId != itemId) return false;

        // Check if the quantity of the specified item in the slot is enough
        if (slots[slotIndex].Quantity < quantity) return false;

        // Remove the item from the slot
        slots[slotIndex].Quantity -= quantity;
        if (slots[slotIndex].Quantity <= 0)
        {
            slots[slotIndex].ItemId = -1;
            slots[slotIndex].Quantity = 0;
        }
        return true;
    }

    public int GetItemId(int slotId)
    {
        return slots[slotId].ItemId;
    }
    public int GetQuantity(int slotId)
    {
        return slots[slotId].Quantity;
    }
    public int GetTotalSlots()
    {
        return slotNum;
    }
}
