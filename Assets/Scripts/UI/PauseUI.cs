using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : UIForm
{
    public override void Start()
    {
        //base.Start();
        Time.timeScale = 0;
    }

    public override void Close()
    {
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }
}
