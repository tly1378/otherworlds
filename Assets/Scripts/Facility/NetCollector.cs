using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetCollector : ItemContainer
{
    public SpriteRenderer netRenderer;
    public Sprite open_net;
    public Sprite closed_net;
    private bool isCatching;
    private ItemContainer container;

    public new void Update()
    {
        if (isCatching)
        {
            if (inventory.Count == 0)
            {
                Vector2 point = netRenderer.transform.position;
                float radius = 1f;
                Collider2D[] results = new Collider2D[3];
                ContactFilter2D filter = new ContactFilter2D();
                filter.SetLayerMask(LayerMask.GetMask(nameof(Facility), "OutItem"));
                Physics2D.OverlapCircle(point, radius, filter, results);

                container = null;
                foreach (Collider2D collider in results)
                {
                    if (collider == null)
                    {
                        break;
                    }
                    container = collider.GetComponent<ItemContainer>();
                    if (container != null && container.isResources)
                    {
                        inventory.Receive(container.inventory);
                        container.transform.SetParent(netRenderer.transform);
                        container.GetComponent<Rigidbody2D>().isKinematic = true;
                        container.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        container.transform.localPosition = Vector3.zero;
                        break;
                    }
                }

                netRenderer.sprite = open_net;
                if (coolingTime > 0)
                {
                    coolingTimer -= Time.deltaTime;
                }
                if (coolingTimer <= 0)
                {
                    coolingTimer = coolingTime;
                    RandomPut(potentialItems);
                }
            }
            else
            {
                netRenderer.sprite = closed_net;
            }
        }
    }

    public override bool IsPutable
    {
        get
        {
            if (!base.IsPutable) return false;

            Vector2 point = netRenderer.transform.position;
            float radius = 2f;
            ContactFilter2D contactFilter = new ContactFilter2D();
            Collider2D[] results = new Collider2D[3];

            if(Physics2D.OverlapCircle(point, radius, contactFilter, results) == 0)
            {
                return  true;
            }
            else
            {
                return false;
            }
        }
    }
    public override void Use()
    {
        if (container != null)
        {
            Destroy(container.gameObject);
        }
        if(inventory.Count > 0)
            Player.player.inventory.Receive(inventory);
    }
    public override void Abort()
    {
        isCatching = false;
    }
    public override void EnterRoom(Room room)
    {
        isCatching = true;
    }
    public override void ExitRoom(Room room)
    {
        isCatching = false;
    }
}
