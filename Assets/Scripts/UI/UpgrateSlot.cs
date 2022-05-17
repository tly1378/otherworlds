using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgrateSlot : Slot
{
    [HideInInspector()]
    public UIForm ui;

    public TMPro.TextMeshProUGUI description;
    public new TMPro.TextMeshProUGUI name;
    [HideInInspector()]
    public Facility facility;

    public void Initialization(Facility facility, Facility.UpgradeInfo upgrade)
    {
        this.facility = facility;
        description.text = upgrade.requirementsString();
        name.text = upgrade.target.name;
    }

    public void Upgrade()
    {
        facility.Upgrade(slotIndex, Player.player.inventory);
        ui.Close();
    }

    public void SetUpgrate(Facility facility, int i, ConstructionUI constructionUI)
    {
        slotIndex = i;
        this.facility = facility;
        name.text = facility.upgrades[i].title;
        description.text = facility.upgrades[i].info + "\n";
        description.text += facility.upgrades[i].requirementsString();
        ui = constructionUI;
        GameObject target = facility.upgrades[i].target;
        Sprite sprite = target.GetComponent<SpriteRenderer>().sprite;
        background.sprite = sprite;
    }
}
