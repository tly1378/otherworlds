using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProp : Prop
{
    public GameObject bullet;
    public Transform muzzle;
    public int maxBulletNum;
    public float coolingTime;
    public bool mustPull;
    private float coolingTimer;

    public void Fire()
    {
        if(coolingTimer < 0)
        {
            coolingTimer = coolingTime;
            Instantiate(bullet, muzzle).transform.SetParent(null);
            Durability -= 1;
        }
    }

    public override void StartUse()
    {
        Fire();
    }

    public override void Useing()
    {
        if (!mustPull) Fire();
    }

    public override bool IsFun => Input.GetKeyDown(KeyCode.R);
    public override void Function()
    {
        if (Durability <= 0)
        {
            Durability += maxBulletNum;
        }
    }

    private void Update()
    {
        if (coolingTime >= 0)
        {
            coolingTimer -= Time.deltaTime;
        }
    }
}
