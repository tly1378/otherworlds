using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIForm : MonoBehaviour, IDragHandler
{
    public AnimationCurve startCurve;
    public AnimationCurve endCurve;
    public RectTransform isMovable;
    public TMPro.TextMeshProUGUI title;

    public virtual void Start()
    {
        StartCoroutine(OpenEffect(GetComponent<RectTransform>(), startCurve));
    }

    private IEnumerator OpenEffect(RectTransform transform, AnimationCurve curve)
    {
        Vector3 target = transform.localScale;
        float startTime = Time.time;
        float endTime = curve.keys[curve.length - 1].time;
        while (true)
        {
            float time = Time.time - startTime;
            if (time > endTime) break;
            transform.localScale = target * curve.Evaluate(time);
            yield return null;
        }
    }

    public void OnDrag(PointerEventData eventData) 
    { 
        if(isMovable != null)
            isMovable.anchoredPosition += eventData.delta; 
    }

    public virtual void Close()
    {
        StartCoroutine(OpenEffect(GetComponent<RectTransform>(), endCurve));
        Destroy(gameObject); 
    }

    public void QuitGame() { 
        GameManager.Quit();
    }
    public void ChangeSence(int i)
    {
        SceneManager.LoadScene(i);
    }

    public virtual void Refresh() { }
}
