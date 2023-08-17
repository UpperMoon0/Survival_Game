using System.Linq;
using UnityEngine;

public class Inventory
{
    private Slot[] slots;
    public Inventory(int slotNum)
    {
        slots = Enumerable.Range(0, slotNum).Select(_ => new Slot()).ToArray();
    }
    public bool AddItem(int itemId, int quantity)
    {
        // Check if there is enough space in the inventory to hold the specified quantity of items
        int totalSpaceLeft = slots.Where(slot => slot.ItemId == itemId || slot.ItemId == -1)
                                  .Sum(slot => slot.Capacity - slot.Quantity);
        if (totalSpaceLeft < quantity) return false;

        // Find all slots that contain the same itemId and add the items to them
        foreach (Slot slot in slots.Where(slot => slot.ItemId == itemId))
        {
            int spaceLeft = slot.Capacity - slot.Quantity;
            int quantityToAdd = Mathf.Min(spaceLeft, quantity);
            slot.Quantity += quantityToAdd;
            quantity -= quantityToAdd;
            if (quantity == 0) return true;
        }

        // If no slot has the same itemId or they are full, find the first empty slot
        foreach (Slot slot in slots.Where(slot => slot.ItemId == -1))
        {
            int spaceLeft = slot.Capacity;
            int quantityToAdd = Mathf.Min(spaceLeft, quantity);
            slot.ItemId = itemId;
            slot.Quantity = quantityToAdd;
            quantity -= quantityToAdd;
            if (quantity == 0) return true;
        }

        return false;
    }
    public bool AddItemToSlot(int slotIndex, int itemId, int quantity)
    {
        Slot slot = slots[slotIndex];

        // Check if the slot is empty or contains the same item
        if (slot.ItemId != -1 && slot.ItemId != itemId) return false;

        // Check if there is enough space in the slot to hold the specified quantity of items
        int spaceLeft = slot.Capacity - slot.Quantity;
        if (spaceLeft < quantity) return false;

        // Add the item to the slot
        slot.ItemId = itemId;
        slot.Quantity += quantity;
        return true;
    }
    public bool RemoveItem(int itemId, int quantity)
    {
        // Check if the total quantity of the specified item in the inventory is enough
        int totalQuantity = GetTotalQuantity(itemId);
        if (totalQuantity < quantity) return false;

        // Find all slots that contain the specified itemId and remove the items from them
        foreach (Slot slot in slots.Where(slot => slot.ItemId == itemId))
        {
            int quantityToRemove = Mathf.Min(slot.Quantity, quantity);
            slot.Quantity -= quantityToRemove;
            quantity -= quantityToRemove;
            if (slot.Quantity <= 0)
            {
                // Make the slot empty
                slot.ItemId = -1;
                slot.Quantity = 0;
            }
            if (quantity == 0) return true;
        }

        return false;
    }
    public bool RemoveItemFromSlot(int slotIndex, int itemId, int quantity)
    {
        Slot slot = slots[slotIndex];

        // Check if the slot contains the specified item
        if (slot.ItemId != itemId) return false;

        // Check if the quantity of the specified item in the slot is enough
        if (slot.Quantity < quantity) return false;

        // Remove the item from the slot
        slot.Quantity -= quantity;
        if (slot.Quantity <= 0)
        {
            // Make the slot empty
            slot.ItemId = -1;
            slot.Quantity = 0;
        }
        return true;
    }

    public int GetItemId(int index) => slots[index].ItemId;
    public int GetQuantity(int index) => slots[index].Quantity;
    public int GetCapacity(int index) => slots[index].Capacity;
    public int GetTotalSlots() => slots.Length;

    public bool Craft(Recipe recipe)
    {
        // Check if all ingredients are available in the inventory
        foreach (int[] ingredient in recipe.Ingredient)
        {
            int itemId = ingredient[0];
            int requiredQuantity = ingredient[1];
            if (!RemoveItem(itemId, requiredQuantity))
            {
                // If any ingredient is not available, add back the removed ingredients
                for (int i = 0; i < recipe.Ingredient.IndexOf(ingredient); i++)
                {
                    AddItem(recipe.Ingredient[i][0], recipe.Ingredient[i][1]);
                }
                return false;
            }
        }

        // Add the result item to the inventory
        AddItem(recipe.Result[0], recipe.Result[1]);

        return true;
    }
    public int GetTotalQuantity(int itemId)
    {
        return slots.Where(slot => slot.ItemId == itemId).Sum(slot => slot.Quantity);
    }
}
