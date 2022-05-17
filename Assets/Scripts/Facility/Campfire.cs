using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : TemperatureSource
{
    [Header(nameof(Campfire))]
    public ParticleSystem fire;
    public GameObject pot;

    public override void Start()
    {
        base.Start();
        fire.Stop();
    }

    public override void Update()
    {
        base.Update();
        if (inventory.craftingTimer <= -5f && fire.isPlaying)
        {
            isWorking = false;
            fire.Stop();
        }
        else if(inventory.craftingTimer > 0 && fire.isStopped)
        {
            isWorking = true;
            fire.Play();
        }
    }
}
