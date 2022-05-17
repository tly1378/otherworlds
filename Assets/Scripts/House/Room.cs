using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static List<Door> alldoors = new List<Door>();
    [HideInInspector()]
    public List<Door> doors = new List<Door>();


    public float mass;
    public float temperature;
    public List<TemperatureSource> temperatureSources;
    public float Temperature 
    { 
        get 
        {
            float temperature = this.temperature;
            foreach(TemperatureSource temperatureSource in temperatureSources)
            {
                if(temperatureSource.isWorking)
                    temperature += temperatureSource.temperature;
            }
            return temperature; 
        } 
    }


    public class Door
    {
        [HideInInspector]
        public Room room;
        public Transform transform;
        public Transform target;
        public GameObject bridge;
        public void SetTarget(Transform transform)
        {
            target = transform;
        }
    }
    private void Start()
    {
        foreach(Transform transform in transform)
        {
            if (transform.CompareTag(nameof(Door)))
            {
                Door door = new Door
                {
                    room = this,
                    transform = transform
                };
                alldoors.Add(door);
                doors.Add(door);
            }
        }
    }
}
