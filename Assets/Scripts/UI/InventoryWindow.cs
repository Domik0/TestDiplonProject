using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] public Inventory targetInventory;
    [SerializeField] RectTransform itemsPanel;

    private List<GameObject> drawnIcons = new List<GameObject>();

    public void StartAddInventory(Inventory inventory)
    {
        targetInventory = inventory;
        targetInventory.onItemAdded += OnItemAdded;
        Redraw();
    }

    private void OnItemAdded(Item item) => Redraw();

    void Redraw()
    {
        ClearDrawn();

        foreach (var item in targetInventory.inventoryItems)
        {
            var icon = new GameObject("Icon");
            icon.AddComponent<Image>().sprite = item.image;

            icon.transform.SetParent(itemsPanel);
            drawnIcons.Add(icon);
        }
    }

    void ClearDrawn()
    {
        foreach (var item in drawnIcons)
        {
            Destroy(item);
        }

        drawnIcons.Clear();
    }
}
