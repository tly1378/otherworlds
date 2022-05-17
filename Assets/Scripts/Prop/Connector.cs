using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : Prop
{
    public float radius;

    private List<Room.Door> GetDoors(float radius)
    {
        List<Room.Door> doors = new List<Room.Door>();
        foreach (Room.Door door in Room.alldoors)
        {
            if ((door.transform.position - transform.position).sqrMagnitude < radius * radius)
            {
                doors.Add(door);
            }
        }
        return doors;
    }

    public override bool IsStartUse => Input.GetKeyDown(KeyCode.E);
    public override void StartUse()
    {
        List<Room.Door> doors = GetDoors(radius);
        if (doors.Count >= 2)
        {
            if (doors.Count == 2)
            {
                House.Bind(doors[0], doors[1]);
                Durability -= 1;
                Character character = GetComponentInParent<Character>();
                character.upper.GetComponent<SpriteRenderer>().enabled = true;
                character.SelectedIndex = character.SelectedIndex;
            }
            else
            {
                Debug.LogWarning("Ambiguity");
            }
        }
    }

    public override bool IsFun => Input.GetKeyDown(KeyCode.R);
    public override void Function()
    {
        List<Room.Door> doors = GetDoors(radius);
        if (doors.Count >= 2)
        {
            if (doors.Count == 2)
            {
                House.Split(doors[0], doors[1]);
            }
            else
            {
                Debug.LogWarning("Ambiguity");
            }
        }
    }
}
