using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> characterItems = new List<Item>();
    public ItemDatabase itemDatabase;
    public UIInventory inventoryUI;


    private void Start()
    {
        GiveItem(1);
        GiveItem(0);
        GiveItem(2);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
        }
    }
    public void GiveItem(int id)
    {
        Item itemAdd = itemDatabase.GetItem(id);
        characterItems.Add(itemAdd);
        inventoryUI.Addnewitem(itemAdd);
        Debug.Log("Added item: " + itemAdd.title);
    }
    public void GiveItem(string itemName)
    {
        Item itemAdd = itemDatabase.GetItem(itemName);
        characterItems.Add(itemAdd);
        Debug.Log("Added item: " + itemAdd.title);
    }
    public Item CheckForItem(int id)
    {
        return characterItems.Find(item => item.id == id);
    }
    public void RemoveItem(int id)
    {
        Item itemToRemove = CheckForItem(id);
        if(itemToRemove != null)
        {
            characterItems.Remove(itemToRemove);
            inventoryUI.RemoveItem(itemToRemove);
        }
    }
}
