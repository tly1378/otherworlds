using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureSource : ItemContainer
{
    [Header(nameof(TemperatureSource))]
    public float temperature;
    public bool isWorking;

    public override void EnterRoom(Room room)
    {
        this.room = room;
        room.temperatureSources.Add(this);
    }

    public override void ExitRoom(Room room)
    {
        room.temperatureSources.Remove(this);
    }
}
