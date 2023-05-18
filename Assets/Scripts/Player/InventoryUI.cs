using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject selectionArea;
    [SerializeField] private GameObject inventoryPrefab;
    [SerializeField] private Inventory inventory;
    private InventoryBlock[] inventoryBlocks;
    int selected;
    private void Start() {
        inventory.onInventoryChanged += UpdateInventory;
    }
    private void OnDestroy() {
        inventory.onInventoryChanged -= UpdateInventory;
    }
    

    

    private void UpdateInventory()
    {
        inventoryBlocks = inventory.GetInventoryBlocks();
        //destroy old
        foreach (Transform child in selectionArea.transform)
        {
            Destroy(child.gameObject);
        }
        //make new
        foreach (var item in inventoryBlocks)
        {
            var invItem = Instantiate(inventoryPrefab, selectionArea.transform);
            invItem.GetComponent<InventoryItemUI>().Init(item.blocksSO.invSprite, item.amount);
        }
        
        //select right one
        Deselect();
        selected = inventory.Selected;
        //apply selection
        Select();
    }
    private void Deselect()
    {
        Debug.Log("deselect: " + selected);
    }

    private void Select()
    {
        Debug.Log("select: " + selected);
    }
}
