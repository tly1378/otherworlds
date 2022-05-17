using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public GameObject facility;
    public GameObject points;
    public Task[] taskList;
    public Task nowTask;
    public Text nowTaskTxt;
    static TaskManager instance;
    public GameObject slotGrid;
    public GameObject emptySlot;
    public List<GameObject> slots = new List<GameObject>();
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (nowTask != null)
        {
            nowTaskTxt.text = nowTask.introduce;
        }
        foreach (Task task in taskList) 
        {
            switch (task.taskType)
            {
                case 0:
                    foreach (Items items in Player.player.inventory.itemList) 
                    {
                        if (items.item == task.needItem) 
                        {
                            task.nowNum+= items.count;
                        }
                    }
                    if (task.nowNum >= task.needNum)
                    {
                        if (task.nextTask != null)
                        {
                            AddNewTask(task.nextTask);
                        }
                        finishTask(task);
                    }
                    break;
                case 1:
                    foreach (Transform child in facility.transform) 
                    {
                        if (child.name == task.taskObject.name) 
                        {
                            task.nowNum++;
                        }
                    }
                    if (task.nowNum >= task.needNum)
                    {
                        if (task.nextTask != null)
                        {
                            AddNewTask(task.nextTask);
                        }
                        finishTask(task);
                    }
                    break;
                case 2:
                    foreach (Transform child in points.transform)
                    {
                        if (child.name == task.pointNum.ToString())
                        {
                            GameObject player = GameObject.Find("Player");
                            if (Vector3.Distance(player.transform.position,child.transform.position)<=1f)
                            {
                                if (task.nextTask != null)
                                {
                                    AddNewTask(task.nextTask);
                                }
                                finishTask(task);
                            }
                        }
                    }
                    break;
            }
        }
    }

    public void reflashTaskList() 
    {
        for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            if (instance.slotGrid.transform.childCount == 0)
                break;
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
            instance.slots.Clear();
        }

        for (int i = 0; i < instance.taskList.Length; i++)
        {
            instance.slots.Add(Instantiate(instance.emptySlot));
            instance.slots[i].transform.SetParent(instance.slotGrid.transform);
            instance.slots[i].GetComponent<TaskSlot>().task = instance.taskList[i];
        }

        foreach (Task task in taskList)
        {
                switch (task.taskType)
                {
                    case 0:
                        task.introduce = "获得" + task.needNum + "个" + task.needItem.name + "(" + task.nowNum + "/" + task.needNum + ")";
                        break;
                    case 1:
                        task.introduce = "建造" + task.needNum + "个" + task.taskObject.name + "(" + task.nowNum + "/" + task.needNum + ")";
                        break;
                    case 2:
                        break;
                }
        }
    }

    private int temp;
    public void finishTask(Task task) 
    {
        if (task.TaskStory != null)
        {
            GameObject.Find("Canvas").GetComponent<StoryManager>().StartNewStory(task.TaskStory);
        }
        if (nowTask == task) 
        {
            nowTask = null;
            nowTaskTxt.text = "";
        }
        for (int i = 0; i < taskList.Length; i++) 
        {
            if (taskList[i] == task) 
            {
                temp = i;
            }
        }
        for (int j = temp+1; j < taskList.Length; j++) 
        {
            taskList[j - 1] = taskList[j];
        }
        Task[] tasks = new Task[taskList.Length - 1];
        for (int i = 0; i < taskList.Length - 1; i++) 
        {
            tasks[i] = taskList[i];
        }
        taskList = tasks;
    }

    public void AddNewTask(Task task) 
    {
        Task[] tasks = new Task[taskList.Length + 1];
        for (int i = 0; i < taskList.Length; i++) 
        {
            tasks[i] = taskList[i];
        }
        tasks[taskList.Length] = task;
        taskList = tasks;
    }
    
}
