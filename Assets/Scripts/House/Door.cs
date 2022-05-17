using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Door : MonoBehaviour
{
    Rigidbody2D house_rb;
    Transform target_transform;
    public GameObject bridge;

    private void Start()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(nameof(Door)))
        {
            house_rb = GetComponentInParent<Rigidbody2D>();
            target_transform = collision.transform;
        }
    }

    //private void Update()
    //{
    //    if (target_transform != null)
    //    {
    //        if (house_rb == null)
    //        {
    //            target_transform = null;
    //            return;
    //        }
    //        house_rb.AddForceAtPosition((target_transform.position-transform.position).normalized * 20, transform.position);
    //    }
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(nameof(Door)))
        {
            target_transform = null;
        }
    }
}
