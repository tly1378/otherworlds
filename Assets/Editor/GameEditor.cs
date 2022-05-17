using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

[CustomEditor(typeof(GameManager))]
public class GameEditor : Editor
{
    private void OnEnable()
    {
    //    GameManager.LoadItemsFromJson();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //if (GUILayout.Button("新建/删除物品"))
        //{
        //    GameManager editor = (GameManager)target;
        //    Item.Info itemInfo = editor.information;
        //    if (GameManager.Items.Contains(itemInfo))
        //    {
        //        GameManager.Items.Remove(itemInfo);
        //    }
        //    else
        //    {
        //        GameManager.Items.Add(itemInfo);
        //    }
        //    Debug.Log(ListToString(GameManager.Items));
        //    Setting.PutJson(Setting.itemFilePath, new SerializableList<Item.Info>(GameManager.Items));
        //}

        //if (GUILayout.Button("打印物品列表"))
        //{
        //    Debug.Log(ListToString(GameManager.Items));
        //}
    }

    public static string ListToString<T>(List<T> list)
    {
        string txt = "";
        foreach(T item in list)
        {
            if (item != null)
            {
                txt += item.ToString() + "; ";
            }
        }
        return txt;
    }
}
