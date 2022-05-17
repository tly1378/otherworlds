using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoutableProp : Prop
{
    public float force;

    public ParticleSystem particle;
    private Rigidbody2D rb;

    private void Start()
    {
        particle.Stop();
    }

    public override bool IsStartUse => Input.GetKeyDown(KeyCode.E);
    public override void StartUse()
    {
        particle.Play();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public override bool IsUseing => Input.GetKey(KeyCode.E);
    public override void Useing()
    {
        if (rb != null)
        {
            rb.AddForce(-transform.up * force * Time.deltaTime);
            Durability -= Time.deltaTime;
        }
    }

    public override void NoDurability()
    {

        //Inventory inventory = GetComponentInParent<Character>().inventory;
        //Items items = inventory.Get(Item.Type.气罐);
        //if (items != null && items.have)
        //{
        //    Durability += 10 * items.item.mass;
        //    inventory.Consume(items.item);
        //}
        //else
        //{
        //    particle.Stop();
        //}
    }

    public override bool IsEndUse => Input.GetKeyUp(KeyCode.E);
    public override void EndUse()
    {
        if (particle.isPlaying)
        {
            particle.Stop();
        }
    }
}
