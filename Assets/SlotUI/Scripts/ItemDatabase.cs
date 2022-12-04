using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    [SerializeField] GameObject InvPanelParent;
    private void Start()
    {
        //Invoke("setNotVisible", 0.3f);
    }
    private void setNotVisible()
    {
        InvPanelParent.gameObject.SetActive(false);
    }

    private void Awake()
    {
        BuildDatabase();
    }

    public Item GetItem(int id)
    {
        return items.Find(item => item.id == id);
    }
    public Item GetItem(string itemName)
    {
        return items.Find(item => item.title == itemName);
    }
    void BuildDatabase()
    {
        items = new List<Item>()
        {
            new Item(0,"Diamond sword"," A sword made of diamond.",
            new Dictionary<string, int>
            {
                {"Power", 15 },
                {"Defence", 10 }
            }),
            
            new Item(1,"Diamond ore"," A diamond ore.",
            new Dictionary<string, int>
            {
                {"Value", 300 }
            }),

            new Item(2,"Silver Pick"," A pick that kill a vampire.",
            new Dictionary<string, int>
            {
                {"Power", 5 },
                {"Mining", 20 }
            })
        };
    }
}
