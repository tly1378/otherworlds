using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : ContainerUI
{
    public static GameObject sampleUI;
    [HideInInspector()]
    public List<Craft> crafts;

    [Header(nameof(CraftUI))]
    public Text itemInformation;
    public RectTransform craftGrid;
    public GameObject craftSlot;

    public override void Start()
    {
        base.Start();
        foreach (Transform ui_transform in transform.parent)
        {
            if (ui_transform == transform) continue;
            CraftUI ui = ui_transform.GetComponent<CraftUI>();
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
            GameObject slotGO = Instantiate(base.slot, grid);
            Slot slot = slotGO.GetComponent<Slot>();
            slot.slotIndex = i;
            slot.Initialization(inventory[i]);
        }

        //合成
        for (int i = 0; i < craftGrid.childCount; i++)
        {
            if (craftGrid.childCount == 0)
                break;
            Destroy(craftGrid.GetChild(i).gameObject);
        }

        //为合成表创建Slot
        for (int i = 0; i < crafts.Count; i++)
        {
            GameObject slotGO = Instantiate(craftSlot, craftGrid);
            CraftSlot slot = slotGO.GetComponent<CraftSlot>();
            slot.slotIndex = i;
            slot.Initialization(crafts[i], inventory);
        }
    }
}