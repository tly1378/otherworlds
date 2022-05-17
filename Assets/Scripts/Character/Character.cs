using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class Character : MonoBehaviour
{
    //系统属性
    [HideInInspector()]
    public Rigidbody2D rb;
    public Transform body;
    public Animator upper;
    public Animator lower;
    protected CircleCollider2D characterCollider;
    protected bool isTouching;

    [Header("通用设置")]
    public Transform prop;
    public float walkSpeed;
    public float runSpeed;

    [Header("玩家属性")]
    public float strength;
    public float maxHP;
    private float hp;
    public float maxHV;
    private float hv;
    public float hungerRate;
    public float temperature;

    //道具
    public Prop Prop
    {
        get
        {
            Prop p = prop.GetComponentInChildren<Prop>();
            return p;
        }
    }

    //物品
    public Inventory inventory;
    protected int selectedIndex = 0;
    [HideInInspector()]
    public Item arrow;

    bool alive;
    public float HP
    {
        get => hp;
        set
        {
            if (value >= maxHP)
            {
                hp = maxHP;
            }
            else if (value <= 0)
            {
                hp = 0;
                if (alive)
                {
                    alive = false;
                    Death();
                }
            }
            else
            {
                hp = value;
            }
        }
    }

    public float HV
    {
        get => hv;
        set
        {
            if (value > maxHV)
            {
                hv = maxHV;
            }
            else if (value < 0)
            {
                hv = 0;
            }
            else
            {
                hv = value;
            }
        }
    }

    protected virtual void Start()
    {
        alive = true;
        if (inventory == null)
            inventory = Inventory.New(25);
        rb = GetComponent<Rigidbody2D>();
        characterCollider = GetComponent<CircleCollider2D>();
        if (characterCollider.offset != Vector2.zero)
        {
            Debug.LogError("角色的碰撞体必须位于角色中心");
            characterCollider.offset = Vector2.zero;
        }

        hp = maxHP;
        hv = maxHV;
    }

    public virtual Vector2 Direction
    {
        get
        {
            return Vector2.zero;
        }
    }

    public virtual MoveState State
    {
        get
        {
            return MoveState.WALK;
        }
    }

    public float Temperature 
    {
        get
        {
            Room room = GetComponentInParent<Room>();
            if (room == null) return -20;
            return room.Temperature;
        }
    }

    public virtual int SelectedIndex { get => selectedIndex; set => selectedIndex = value; }
    internal bool Attack { get => Input.GetMouseButton(0); }
    internal bool Charge { get => Input.GetMouseButton(1); }

    protected virtual void Update()
    {
        if (HV <= 0)
        {
            HP -= Time.deltaTime;
        }

        if (HV > 0)
        {
            HV -= hungerRate * Time.deltaTime;
        }


        temperature += (Temperature - temperature) * 0.05f * Time.deltaTime;
        if (temperature <= 0)
        {
            HP += temperature * Time.deltaTime / 10;
        }

        //角色移动
        Vector2 direction = Direction;
        MoveState moveState = State;
        if (rb.isKinematic)
        {
            if (direction != Vector2.zero)
            {
                upper.SetBool("walk", true);
                lower.SetBool("walk", true);
            }
        }
        if (direction == Vector2.zero)
        {
            upper.SetBool("walk", false);
            lower.SetBool("walk", false);
        }
    }

    public virtual void Death() { }

    protected virtual void LateUpdate()
    {
        //当角色处于其他碰撞体内执行该段，本段在于实现简易的搓行效果（无摩擦力）。
        Collider2D[] collider2D = Physics2D.OverlapCircleAll(
            transform.position,
            transform.localScale.x * characterCollider.radius,
            LayerMask.GetMask(nameof(House), nameof(Facility))
            );
        if (collider2D.Length == 0)
        {
            isTouching = false;
        }
        else
        {
            isTouching = true;
            for (int i = 0; i < collider2D.Length; i++)
            {
                if (collider2D[i] != null && collider2D[i].enabled && !collider2D[i].isTrigger)
                {
                    if (collider2D[i] is BoxCollider2D boxCollider)
                    {
                        transform.position += (Vector3)MathF.SqueezeOut(characterCollider, boxCollider);
                    }
                    else if (collider2D[i] is CircleCollider2D circleCollider)
                    {
                        transform.position = MathF.SqueezeOut(characterCollider, circleCollider);
                    }
                }
            }
        }
    }

    public enum MoveState
    {
        WALK,
        RUN
    }

    protected void RotateTowards(float angle) => Camera.main.transform.Rotate(0, 0, angle, Space.Self);

    protected virtual void MoveTowards(Vector2 direction, MoveState moveState)
    {
        Vector2 destination = transform.position;
        if (moveState == MoveState.WALK)
        {
            destination += Time.deltaTime * walkSpeed * direction;
            transform.position = destination;
        }
        else
        {
            destination += Time.deltaTime * runSpeed * direction;
            transform.position = destination;
        }
    }
}
