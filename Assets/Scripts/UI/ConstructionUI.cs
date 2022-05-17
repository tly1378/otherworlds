using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionUI : UIForm
{
    public GameObject slotSample;
    public Transform grid;
    [HideInInspector()]
    public Facility facility;

    public override void Refresh()
    {
        for (int i = 0; i < grid.childCount; i++)
        {
            if (grid.childCount == 0)
                break;
            Destroy(grid.GetChild(i).gameObject);
        }

        for (int i = 0; i < facility.upgrades.Count; i++)
        {
            GameObject slotGO = Instantiate(slotSample, grid);
            UpgrateSlot slot = slotGO.GetComponent<UpgrateSlot>();
            slot.SetUpgrate(facility, i, this);
        }
    }
}
