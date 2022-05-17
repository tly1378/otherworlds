using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class Facility : MonoBehaviour
{
    [Serializable]
    public struct UpgradeInfo
    {
        public string title;
        public string info;
        public GameObject target;
        public Items[] requirements;

        public string requirementsString()
        {
            string info = "";
            foreach (Items items in requirements)
            {
                info += items.item.name + "*" + items.count + " ";
            }
            return info;
        }
    }

    [Header(nameof(Facility))]
    public string information;
    public bool moveable = true;
    [SerializeField]
    public List<UpgradeInfo> upgrades;
    public Transform[] facilities;
    public AudioSource audioSource;

    [HideInInspector()]
    public Collider2D[] colliders;
    [HideInInspector()]
    public Rigidbody2D house_rb;
    protected Room room;


    public void PutDown()
    {
        //将绑定的其他设施释放
        foreach (Transform facility in facilities)
        {
            facility.SetParent(GameManager.Facilities);
            facility.GetComponent<Facility>().colliders = facility.GetComponentsInChildren<Collider2D>();
        }
        colliders = GetComponentsInChildren<Collider2D>();
    }

    public virtual void Start()
    {
        name = name.Replace("(Clone)", "");
        tag = "Facility";
        colliders = GetComponentsInChildren<Collider2D>();

        //将升级信息载入文本提示
        if (upgrades.Count>0)
        {
            information += "\r\n";
            int i = 1;
            foreach(UpgradeInfo upgrade in upgrades)
            {
                information += "("+i+++") ";
                information += upgrade.target.name+": ";
                foreach(Items items in upgrade.requirements)
                {
                    information += items.item.name + "*" + items.count + ", ";
                }
                if(information.EndsWith(", "))
                    information = information.Remove(information.Length-2, 2);
                information += "\r\n";
            }
        }
    }

    public Facility Upgrade(int num, Inventory inventory)
    {
        if (num < upgrades.Count)
        {
            if (inventory.Contain(upgrades[num].requirements))
            {
                inventory.Consume(upgrades[num].requirements);
                GameObject newFacility = Instantiate(upgrades[num].target, transform.parent);
                Facility facility = newFacility.GetComponent<Facility>();
                newFacility.transform.localPosition = transform.localPosition;
                newFacility.transform.localRotation = transform.localRotation;

                Rigidbody2D rb = newFacility.GetComponent<Rigidbody2D>();
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
                facility.house_rb = house_rb;
                facility.EnterRoom(room);
                ExitRoom(room);
                newFacility.layer = LayerMask.NameToLayer("Facility");//令设施不与房屋发生碰撞
                
                Destroy(gameObject);
                return facility;
            }
        }
        return null;
    }

    public bool IsCollidable
    {
        set
        {
            if (value)
            {
                gameObject.layer = LayerMask.NameToLayer("OutItem");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("OnCharacter");
            }

            foreach (Collider2D collider in colliders)
            {
                collider.enabled = value;
            }

            foreach(Transform transform in facilities)
            {
                transform.GetComponent<Facility>().IsCollidable = value;
            }
        }
    }

    public virtual bool IsPutable
    {
        get
        {
            foreach (Collider2D collider in colliders)
            {
                Collider2D[] result = new Collider2D[5];
                collider.OverlapCollider(new ContactFilter2D(), result);
                if (result.ToList().Find(
                    delegate (Collider2D c)
                    {
                        if (
                            c != null
                            && (c.CompareTag(nameof(House)) || c.CompareTag(nameof(Facility)))
                            && c.gameObject != collider.gameObject
                        )
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }) != null)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public virtual void EnterRoom(Room room) { }

    public virtual void ExitRoom(Room room) { }

    public virtual void Use()
    {
        OpenConstructionUI();
    }

    public bool OpenConstructionUI() 
    {
        if (this is ItemContainer container && container.isResources) 
            return false;

        Item item = Player.player.inventory[Player.player.SelectedIndex].item;
        if (item != null && item.itemFunction == Item.Type.改造)
        {
            if (upgrades.Count > 0)
            {
                ConstructionUI ui = UIManager.instance.OpenConstructionUI(this);
                return true;
            }
        }
        return false;
    }

    public virtual void Abort() { }

    private void OnGUI()
    {
        if (transform.parent!=null && transform.parent.CompareTag(nameof(Player)) && Input.GetMouseButton(1))
        {
            GUILayout.BeginArea(new Rect(Screen.width - 100, 0, 100, 500));
            GUILayout.Label(information);
            GUILayout.EndArea();
        }
    }

    public void ChangeColor(Color color, int sortingOrder = 0)
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.sortingOrder += sortingOrder;
            sr.color = color;
        }
    }
}
