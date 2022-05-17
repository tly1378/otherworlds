using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Floor : MonoBehaviour
{
    Rigidbody2D house_rb;
    Room room;

    private void Start()
    {
        tag = "Floor";
        house_rb = GetComponentInParent<Rigidbody2D>();
        room = GetComponentInParent<Room>();
    }

    private IEnumerator ReseteRotation(Transform trans, Transform target)
    {
        float trans_localRotation = trans.rotation.eulerAngles.z - target.rotation.eulerAngles.z;
        float start = trans_localRotation;
        float end = (int)(trans_localRotation / 90) * 90;
        if (trans_localRotation % 90 > 45)
        {
            end += 90;
        }
        float interpolationValue = 0;
        while (true)
        {
            trans_localRotation = Mathf.LerpAngle(start, end, (interpolationValue += Time.deltaTime * 5)) + target.rotation.eulerAngles.z;
            trans.rotation = Quaternion.Euler(0, 0, trans_localRotation);
            if (interpolationValue >= 1)
                break;
            yield return null;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(nameof(Character)) || collision.CompareTag(nameof(Player)) || collision.CompareTag(nameof(Facility)))
        {
            if (collision.transform.parent == null || collision.transform.IsChildOf(Player.player.FasilityArea.transform))
            {
                Debug.LogWarning(collision.name);
                return;
            }

            Player player = collision.GetComponent<Player>();
            //若物体本就在房间中
            if (collision.transform.parent.CompareTag(nameof(Floor)))
            {
                // 若物体是玩家
                if (player != null)
                {
                    //若两块Float不在同一房间
                    if (player.transform.parent.parent != transform.parent.parent)
                        //旋转视野
                        StartCoroutine(ReseteRotation(Camera.main.transform, transform));
                }
                return;
            }

            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb == null) return;
            rb.isKinematic = true;
            rb.transform.SetParent(transform);
            rb.velocity = Vector2.zero;

            if (collision.CompareTag("Character") || collision.CompareTag("Player"))
            {
                //令角色不与房屋发生碰撞
                collision.gameObject.layer = LayerMask.NameToLayer("Character");
                if (player != null)
                {
                    //旋转视野
                    StartCoroutine(ReseteRotation(Camera.main.transform, transform));
                }
            }
            else
            {
                //令设施不与房屋发生碰撞
                Facility facility = collision.GetComponent<Facility>();
                facility.house_rb = house_rb;
                facility.EnterRoom(room);
                collision.gameObject.layer = LayerMask.NameToLayer("Facility");
            }
        }
    }

    const int SIZE = 5;
    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag(nameof(Character)) || collider.CompareTag(nameof(Player)) || collider.CompareTag(nameof(Facility)))
        {
            if (collider.transform.parent != transform)
            {
                return;
            }
            else if (collider.IsTouchingLayers(LayerMask.GetMask(nameof(Floor))))
            {
                //离开了地面，但仍在其他地面内。
                ContactFilter2D filter = new ContactFilter2D();
                Collider2D[] colliders = new Collider2D[SIZE];
                collider.OverlapCollider(filter.NoFilter(), colliders);
                for (int i = 0; i < SIZE; i++)
                {
                    if (colliders[i] == null)
                    {
                        break;
                    }
                    if (colliders[i].CompareTag(nameof(Floor)))
                    {
                        collider.transform.SetParent(colliders[i].transform);
                        colliders[i].GetComponent<Floor>().OnTriggerEnter2D(colliders[i]);
                        break;
                    }
                }
                return;
            }

            //令角色/设施会与房屋发生碰撞
            collider.gameObject.layer = LayerMask.NameToLayer("OutItem");
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
            rb.isKinematic = false;
            //若是设施
            Facility facility = collider.GetComponent<Facility>();
            if (facility != null)
            {
                facility.ExitRoom(room);
                facility.house_rb = null;
                rb.transform.SetParent(GameManager.Facilities);
                return;
            }


            //玩家出仓速度
            if (collider.CompareTag(nameof(Player)))
            {
                Player player = collider.GetComponent<Player>();
                rb.velocity = Camera.main.transform.TransformDirection(Player.player.Direction) * (Input.GetKey(KeyCode.LeftShift) ? player.runSpeed : player.walkSpeed) / 3;
            }
            if (collider.CompareTag(nameof(Character)))
            {
                NPC npc = collider.GetComponent<NPC>();
                if(npc!=null)
                    rb.velocity = npc.transform.TransformDirection(npc.direction) * npc.walkSpeed / 5;
            }
            rb.transform.SetParent(GameManager.Characters);
        }
    }
}
