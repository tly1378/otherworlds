using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyroscope : Facility
{
    public float moment;
    public SpriteRenderer indicator;
    public SpriteRenderer director;
    public FacilityButton direction;
    public Transform counterweight;

    private bool isWorking;
    private bool isClockwise;
    private Rigidbody2D rb;

    public override void Start()
    {
        base.Start();
        direction.action = Switch;
    }

    public void Switch()
    {
        director.flipY = !director.flipY;
        isClockwise = !isClockwise;
    }


    public override void Use()
    {
        if (OpenConstructionUI()) return;

        if (isWorking)
        {
            ShutDown();
        }
        else
        {
            indicator.color = Color.green;
            rb = transform.parent.GetComponentInParent<Rigidbody2D>();
            if(rb != null)
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
        ShutDown();
    }

    private void ShutDown()
    {
        indicator.color = Color.yellow;
        isWorking = false;
    }

    private void Update()
    {
        if (isWorking)
        {
            if (isClockwise)
            {
                counterweight.Rotate(Vector3.back, 2*360*Time.deltaTime);
                rb.AddTorque(moment);
            }
            else
            {
                counterweight.Rotate(Vector3.forward, 2*360 * Time.deltaTime);
                rb.AddTorque(-moment);
            }
        }
    }
}
