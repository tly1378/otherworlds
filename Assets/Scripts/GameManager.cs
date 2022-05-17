using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Transform Characters;
    public static Transform Houses;
    public static Transform Facilities;
    public static CursorManager CM;
    public static GUIStyle debugStyle;

    public Transform Canvas;
    public GameObject Bridge;
    public Sprite bluePrint;
    private void Awake()
    {
        Application.runInBackground = true;
        Screen.SetResolution(1920, 1080, true);
    }

    private void Start()
    {
        Time.timeScale =1;
        int Index = SceneManager.GetActiveScene().buildIndex;
        if (Index == 1) //判断当前场景是否为游戏场景
        {
            instance = this;
            Characters = GameObject.Find("Characters").transform;
            Houses = GameObject.Find("Houses").transform;
            Facilities = GameObject.Find("Facilities").transform;
        }

        debugStyle = new GUIStyle();
        debugStyle.normal.textColor = Color.white;
        debugStyle.fontSize = 30;
    }

    public void CloseUI(UIForm ui)
    {
        ui.Close();
    }

    public void ChangeSence(int i)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(i);
    }

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

}