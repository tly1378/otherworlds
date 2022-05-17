using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [HideInInspector()]
    public int slotIndex;
    public Image background;
    public bool indraggable;

    public virtual void Initialization(Items items)
    {
        if (items != null && items.item !=null)
        {
            NewItemsUI(items);
        }
    }

    //背包
    public void NewItemsUI(Items items)
    {
        GameObject go = Instantiate(UIManager.instance.ItemUI, transform);
        ItemsUI ui = go.GetComponent<ItemsUI>();
        if (indraggable) ui.indraggable = true;
        ui.SetItems(items);
    }

    //public Image normalBackground;
    //public Image blueprintBackground;
    //public TMPro.TextMeshProUGUI slotInfo;
    //public TMPro.TextMeshProUGUI slotName;
    ////For Item
    //[HideInInspector()]
    //public GameObject itemsUI;
    ////For Craft
    //[HideInInspector()]
    //public Craft craft;
    ////Foe Upgrade
    //[HideInInspector()]
    //public Facility facility;


    //public void Start()
    //{
    //    ui = GetComponentInParent<UIForm>();
    //}

    //public void SetBackground(Items items)
    //{
    //    Item item = items.item;
    //    if (item != null && item.itemFunction == Item.Type.蓝图)
    //    {
    //        normalBackground.enabled = false;
    //        blueprintBackground.enabled = true;
    //    }
    //    else
    //    {
    //        normalBackground.enabled = true;
    //        blueprintBackground.enabled = false;
    //    }
    //}


    ////合成
    //public void SetCraft(Craft craft)
    //{
    //    this.craft = craft;
    //    image.sprite = craft.products[0].item.image;
    //    slotName.text = craft.products[0].item.name;
    //    slotInfo.text = craft.ToString();
    //}

    ////合成按钮
    //public void Craft()
    //{
    //    Player.player.Craft(craft);
    //}

    ////改造按钮
    //public void Upgrade()
    //{
    //    facility.Upgrade(slotIndex, Player.player.inventory);
    //    ui.Close();
    //}
}
