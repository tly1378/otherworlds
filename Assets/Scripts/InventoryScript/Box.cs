using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    public CraftUI bagManager;
    public int level;
    public Text boxMassage;

    void Start()
    {
        bagManager = this.GetComponent<CraftUI>();
    }

    void Update()
    {
        boxMassage.text = level.ToString();
    }

    public void LevelUp() 
    {
        level++;
    }

    public void Destroy() {
        Destroy(gameObject);
    }
}
