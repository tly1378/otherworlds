using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject pauseUI;
    public GameObject containerUI;
    public GameObject craftUI;
    public GameObject constructionUI;
    public GameObject playerUI;
    public GameObject deathUI;
    [HideInInspector()]
    public CraftUI PlayerUI;
    public GameObject ItemUI;

    private void Start() 
    { 
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale > 0)
        {
            OpenUI(pauseUI);
        }
    }

    public GameObject OpenUI(GameObject sampleUI)
    {
        GameObject ui = Instantiate(sampleUI, FindObjectOfType<Canvas>().transform);
        return ui;
    }

    public ContainerUI OpenContainerUI(Inventory inventory)
    {
        GameObject ui = OpenUI(this.containerUI);
        ContainerUI containerUI = ui.GetComponent<ContainerUI>();
        containerUI.inventory = inventory;
        containerUI.Refresh();
        return containerUI;
    }

    public ConstructionUI OpenConstructionUI(Facility facility)
    {
        GameObject go = Instantiate(constructionUI, FindObjectOfType<Canvas>().transform);
        ConstructionUI ui = go.GetComponent<ConstructionUI>();
        ui.facility = facility;
        ui.title.text = facility.name;
        ui.Refresh();
        return ui;
    }

    public CraftUI OpenPlayUI()
    {
        CraftUI craftUI = OpenCraftUI(Player.player.inventory, Player.player.crafts);
        craftUI.Refresh();
        return craftUI;
    }

    public CraftUI OpenCraftUI(Inventory inventory, List<Craft> crafts)
    {
        GameObject ui = OpenUI(this.craftUI);
        CraftUI craftUI = ui.GetComponent<CraftUI>();
        craftUI.inventory = inventory;
        craftUI.crafts = crafts;
        craftUI.Refresh();

        return craftUI;
    }
}
