using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Item",menuName = "ScriptableObject/New Item")]
public class Item: ScriptableObject
{
    public enum Type
    {
        可使用 = 0,
        不可用 = 1,
        改造 = 2,
        蓝图 = 3,
        道具 = 4,
        气罐 = 6,
        弩 = 7,
        箭 = 8
    }

    public Sprite image;
    public float mass;
    public float hp;
    public float hv;
    public float tp;

    [TextArea] 
    public string introduce;
    public Type itemFunction;
    public GameObject go;
    public AudioClip clip;

    public bool hasEffect 
    { 
        get 
        {
            if (hv != 0 || hp != 0 || tp != 0) return true;
            return false;
        } 
    }

    public override string ToString() => name;

    public void Use(Character character)
    {
        character.HP += hp;
        character.HV += hv;
        character.temperature += tp;
        AudioSource audioSource = character.upper.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
    }
}