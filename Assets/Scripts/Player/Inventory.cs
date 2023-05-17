using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//simple dictionary
public class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    private InventoryBlock[] inventoryData;
    private float scrollValue;
    private void Awake() {
        inventoryData = new InventoryBlock[3];
    }
    private void Update() {
        float scrollDelta = Mouse.current.scroll.ReadValue().y;
        if(scrollDelta != 0) Debug.Log(scrollDelta);
    }
    public void AddBlockToInventory(BlocksSO block, int count)
    {
        //block is in inventory
        for (int i = 0; i < 3; i++)
        {
            if(inventoryData[i].blocksSO == block)
            {
                int amount = inventoryData[i].amount + count;
            }
        }
        //update ui
    }
    public bool RemoveBlockFromInventory(int index)
    {
        
        //update ui
    }
    private struct InventoryBlock
    {
        public BlocksSO blocksSO;
        public int amount;
        public InventoryBlock(BlocksSO blocksSO, int amount)
        {
            this.blocksSO = blocksSO;
            this.amount = amount;
        }
    }
}
