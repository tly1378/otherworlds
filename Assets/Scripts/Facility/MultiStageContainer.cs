using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiStageContainer : ItemContainer
{
    [System.Serializable]
    public struct Stage
    {
        public SpriteRenderer renderer;
        public Sprite sprite;
        public Item[] items;
    }
    public Stage[] stages;

    public override void Start()
    {
        base.Start();
        InvokeRepeating(nameof(CheckStage), 0, 1);
    }

    public void CheckStage()
    {
        HashSet<SpriteRenderer> rendererSet = new HashSet<SpriteRenderer>();
        foreach(Stage stage in stages)
        {
            if (!rendererSet.Contains(stage.renderer))
            {
                if(stage.items.Length == 0)
                {
                    stage.renderer.sprite = stage.sprite;
                    rendererSet.Add(stage.renderer);
                    break;
                }
                foreach (Item item in stage.items)
                {
                    if (inventory.Contain(item))
                    {
                        stage.renderer.sprite = stage.sprite;
                        rendererSet.Add(stage.renderer);
                        break;
                    }
                }
            }
        }
    }
}
