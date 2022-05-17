using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Craft", menuName = "ScriptableObject/New Craft")]
public class Craft : ScriptableObject
{
    public Items[] requirements;
    public Items[] products;
    public float time;

    public override string ToString()
    {
        string info = "";
        //foreach (Items items in products)
        //{
        //    info += items.item.name + "*" + items.count + "+";
        //}
        //info = info.Remove(info.Length -1);
        //info += "=";
        foreach (Items items in requirements)
        {
            info += items.item.name + "*" + items.count + " ";
        }
        if(info.Length>0)
            info = info.Remove(info.Length - 1);
        return info;
    }
}
