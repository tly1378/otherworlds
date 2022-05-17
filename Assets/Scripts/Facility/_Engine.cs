using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Engine : Facility
{
    public float thrust;
    public SpriteRenderer indicator;

    private bool isWorking;
    public Rigidbody2D rb;

    public override void Use()
    {
        if (isWorking)
        {
            indicator.color = Color.yellow;
            isWorking = false;
        }
        else
        {
            indicator.color = Color.green;
            rb = transform.parent.GetComponentInParent<Rigidbody2D>();
            if (rb != null)
            {
                isWorking = true;
            }
            else
            {
                indicator.color = Color.red;
            }
        }
    }

    public override void Abort()
    {
        indicator.color = Color.yellow;
        isWorking = false;
    }

    public override void Start()
    {
        base.Start();
    }
    private void Update()
    {
        if (isWorking&& rb != null)
        {
            rb.AddForce(thrust * transform.right);
            if (rb.velocity.magnitude > 0.5f)
            {
                rb.velocity = rb.velocity.normalized * 0.5f;
            }
        }
    }
}
