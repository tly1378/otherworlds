using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityButton : Facility
{
    public Action action;
    public override void Start()
    {
        base.Start();
    }
    public override void Use()
    {
        action();
    }
}
