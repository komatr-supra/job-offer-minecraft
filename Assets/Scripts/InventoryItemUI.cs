using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textMesh;
    public void Init(Sprite sprite, int amount)
    {
        image.sprite = sprite;
        textMesh.text = amount.ToString();
    }
}
