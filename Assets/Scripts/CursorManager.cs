using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    public Vector2 offsize;
    public Texture2D[] cursors;
    private static int index;

    public static bool IsOnUI { get => CheckMousse(); }

    private void Start()
    {
        Cursor.SetCursor(cursors[0], offsize, CursorMode.Auto);
    }

    private void LateUpdate()
    {
        Cursor.SetCursor(cursors[index], offsize, CursorMode.Auto);
    }

    public static void Set(int index)
    {
        CursorManager.index = index;
    }

    public static void Set(Facility facility)
    {
        if(facility is ItemContainer)
        {
            index = 2;
        }
        else
        {
            index = 0;
        }
    }

    private static bool CheckMousse()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.layer == 5)
            {
                return true;
            }
        }
        return false;
    }
}
