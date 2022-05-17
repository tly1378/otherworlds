using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftSlot : Slot
{
    public Image image;
    public TMPro.TextMeshProUGUI description;
    public new TMPro.TextMeshProUGUI name;

    private Craft craft;
    private Inventory inventory;

    public void Initialization(Craft craft, Inventory inventory)
    {
        this.craft = craft;
        this.inventory = inventory;
        Item item = craft.products[0].item;
        if (item.itemFunction == Item.Type.蓝图)
        {
            background.sprite = GameManager.instance.bluePrint;
            image.transform.localScale = Vector3.one * 0.65f;
        }
        image.sprite = item.image;
        name.text = item.name;
        description.text = craft.ToString();
    }

    public void Craft()
    {
        inventory.Craft(craft);
    }
}
