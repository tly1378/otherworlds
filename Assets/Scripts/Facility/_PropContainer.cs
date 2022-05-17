using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _PropContainer : Facility
{
    public GameObject propObject;
    public int count = int.MaxValue;
    public override void Start()
    {
        base.Start();
    }
    public override void Use()
    {
        foreach (Transform transform in Player.player.prop)
        {
            Destroy(transform.gameObject);
        }
        GameObject newObject = Instantiate(propObject, Player.player.prop);
        newObject.SetActive(true);
        count--;
    }
}
