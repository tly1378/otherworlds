using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class House : MonoBehaviour
{
    public float speed = 500;
    public float rotate = 500;

    private static List<House> buildings;
    private static Rigidbody2D rb;

    private void Start()
    {
        if (buildings == null)
        {
            buildings = new List<House>();
            rb = GetComponent<Rigidbody2D>();
        }
        else
        {
            enabled = false;
        }
        buildings.Add(this);

        Rigidbody2D thisHouse = GetComponent<Rigidbody2D>(); ;
        foreach (Room room in GetComponentsInChildren<Room>())
        {
            thisHouse.mass += room.mass;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            enabled = false;
            int index = (buildings.IndexOf(rb.GetComponent<House>()) + 1) % buildings.Count;
            rb = buildings[index].GetComponent<Rigidbody2D>();
            buildings[index].enabled = true;
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            print("切换至" + rb.name);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.AddForce(speed * transform.up);
            //rb.velocity = transform.right;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.AddForce(speed * -transform.up);
            //rb.velocity = -transform.right;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddTorque(rotate);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddTorque(-rotate);
        }
    }

    private void OnEnable()
    {
        name += "(Ctrl)";
    }

    private void OnDisable()
    {
        name = name.Remove(name.Length - 6);
    }

    public static void Bind(Room.Door door1, Room.Door door2)
    {
        if(door1.room != door2.room)//不在一个房间
        {
            if (door1.target == null && door2.target == null)//两门均没有绑定别的门
            {
                door1.SetTarget(door2.transform);
                door2.SetTarget(door1.transform);

                GameObject bridge = Instantiate(GameManager.instance.Bridge, door1.transform);
                door1.bridge = bridge;
                door2.bridge = bridge;
                bridge.transform.position = (door1.transform.position + door2.transform.position) / 2;

                bridge.transform.up = door1.transform.position - door2.transform.position;

                if (door1.room.transform.parent != door2.room.transform.parent)//两门所在的房间不在一个屋子里
                {
                    Transform abandonedHouse = door2.transform.parent.parent;
                    Transform reservedHouse = door1.transform.parent.parent;
                    //质量转移
                    Rigidbody2D rb = reservedHouse.GetComponent<Rigidbody2D>();
                    rb.mass += abandonedHouse.GetComponent<Rigidbody2D>().mass;
                    //刚体转移
                    Rigidbody2D rigidbody2D = reservedHouse.GetComponent<Rigidbody2D>();
                    foreach (Facility facility in abandonedHouse.GetComponentsInChildren<Facility>())
                    {
                        facility.house_rb = rigidbody2D;
                    }
                    //房间转移
                    List<Transform> rooms = new List<Transform>();
                    foreach(Transform transform in abandonedHouse)
                    {
                        rooms.Add(transform);
                    }
                    foreach (Transform room in rooms)
                    {
                        room.SetParent(reservedHouse);
                    }
                    //销毁空房屋
                    buildings.Remove(abandonedHouse.GetComponent<House>());
                    Destroy(abandonedHouse.gameObject);

                    print("绑定成功");
                }
                else
                {
                    print("绑定成功，但两门属于同一房屋。");
                }

            }
            else
            {
                print("绑定失败，该门已经绑定了其他门。");
            }
        }
        else
        {
            print("无法绑定，两门属于同一房间。");
        }
    }

    //将房间分开
    public static void Split(Room.Door door1, Room.Door door2)
    {
        //两门确实被连接
        if(door1.target == door2.transform)
        {
            //清空连接信息
            door1.target = null; 
            door2.target = null;
            GameObject bridge = door1.bridge;
            door1.bridge = null;
            door2.bridge = null;
            Destroy(bridge);
            //寻找门其中一侧的房间
            Room room1 = door1.room;
            Room room2 = door2.room;
            Queue<Room> waitingRoom = new Queue<Room>();//待查
            HashSet<Room> arrivedRoom = new HashSet<Room>();//可达
            waitingRoom.Enqueue(room1);
            arrivedRoom.Add(room1);
            while (waitingRoom.Count > 0)
            {
                Room currentRoom = waitingRoom.Dequeue();
                //遍历检查每一个门
                foreach(Room.Door door in currentRoom.doors)
                {
                    print(door.target);
                    Room targetRoom;
                    //若门有连接其他房门
                    if (door.target != null)
                    {
                        targetRoom = door.target.GetComponentInParent<Room>();
                        //若该房门与本房门属于同一房间
                        if (targetRoom == room2)
                        {
                            return;
                        }
                        //若该房间没有被添加到可达列表
                        if(!arrivedRoom.Contains(targetRoom))
                        {
                            arrivedRoom.Add(targetRoom);
                            waitingRoom.Enqueue(targetRoom);
                        }
                    }
                }
            }

            //分裂房间
            GameObject newHouse = new GameObject(nameof(House)+"_"+Time.time);
            Rigidbody2D rb = newHouse.AddComponent<Rigidbody2D>();
            newHouse.AddComponent<House>();
            foreach(Room room in arrivedRoom)
            {
                room.transform.SetParent(newHouse.transform);
                rb.mass += room.mass;
            }
            newHouse.BroadcastMessage(nameof(Facility.Abort));
            room2.transform.parent.BroadcastMessage(nameof(Facility.Abort),SendMessageOptions.DontRequireReceiver);

            //刚体转移
            foreach (Facility facility in newHouse.GetComponentsInChildren<Facility>())
            {
                facility.house_rb = rb;
            }

            //rb.velocity = room1.GetComponentInParent<Rigidbody2D>().velocity;
        }
        else
        {
            Debug.LogWarning("解绑失败，两门本就未链接");
        }
    }
}
