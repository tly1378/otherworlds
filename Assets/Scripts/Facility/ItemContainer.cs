using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class ItemContainer : Facility
{
    private ContainerUI ui;

    [Header(nameof(ItemContainer))]
    public bool isResources;//可直接拾取
    public Inventory inventory;
    public int size;
    public float coolingTime;
    protected float coolingTimer;
    public PotentialItem[] potentialItems;

    public bool autoCraft;
    public List<Craft> crafts;

    [Serializable]
    public struct PotentialItem
    {
        public Item item;
        public int minCount;
        public int maxCount;
    }

    public override void Start()
    {
        base.Start();
        if(inventory==null)
            inventory = Inventory.New(size);

        if (coolingTime <= 0)
        {
            RandomPut(potentialItems);
        }
        else
        {
            coolingTimer = coolingTime;
        }

        //Test
        if (isResources)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.AddForceAtPosition(Random.insideUnitCircle * 5, Random.insideUnitCircle*5);
            rb.AddForce(Random.onUnitSphere * 200);
        }
    }

    public virtual void Update()
    {
        coolingTimer -= Time.deltaTime;
        if (coolingTime > 0)
        {
            if (coolingTimer <= 0)
            {
                coolingTimer = coolingTime;
                RandomPut(potentialItems);
            }
        }

        //合成
        if (inventory.craftingEnumerator == null)
        {
            if (autoCraft)
            {
                foreach (Craft craft in crafts)
                {
                    if (inventory.Contain(craft.requirements))
                    {
                        inventory.Craft(craft);
                        break;
                    }
                }
            }
        }
        else
        {
            inventory.ui.title.text = name + "(" + inventory.craftingTimer.ToString("f0") + ")";
        }
    }

    public void RandomPut(PotentialItem[] potentialItems)
    {
        foreach (PotentialItem potentialItem in potentialItems)
        {
            int count = Random.Range(potentialItem.minCount, potentialItem.maxCount);
            if (count > 0)
            {
                Items resource = new Items(potentialItem.item, count);
                inventory.Receive(resource);
            }
        }

        if (ui != null)
        {
            ui.Refresh();
        }
    }

    public override void Use()
    {
        if (OpenConstructionUI()) return;

        if (isResources)
        {
            Player.player.inventory.Receive(inventory);
            if(UIManager.instance.PlayerUI != null)
                UIManager.instance.PlayerUI.Refresh();
            Destroy(gameObject);
        }
        else
        {
            if (size > 0)
            {
                if (autoCraft || crafts.Count == 0)
                {
                    ui = UIManager.instance.OpenContainerUI(inventory);
                }
                else
                {
                    ui = UIManager.instance.OpenCraftUI(inventory, crafts);
                }
                inventory.ui = ui;
                ui.title.text = name;
            }
        }
    }
}
