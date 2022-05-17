using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemsUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Inventory originalInventory;
    public Transform originalParent;
    private int originalItemIndex;
    public ContainerUI ui;

    public Image image;
    public Image background;
    public TMPro.TextMeshProUGUI count;

    public bool indraggable;

    private void Start()
    {
        ui = GetComponentInParent<ContainerUI>();
        originalInventory = ui.inventory;
    }

    public void SetItems(Items items)
    {
        if (items.have)
        {
            image.enabled = true;
            image.sprite = items.item.image;
            if(items.item.itemFunction == Item.Type.蓝图)
            {
                background.enabled = true;
                image.transform.localScale = Vector3.one * 0.65f;
            }
            else
            {
                background.enabled = false;
            }
            count.text = items.count.ToString();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (indraggable) return;
        originalParent = transform.parent;
        originalItemIndex = originalParent.GetComponent<Slot>().slotIndex;

        transform.SetParent(GameManager.instance.Canvas);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (indraggable) return;
        transform.position = eventData.position;
    }

    //private void FromTo(Inventory from, int f, Inventory to, int t)
    //{
    //    if(from[f].item != to[t].item)
    //    {
    //        Items temp = from[f];
    //        from[f] = to[t];
    //        to[t] = temp;
    //    }
    //    else
    //    {
    //        from.TransferTo(f, to, t);
    //    }
    //}

    public void OnEndDrag(PointerEventData eventData)
    {
        if (indraggable) return;
        if (eventData.pointerCurrentRaycast.gameObject != null 
            && eventData.pointerCurrentRaycast.gameObject.layer == 5 
            )
        {
            GameObject targetGO = eventData.pointerCurrentRaycast.gameObject;
            Slot slot = targetGO.GetComponentInParent<Slot>();
            if (slot != null)
            {
                int targetIndex = slot.slotIndex;
                ContainerUI targetIM = targetGO.GetComponentInParent<ContainerUI>();
                Inventory targetBag = targetIM.inventory;

                if (targetGO.name == "ItemImage")
                {
                    Item item = originalInventory[originalItemIndex].item;

                    if(originalInventory[originalItemIndex].item == targetBag[targetIndex].item)
                    {
                        transform.SetParent(targetGO.transform);
                        transform.position = targetGO.transform.position;

                        targetBag[targetIndex].item = originalInventory[originalItemIndex].item;
                        targetBag[targetIndex].count += originalInventory[originalItemIndex].count;

                        originalInventory[originalItemIndex].item = null;
                        originalInventory[originalItemIndex].count = 0;
                    }
                    else
                    {
                        //将自身与目标交换位置
                        //自身移至目标
                        transform.SetParent(targetGO.transform.parent.parent);
                        transform.position = targetGO.transform.parent.parent.position;
                        //交换item
                        originalInventory[originalItemIndex].item = targetBag[targetIndex].item;
                        targetBag[targetIndex].item = item;
                        //交换count
                        int count = originalInventory[originalItemIndex].count;
                        originalInventory[originalItemIndex].count = targetBag[targetIndex].count;
                        targetBag[targetIndex].count = count;
                        //目标移至自身
                        targetGO.transform.parent.SetParent(originalParent);
                        targetGO.transform.parent.position = originalParent.position;
                    }
                    GetComponent<CanvasGroup>().blocksRaycasts = true;
                    ui.Refresh();
                    targetIM.Refresh();
                    return;
                }

                if (eventData.pointerCurrentRaycast.gameObject.name == "Slot(Clone)")
                {
                    transform.SetParent(targetGO.transform);
                    transform.position = targetGO.transform.position;

                    targetBag[targetIndex].item = originalInventory[originalItemIndex].item;
                    targetBag[targetIndex].count = originalInventory[originalItemIndex].count;
                    if (targetIndex != originalItemIndex || originalInventory != targetBag)
                    {
                        originalInventory[originalItemIndex].item = null;
                        originalInventory[originalItemIndex].count = 0;
                    }

                    GetComponent<CanvasGroup>().blocksRaycasts = true;

                    ui.Refresh();
                    targetIM.Refresh();
                    return;
                }

            }
        }

        transform.SetParent(originalParent);
        transform.position = originalParent.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        originalItemIndex = transform.parent.GetComponent<Slot>().slotIndex;
        Item originalItem = originalInventory[originalItemIndex].item;
        if (originalItem != null)
        {
            CraftUI inventoryUI = GetComponentInParent<CraftUI>();
            if (inventoryUI.itemInformation != null)
            {
                inventoryUI.itemInformation.text = originalItem.name + "\n"+ originalItem.introduce;
            }
        }
    }
}
