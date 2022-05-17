using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Task", menuName = "ScriptableObject/New Task")]
public class Task : ScriptableObject
{
    public int taskType;//0拥有一定数量物体;1完成一定数量建筑的建造;2到达指定地点;
    public GameObject taskObject;//所需建筑
    public Item needItem;//所需物品
    public int needNum;//需要的数量
    public int nowNum;//现有数量
    public string introduce;//介绍
    public Task nextTask;//下一个任务
    public float pointNum;//目标点标号
    public Story TaskStory;//触发剧情
}
