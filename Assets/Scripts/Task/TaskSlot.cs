using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskSlot : MonoBehaviour
{
    public Task task;
    public Text introduce;

    // Update is called once per frame
    void Update()
    {
        introduce.text = task.introduce;
    }

    public void ChangeNowTask() 
    {
        GetComponentInParent<TaskManager>().nowTask = task;
    }
}
