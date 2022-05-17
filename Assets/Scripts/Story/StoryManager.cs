using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    public GameObject storyUI;
    public Image LC;
    public Image RC;
    public Text nowText;
    public Story nowStory;
    public int flag;
    public bool isStart;

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            storyUI.SetActive(true);
            if (flag < nowStory.Sentence.Length)
            {
                nowText.text = nowStory.Sentence[flag];
                if (flag < nowStory.Teller.Length && !nowStory.Teller[flag])
                {
                    LC.gameObject.SetActive(true);
                    RC.gameObject.SetActive(false);
                    LC.sprite = nowStory.LC;
                }
                else
                {
                    LC.gameObject.SetActive(false);
                    RC.gameObject.SetActive(true);
                    RC.sprite = nowStory.RC;
                }
            }
            else
            {
                storyUI.SetActive(false);
                isStart = false;
            }
        }
        else 
        {
            storyUI.SetActive(false);
        }
    }

    public void StartNewStory(Story story)
    {
        nowStory = story;
        flag = 0;
        isStart = true;
        
    }
    public void NextSentence() 
    {
        flag++;
    }
}
