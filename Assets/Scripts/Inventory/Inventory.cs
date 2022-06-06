using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    public Action onItemAddorDel;

    [SerializeField] private List<Item> StartItems = new List<Item>();

    public List<Item> inventoryItems = new List<Item>();
    
    void Awake()
    {
        var storage = Storage.GetStorage();
    }
    
    public void AddItem(Item item)
    {
        inventoryItems.Add(item);

        onItemAddorDel?.Invoke();
    }

    public void RemoveItemAt(int index)
    {
        inventoryItems.RemoveAt(index);

        onItemAddorDel?.Invoke();
    }
}
