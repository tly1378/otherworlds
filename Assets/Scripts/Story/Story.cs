using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Story", menuName = "ScriptableObject/New Story")]
public class Story : ScriptableObject
{
    public Sprite LC;//左立绘
    public Sprite RC;//右立绘
    public string[] Sentence;//文本
    public bool[] Teller;//讲述者：false为左，true为右
}
