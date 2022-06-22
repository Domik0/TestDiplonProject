using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string title;
    public int timeDurationBonus;
    public Sprite image;
    public GameObject Granade;
}
