using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableList<T>
{
    [SerializeField]
    List<T> list;

    public List<T> ToList() { return list; }

    public SerializableList(List<T> list)
    {
        this.list = list;
    }
}
