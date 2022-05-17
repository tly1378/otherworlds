using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutBar : ContainerUI
{
    public static ShortcutBar instance;
    public Transform choose;
    public int maxSlot;
    public TMPro.TextMeshProUGUI Timer;
    public TMPro.TextMeshProUGUI throwOut_text;
    public GameObject throwOut;
    public GameObject eat;
    public GameObject[] icons;

    private new void Start()
    {
        instance = this;
        InvokeRepeating(nameof(Refresh), 0, 0.1f);
    }

    private void Update()
    {
        icons[0].gameObject.SetActive(false);
        icons[1].gameObject.SetActive(false);
        icons[2].gameObject.SetActive(false);
        icons[3].gameObject.SetActive(false);
        icons[4].gameObject.SetActive(false);
        icons[5].gameObject.SetActive(false);
        Item item = Player.player.inventory[Player.player.SelectedIndex].item;
        if (item != null)
        {
            if (item.itemFunction == Item.Type.弩)
            {
                throwOut_text.text = "长按Q: 扔出"; 
                icons[0].gameObject.SetActive(true);
                icons[1].gameObject.SetActive(true);
                icons[2].gameObject.SetActive(true);
            }
            else if (item.itemFunction == Item.Type.道具)
            {
                throwOut_text.text = "长按Q: 扔出";
                if(item.name == "阀")
                {
                    icons[3].gameObject.SetActive(true);
                }
                else if (item.name == "铁桥")
                {
                    icons[5].gameObject.SetActive(true);
                }
            }
            else if(item.itemFunction == Item.Type.改造)
            {
                throwOut_text.text = "Q: 扔出";
                icons[4].gameObject.SetActive(true);
            }
            else
            {
                throwOut_text.text = "Q: 扔出";
            }

            throwOut.SetActive(true);
            if (item.itemFunction == Item.Type.可使用)
            {
                eat.SetActive(true);
            }
            else
            {
                eat.SetActive(false);
            }
        }
        else
        {
            throwOut.SetActive(false);
            eat.SetActive(false);
        }

        float timer = Player.player.inventory.craftingTimer;
        if (timer > 0)
            Timer.text = timer.ToString("f2");
        else
            Timer.text = "";

        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Player.player.SelectedIndex = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)) 
            {
                Player.player.SelectedIndex = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Player.player.SelectedIndex = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Player.player.SelectedIndex = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Player.player.SelectedIndex = 4;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Player.player.SelectedIndex = 5;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                Player.player.SelectedIndex = 6;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                Player.player.SelectedIndex = 7;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Player.player.SelectedIndex = 8;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Player.player.SelectedIndex = 9;
            }
        }
    }

    public override void Refresh()
    {
        for (int i = 0; i < grid.childCount; i++)
        {
            if (grid.childCount == 0)
                break;
            Destroy(grid.GetChild(i).gameObject);
        }
        for (int i = 0; i < inventory.itemList.Count && i < maxSlot; i++)
        {
            GameObject slotGO = Instantiate(base.slot, grid);
            Slot slot = slotGO.GetComponent<Slot>();
            slot.slotIndex = i;
            slot.indraggable = true;
            slot.Initialization(inventory[i]);
        }
    }

    public void UpdateChoose(int index)
    {
        choose.position = grid.GetChild(index).position;
    }
}
