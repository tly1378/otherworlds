using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Prop : MonoBehaviour
{
    public float durability;
    public bool isDisposable;
    private Character character;

    private void Start()
    {
        character = GetComponentInParent<Character>();
    }

    public float Durability
    {
        get => durability;
        set
        {
            durability = value;
            if(durability <= 0)
            {
                NoDurability();
                if (isDisposable)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private AudioSource source;
    public AudioClip startClip;
    public virtual bool IsStartUse { get => Input.GetMouseButtonDown(0); }
    public virtual void StartUse() 
    {
        source.clip = startClip;
        source.Play();
    }

    public AudioClip usingClip;
    public virtual bool IsUseing { get => Input.GetMouseButton(0); }
    public virtual void Useing() 
    {
        source.clip = usingClip;
        source.loop = true;
        source.Play();
    }

    public AudioClip endClip;
    public virtual bool IsEndUse { get => Input.GetMouseButtonUp(0); }
    public virtual void EndUse() 
    {
        source.clip = endClip;
        source.Play();
    }

    public virtual bool IsFun { get => Input.GetKeyDown(KeyCode.E); }
    public virtual void Function() { }

    public virtual void NoDurability() { }


    public string information;
    private void OnGUI()
    {
        if (transform.parent.CompareTag(nameof(Player)) && !Input.GetMouseButton(1))
        {
            GUILayout.BeginArea(new Rect(Screen.width - 100, 0, 100, 500));
            GUILayout.Label(information);
            GUILayout.EndArea();
        }
    }
}
