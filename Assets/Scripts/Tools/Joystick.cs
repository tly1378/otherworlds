using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Vector2 joystick;

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        joystick = transform.position - transform.parent.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
        joystick = Vector2.zero;
    }

    GUIStyle fontStyle;
    private void Start()
    {
        fontStyle = new GUIStyle();
        fontStyle.normal.textColor = Color.white;
        fontStyle.fontSize = 100;
    }
}
