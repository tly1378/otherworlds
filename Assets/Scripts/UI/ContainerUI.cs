using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerUI : UIForm
{

    [HideInInspector()]
    public Inventory inventory;
    [Header(nameof(ContainerUI))]
    public RectTransform grid;
    public GameObject slot;

    public override void Start()
    {
        base.Start();
        foreach (Transform ui_transform in transform.parent)
        {
            if (ui_transform == transform) continue;
            ContainerUI ui = ui_transform.GetComponent<ContainerUI>();
            if (ui != null && !ui.CompareTag("PermanentUI") && ui.inventory == inventory)
            {
                Destroy(ui.gameObject);
            }
        }
        Refresh();
    }

    public override void Refresh()
    {
        //容器
        for (int i = 0; i < grid.childCount; i++)
        {
            if (grid.childCount == 0)
                break;
            Destroy(grid.GetChild(i).gameObject);
        }
        for (int i = 0; i < inventory.itemList.Count; i++)
        {
            GameObject slotGO = Instantiate(this.slot, grid);
            Slot slot = slotGO.GetComponent<Slot>();
            slot.slotIndex = i;
            slot.Initialization(inventory[i]);
        }
    }

    public override void Close()
    {
        Destroy(gameObject);
    }

    public void ClearUp()
    {
        for (int i = 0; i < inventory.itemList.Count; i++)
        {
            if (inventory.itemList[i].item != null)
            {
                for (int j = i + 1; j < inventory.itemList.Count; j++)
                {
                    if (inventory.itemList[j].item != null)
                    {
                        if (inventory.itemList[i].item.name == inventory.itemList[j].item.name)
                        {
                            inventory[i].count += inventory[j].count;
                            inventory[j].item = null;
                            inventory[j].count = 0;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < inventory.itemList.Count; i++)
        {
            if (inventory.itemList[i].item == null)
            {
                bool has = false;
                for (int j = i + 1; j < inventory.itemList.Count; j++)
                {
                    if (inventory.itemList[j].item != null)
                    {
                        inventory[i].item = inventory[j].item;
                        inventory[i].count = inventory[j].count;
                        inventory[j].item = null;
                        inventory[j].count = 0;
                        has = true;
                        break;
                    }
                }
                if (!has)
                    break;
            }
        }
        Refresh();
    }

    public void TakeAll()
    {
        Player.player.inventory.Receive(inventory);
        Refresh();
        if (UIManager.instance.PlayerUI != null)
        {
            UIManager.instance.PlayerUI.Refresh();
        }
    }
}
