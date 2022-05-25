using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    public Action<Item> onItemAdded;

    [SerializeField] private List<Item> StartItems = new List<Item>();

    public List<Item> inventoryItems = new List<Item>();
    
    void Awake()
    {
        var storage = Storage.GetStorage();
        //AddItem(storage.GetItem("BombSlowdown"));
        //AddItem(storage.GetItem("InvisibilityPotion"));
    }
    
    public void AddItem(Item item)
    {
        inventoryItems.Add(item);
        
        onItemAdded?.Invoke(item);
    }
}
