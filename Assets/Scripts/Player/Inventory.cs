using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//simple dictionary
public class Inventory : MonoBehaviour
{
    public Action onInventoryChanged;
    private List<InventoryBlock> inventoryData;
    private int selected;
    public int Selected => selected;
    private void Awake()
    {
        inventoryData = new();
        onInventoryChanged += UpdateSelection;
    }
    private void OnDestroy() {
        onInventoryChanged -= UpdateSelection;
    }
    private void Update() {
        float scrollDelta = Mouse.current.scroll.ReadValue().y;
        if(scrollDelta != 0)
        {
            float sign = Mathf.Sign(scrollDelta);
            int direction = Mathf.RoundToInt(sign);

        }
    }
    private void UpdateSelection()
    {
        MoveSelection(0);
    }
    private void MoveSelection(int direction)
    {
        selected += direction;
        selected = WrapSelection();
    }

    public bool TryGetSelected(out BlocksSO blocksSO)
    {
        if(selected < inventoryData.Count)
        {
            blocksSO = inventoryData[selected].blocksSO;
            return true;
        }
        blocksSO = FakeDatabase.Instance.GetBlock(0);
        return false;
    }

    private int WrapSelection()
    {
        if(selected < 0) return inventoryData.Count - 1;
        else if(selected > inventoryData.Count - 1) return 0;
        return selected;
    }
    public InventoryBlock[] GetInventoryBlocks() => inventoryData.ToArray();
    public void AddBlockToInventory(BlocksSO block, int count)
    {
        //block is in inventory
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if(inventoryData[i].blocksSO == block)
            {
                int newAmount = inventoryData[i].amount + count;
                inventoryData[i] = new InventoryBlock(block, newAmount);
                onInventoryChanged?.Invoke();
                return;
            }
        }
        //new item
        inventoryData.Add(new InventoryBlock(block, count));
        //update ui
        onInventoryChanged?.Invoke();
    }
    public bool RemoveBlockFromInventory(BlocksSO block)
    {
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if(inventoryData[i].blocksSO == block)
            {
                int newAmount = inventoryData[i].amount - 1;
                if(newAmount == 0)
                {
                    inventoryData.RemoveAt(i);
                    onInventoryChanged?.Invoke();
                    return true;
                } 
                inventoryData[i] = new InventoryBlock(block, newAmount);
                onInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }
    
}
public struct InventoryBlock
    {
        public BlocksSO blocksSO;
        public int amount;
        public InventoryBlock(BlocksSO blocksSO, int amount)
        {
            this.blocksSO = blocksSO;
            this.amount = amount;
        }
    }