using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Items
{
    public Item item;
    public int count;
    public bool have
    {
        get => item != null && count > 0;
    }

    public Items(Item item = null, int count = 0)
    {
        this.item = item;
        this.count = count;
    }

    public override string ToString()
    {
        if (item == null) 
            return "无物品";
        else
            return $"{item.name}*{count}";
    }
}

[CreateAssetMenu(fileName = "New Inventory", menuName = "ScriptableObject/New Inventory")]
public class Inventory : ScriptableObject, IEnumerable
{
    public int maxCount = int.MaxValue;
    public float maxMass = float.MaxValue;

    public List<Items> itemList = new List<Items>();
    public UIForm ui;
    public IEnumerator craftingEnumerator;
    public float craftingTimer;

    public static Inventory New(int size = 20)
    {
        Inventory inventory = CreateInstance<Inventory>();
        for(int i = 0; i < size; i++)
        {
            inventory.itemList.Add(new Items());
        }
        return inventory;
    }

    public int GetCount(Item item)
    {
        int count = 0;
        foreach (Items items in this)
        {
            if (items.item == item)
                count += items.count;
        }
        return count;
    }

    //public void TransferTo(int f, Inventory to, int t)
    //{
    //    if (this[f].item == null) return;
    //    float mass = to.maxMass - to.Mass;
    //    if (mass <= 0) return;
    //    int maxReceivableCount = (int)(mass / this[f].item.mass);
    //    if (maxReceivableCount < 0) maxReceivableCount = to.maxCount;

    //    if (maxReceivableCount > this[f].count)
    //    {
    //        to[t].item = this[f].item;
    //        to[t].count = this[f].count;
    //        this[f].item = null;
    //        this[f].count = 0;
    //    }
    //    else
    //    {
    //        to[t].item = this[f].item;
    //        to[t].count = maxReceivableCount;
    //        this[f].count -= maxReceivableCount;
    //    }
    //}

    public float Mass
    {
        get
        {
            float mass = 0;
            foreach (Items items in itemList)
            {
                if (items.item != null)
                    mass += items.item.mass * items.count;
            }
            return mass;
        }
    }

    public int Count
    {
        get
        {
            int sum = 0;
            foreach (Items items in itemList)
            {
                sum += items.count;
            }
            return sum;
        }
    }
    public Items Get(Item.Type type)
    {
        foreach (Items items in itemList)
        {
            if (items.item != null && items.count > 0 && items.item.itemFunction == type)
            {
                return items;
            }
        }
        return null;
    }

    public bool Contain(Item.Type type, int count = 1)
    {
        int sum = 0;
        foreach (Items items in itemList)
        {
            if (items.item != null && items.item.itemFunction == type)
            {
                sum += items.count;
            }
        }
        if (sum >= count)
            return true;
        else
            return false;
    }

    public bool Contain(Item item, int count = 1)
    {
        int sum = 0;
        foreach (Items items in itemList)
        {
            if(items.item == item)
                sum += items.count;
        }
        if (sum >= count)
            return true;
        else
            return false;
    }

    public bool Contain(Items[] requirements)
    {
        foreach (Items items in requirements)
        {
            int count = GetCount(items.item);
            if (count < items.count)
            {
                return false;
            }
        }
        return true;
    }

    public void Consume(params Items[] requirements)
    {
        foreach (Items items in requirements)
        {
            if(items.item!=null)
                Consume(items.item, items.count);
        }
    }

    public void Consume(int index, int count = 1, bool isPlayer = true)
    {
        if (this[index].item != null)
        {
            this[index].count -= count;
            if (this[index].count > 0)
            {
                return;
            }
            else
            {
                this[index].item = null;
                this[index].count = 0;
            }
        }
        if(ui!=null)
            ui.Refresh();
        if (isPlayer)
            Player.player.SelectedIndex = Player.player.SelectedIndex;
    }

    public void Consume(Item item, int count = 1)
    {
        for(int i=0; i < itemList.Count; i++)
        {
            if(this[i].item == item)
            {
                this[i].count -= count;
                if (this[i].count > 0)
                {
                    return;
                }
                else
                {
                    this[i].item = null;
                    this[i].count = 0;
                }
            }
        }
    }

    public override string ToString()
    {
        if (Count == 0)
            return "无物品";
        string txt = $"物品列表[{Count}]：";
        foreach (Items items in this)
        {
            txt += $"{items.item.name}、";
        }
        txt = txt.Remove(txt.Length - 1) + $"\n总重为{Mass}";
        return txt;
    }

    public IEnumerator GetEnumerator()
    {
        return itemList.GetEnumerator();
    }

    public Items this[Item item]
    {
        get => itemList.Find(delegate (Items items) { return items.item == item; });
    }

    public Items this[int index]
    {
        get => itemList[index];
        set 
        {
            itemList[index] = value;
        }
    }

    public List<Items> Receive(params Items[] itemses)
    {
        List<Items> itemses_return = new List<Items>();
        foreach(Items items in itemses)
        {
            itemses_return.Add(Receive(items));
        }
        return itemses_return;
    }

    public Items Receive(Item item, int count = 1)
    {
        return Receive(new Items(item, count));
    }

    public Items Receive(Items new_items)
    {

        Items return_items = new Items(new_items.item, new_items.count);
        
        if (return_items.item == null) return return_items;
        float mass = maxMass - Mass;
        if (mass <= 0) return return_items;
        int count = (int)(mass / return_items.item.mass);
        if (count < 0) count = maxCount;
        if (count > return_items.count) count = return_items.count;

        for(int i =0;i<itemList.Count;i++)
        {
            if(itemList[i].item == null || itemList[i].item == return_items.item)
            {
                itemList[i].item = return_items.item;
                itemList[i].count += count;
                return_items.count -= count;

                if (this == Player.player.inventory)
                {
                    if (Player.player.SelectedIndex == i)
                        Player.player.SelectedIndex = Player.player.SelectedIndex;
                }
                return return_items;
            }
        }
        return return_items;
    }



    internal int IndexOf(Item selectedItem)
    {
        int i = 0;
        foreach(Items items in this)
        {
            if(items.item.name == selectedItem.name)
            {
                return i;
            }
            i++;
        }
        return -1;
    }

    public void Receive(Inventory new_itemses)
    {
        for (int i = 0; i < new_itemses.itemList.Count; i++)
        {
            Items items = new_itemses.itemList[i];
            Receive(items);
            new_itemses.Consume(items);
        }
    }


    public void Craft(Craft craft)
    {
        if (craftingEnumerator == null)
        {
            if (Contain(craft.requirements))
            {
                craftingEnumerator = Crafting(craft);
                GameManager.instance.StartCoroutine(craftingEnumerator);
            }
        }
    }

    public IEnumerator Crafting(Craft craft)
    {
        Consume(craft.requirements);
        if (ui != null) ui.Refresh();
        craftingTimer = craft.time;
        while (true)
        {
            if (craftingTimer <= 0)
            {
                ShortcutBar.instance.Timer.text = "";
                break;
            }
            else
            {
                craftingTimer -= Time.deltaTime;
                yield return null;
            }
        }

        Receive(craft.products);
        if (ui != null) ui.Refresh();
        craftingEnumerator = null;
    }
}
