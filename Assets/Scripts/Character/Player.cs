using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : Character
{
    const float FORCE = 10;
    public static Player player;

    public AudioSource lowerAS;
    public AudioSource upperAS;

    public CircleCollider2D FasilityArea;
    public List<Craft> crafts;
    public IEnumerator craftingEnumerator;

    private IEnumerator enumerator_moving;
    private IEnumerator enumerator_planning;
    private bool enumerator_planning_should_stop;
    private IEnumerator enumerator_throw;


    public override int SelectedIndex 
    { 
        get => selectedIndex;
        set
        {
            if (selectedIndex == value) print("刷新");

            selectedIndex = value;
            ShortcutBar.instance.UpdateChoose(value);
            Item item = inventory[selectedIndex].item;
            if(item?.itemFunction == Item.Type.弩)
            {
                upper.SetBool("crossbow", true);
            }
            else
            {
                upper.SetBool("crossbow", false);
            }

            foreach (Transform transform in prop) Destroy(transform.gameObject);
            if(item?.itemFunction == Item.Type.道具)
            {
                Instantiate(item.go, prop);
                upper.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                upper.GetComponent<SpriteRenderer>().enabled = true;
                Prop prop = Prop;
                if(prop != null)
                    Destroy(prop.gameObject);
            }
        }
    }


    public override Vector2 Direction
    {
        get
        {
            Vector2 direction = Vector2.zero;
            if (direction.x == 0 && direction.y == 0)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    direction += Vector2.up;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    direction += Vector2.left;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    direction += Vector2.down;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    direction += Vector2.right;
                }
            }
            return direction.normalized;
        }
    }

    public override MoveState State => Input.GetKey(KeyCode.LeftShift) ? MoveState.RUN : MoveState.WALK;

    protected override void Start()
    {
        base.Start();
        ShortcutBar.instance.GetComponent<ShortcutBar>().inventory = inventory;
        player = this;
    }

    protected override void Update()
    {
        if (Time.timeScale == 0) return;
        base.Update();

        //视角旋转
        if (Input.GetKey(KeyCode.P))
        {
            RotateTowards(90 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.L))
        {
            RotateTowards(-90 * Time.deltaTime);
        }

        //角色旋转
        if(enumerator_moving==null)
            MathF.FaceToMouse(body, 1800);
        else
            MathF.FaceToMouse(body, 100);

        //角色移动
        if (rb.isKinematic)
        {
            Vector2 direction = Direction;
            MoveState moveState = State;
            if (direction.x != 0 || direction.y != 0)
            {
                direction = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.z, Vector3.forward) * direction;
                MoveTowards(direction, moveState);
                if(!lowerAS.isPlaying)
                    lowerAS.Play();
            }
            else
            {
                if (lowerAS.isPlaying)
                    lowerAS.Pause();
            }
            MathF.FaceTo(lower.transform, (Vector2)lower.transform.position + direction);
        }
        else
        {
            if (lowerAS.isPlaying)
                lowerAS.Stop();
        }

        //物品相关操作
        if (Input.GetKeyDown(KeyCode.I))//打开背包
        {
            if (UIManager.instance.PlayerUI == null)
            {
                UIManager.instance.PlayerUI = UIManager.instance.OpenPlayUI();
                inventory.ui = UIManager.instance.PlayerUI;
                inventory.ui.title.text = "背包";
            }
            else
            {
                UIManager.instance.PlayerUI.Close();
                UIManager.instance.PlayerUI = null;
            }
        }
        else if (Input.GetKeyDown(KeyCode.F) && inventory[SelectedIndex] != null)//使用物品
        {
            if (inventory[SelectedIndex].count>0)
            {
                if (inventory[SelectedIndex].item.hasEffect)
                {
                    Use(inventory[SelectedIndex].item);
                    inventory.Consume(SelectedIndex);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q) && inventory[SelectedIndex] != null)//丢弃物品
        {

            if (inventory[SelectedIndex].count > 0)
            {
                Item.Type type = inventory[SelectedIndex].item.itemFunction;
                if (enumerator_throw == null)
                {
                    if (type == Item.Type.弩 || type == Item.Type.道具)
                    {
                        enumerator_throw = ThrowImportant(SelectedIndex, 0.25f);
                    }
                    else
                    {
                        enumerator_throw = ThrowImportant(SelectedIndex);
                    }
                    StartCoroutine(enumerator_throw);
                }
            }
            SelectedIndex = SelectedIndex;
        }
        else
        {
            //选择物品
            int scroll = (int)Input.GetAxisRaw("Mouse ScrollWheel");
            if (scroll != 0)//滚动（向前滚是1，向后滚是-1）
            {
                if (
                    !(SelectedIndex == 0 && scroll == 1)//‘位于首位’仍‘向前滚’排除
                    && // 空背包排除
                    !(SelectedIndex == 9 && scroll == -1)//‘位于末位’仍‘向后滚’排除
                    )
                {
                    int newIndex = SelectedIndex - scroll;
                    SelectedIndex = newIndex;
                    if (enumerator_planning != null)
                    {
                        enumerator_planning_should_stop = true;
                    }
                }
            }
        }

        //若当前没有正在执行的其他任务
        if (enumerator_moving == null && enumerator_planning == null)
        {
            CursorManager.Set(0);

            if(inventory[SelectedIndex].item != null)
            {
                //检查是否为弩
                if (inventory[selectedIndex].item.itemFunction == Item.Type.弩)
                {
                    return;
                }

                //检查是否为蓝图
                if (inventory[selectedIndex].item.itemFunction == Item.Type.蓝图)
                {
                    CursorManager.Set(1);
                    GameObject facility_go = inventory[SelectedIndex].item.go;
                    enumerator_planning = Planning(facility_go, inventory[SelectedIndex].item);
                    StartCoroutine(enumerator_planning);
                    return;
                }
            }


            //获取与角色接触的设施
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask(nameof(Facility), "OutItem"));
            Collider2D[] collider2Ds = new Collider2D[5];
            FasilityArea.OverlapCollider(filter, collider2Ds);
            Collider2D collider2D = null;
            Facility facility = null;
            foreach (Collider2D collider in collider2Ds)
            {
                if (collider == null)
                {
                    break;
                }
                facility = collider.GetComponent<Facility>();
                if (facility != null)
                {
                    collider2D = collider;
                    break;
                }
            }

            if (facility != null)
            {
                //鼠标是否悬于设施上方
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                bool x = collider2D.OverlapPoint(mousePosition);
                if (x && !CursorManager.IsOnUI)
                {
                    if (facility != null && facility.moveable && Input.GetMouseButtonDown(1))//是否按下鼠标右键
                    {
                        facility.Abort();
                        enumerator_moving = PickFisility(facility);
                        StartCoroutine(enumerator_moving);
                    }

                    CursorManager.Set(facility);

                    if (Input.GetMouseButtonDown(0))//是否按下鼠标左键 + 是否没有手持任何物品
                    {
                        facility.Use();
                    }
                }
            }

            //道具相关操作
            Prop prop = Prop;
            if (prop != null)
            {
                if (prop.IsFun)
                {
                    prop.Function();
                }

                if (prop.IsEndUse)
                {
                    prop.EndUse();
                }
                else if (prop.Durability > 0)
                {
                    if (prop.IsStartUse)
                    {
                        prop.StartUse();
                    }
                    if (prop.IsUseing)
                    {
                        prop.Useing();
                    }
                }
            }
        }
        else
        {
            CursorManager.Set(1);
        }
    }

    private IEnumerator ThrowImportant(int selectedIndex, float time = 0)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            if (Input.GetKeyUp(KeyCode.Q))
            {
                enumerator_throw = null;
                yield break;
            }
            yield return null;
        }
        ShortcutBar.instance.throwOut_text.color = Color.green;
        ShortcutBar.instance.throwOut.GetComponent<Image>().color = Color.green;

        while (true)
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                //根据物品质量产生反作用力
                StartCoroutine(ThrowIcon(inventory[selectedIndex].item));
                GetComponent<Rigidbody2D>().AddForce(FORCE * inventory[selectedIndex].item.mass * -body.up, ForceMode2D.Impulse);
                inventory.Consume(SelectedIndex);
                enumerator_throw = null;
                ShortcutBar.instance.throwOut_text.color = Color.white;
                ShortcutBar.instance.throwOut.GetComponent<Image>().color = Color.white;
                break;
            }
            yield return null;
        }
    }

    public GameObject throwAway;
    private IEnumerator ThrowIcon(Item item)
    {
        float timer = 1f;
        GameObject throwAway = Instantiate(this.throwAway);
        throwAway.transform.position = transform.position;
        SpriteRenderer sr = throwAway.GetComponent<SpriteRenderer>();
        sr.sprite = item.image;
        
        Vector2 direction = body.transform.up;
        while (timer > 0)
        {
            throwAway.transform.Translate(direction*Time.deltaTime*5, Space.World);
            throwAway.transform.Rotate(Vector3.back, 180 * Time.deltaTime);
            timer -= Time.deltaTime;
            sr.color = new Vector4(1,1,1, sr.color.a-Time.deltaTime);
            yield return null;
        }
        Destroy(throwAway);
    }

    private void Use(Item item)
    {
        item.Use(this);
    }

    private IEnumerator Planning(GameObject sample, Item item)
    {
        GameObject go = Instantiate(sample);
        Facility facility = go.GetComponent<Facility>();
        facility.IsCollidable = false;
        Color green = Color.green;
        green.a = 0.5f;
        Color yellow = Color.yellow;
        yellow.a = 0.5f;
        Color white = Color.white;
        white.a = 1f;
        facility.ChangeColor(Color.green, 100);

        while (true)
        {
            //外部命令
            if (enumerator_planning_should_stop || !inventory.Contain(item))
            {
                enumerator_planning_should_stop = false;
                Destroy(go);
                break;
            }
            //对齐鼠标
            facility.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetKey(KeyCode.R))
            {
                go.transform.Rotate(Vector3.back*0.1f);
            }

            //可放置
            if (facility.IsPutable)
            {
                facility.ChangeColor(Color.green, 0);
                if (Input.GetMouseButtonDown(0) && !CursorManager.IsOnUI)
                {

                    inventory.Consume(inventory[SelectedIndex].item);
                    facility.transform.SetParent(GameManager.Facilities);
                    facility.IsCollidable = true;
                    facility.ChangeColor(Color.white, -100);
                    CursorManager.Set(0);
                    facility.PutDown();
                    break;
                }
            }
            else
            {
                facility.ChangeColor(Color.yellow);
            }
            yield return null;
        }

        enumerator_planning = null;
        CursorManager.Set(0);
    }

    private IEnumerator PickFisility(Facility facility)
    {
        upper.SetBool("carry", true);
        AudioSource audioSource = upper.GetComponent<AudioSource>();
        audioSource.clip = Resources.Load<AudioClip>("Music/SoundEffect/设施离开");
        audioSource.time = 0.35f;
        audioSource.Play();

        //拿起设施
        SpriteRenderer[] spriteRenderers = facility.GetComponentsInChildren<SpriteRenderer>();
        void Take(Facility facility)
        {
            facility.transform.SetParent(FasilityArea.transform);
            facility.GetComponent<Rigidbody2D>().isKinematic = true;
            facility.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            facility.gameObject.layer = LayerMask.NameToLayer("OnCharacter");//令设施不与房屋发生碰撞
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                sr.sortingLayerName = "Character";
                sr.sortingOrder += 100;
            }
            facility.IsCollidable = false;
        }
        Take(facility);


        while (true)
        {
            //松开右键
            if (!Input.GetMouseButton(1))
            {
                upper.SetBool("carry", false);
                if (facility == null)
                {
                    Debug.LogWarning("设施意外消失");
                    break;
                }

                if (facility.transform.IsChildOf(FasilityArea.transform))//确认操作，设施防止意外掉落。
                {
                    print("OUT");
                    //探测是否有障碍物阻挡设施放置
                    facility.IsCollidable = true;
                    if (facility.IsPutable)
                    {
                        CursorManager.Set(0);
                        //若无则：放下设施
                        facility.transform.SetParent(GameManager.Facilities);
                        //放下设施
                        audioSource = upper.GetComponent<AudioSource>();
                        audioSource.clip = Resources.Load<AudioClip>("Music/SoundEffect/设施放置");
                        audioSource.Play();

                        spriteRenderers = facility.GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer sr in spriteRenderers)
                        {
                            sr.sortingLayerName = "Facility";
                            sr.sortingOrder -= 100;
                        }
                        facility.gameObject.layer = LayerMask.NameToLayer("OutItem");
                        facility.GetComponent<Rigidbody2D>().isKinematic = false;
                    }
                    else
                    {
                        //若有则：撤回enable操作
                        facility.IsCollidable = false;
                    }
                }
                else
                {
                    Debug.LogWarning("Fp: "+facility.transform.parent);
                    break;
                }
            }
            yield return null;
        }
        enumerator_moving = null;
    }

    protected override void MoveTowards(Vector2 direction, MoveState moveState)
    {
        if (FasilityArea.transform.childCount > 0)
        {
            Vector3 destination = transform.position;
            Rigidbody2D stuff = FasilityArea.GetComponentInChildren<Rigidbody2D>();
            float debuff = 0.5f;
            if (stuff != null)
            {
                debuff = strength / stuff.mass;
                debuff = debuff > 1 ? 1 : debuff;
            }
            destination += debuff * Time.deltaTime * walkSpeed * (Vector3)direction;
            transform.position = destination;
            //ChangePosition(destination);
        }
        else
        {
            base.MoveTowards(direction, moveState);
        }
    }

    public override void Death()
    {
        Time.timeScale = 0;
        UIManager.instance.OpenUI(UIManager.instance.deathUI);
    }
}
