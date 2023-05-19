using UnityEngine;

//inventory UI at the bottom of the gamr
namespace UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject selectionArea;
        [SerializeField] private GameObject inventoryPrefab;
        [SerializeField] private Character.Inventory inventory;
        private InventoryBlock[] inventoryBlocks;
        private int selected;
        private void Start()
        {
            inventory.onInventoryChanged += UpdateInventory;
        }
        private void OnDestroy()
        {
            inventory.onInventoryChanged -= UpdateInventory;
        }
        private void UpdateInventory()
        {
            inventoryBlocks = inventory.GetInventoryBlocks();
            selected = inventory.Selected;
            //destroy old
            foreach (Transform child in selectionArea.transform)
            {
                Destroy(child.gameObject);
            }
            //make new
            for (int i = 0; i < inventoryBlocks.Length; i++)
            {
                var invItemData = inventoryBlocks[i];
                var invItem = Instantiate(inventoryPrefab, selectionArea.transform);
                invItem.GetComponent<InventoryItemUI>().Init(invItemData.blocksSO.invSprite, invItemData.amount);
                if (selected == i)
                {
                    invItem.transform.localScale = new Vector3(2, 2, 2);
                }
            }
            selected = inventory.Selected;
        }
        private void Deselect()
        {
            var v = selectionArea.transform.GetChild(selected);
            v.localScale = Vector3.one;
        }
        private void Select()
        {
            var v = selectionArea.transform.GetChild(selected);
            v.localScale = new Vector3(2, 2, 2);
        }
    }
}
