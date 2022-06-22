using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Storage", menuName = "Inventory/Storage")]
public class Storage : ScriptableObject
{
    private static Storage instance;
    public List<Item> AllItems;
    public Item GetItem(string title) => AllItems.Find(i => i.title == title);

    public static Storage GetStorage()
    {
        if (!instance)
            instance = Resources.Load("Storage") as Storage;
        return instance;
    }
}
