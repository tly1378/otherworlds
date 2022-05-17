using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public enum RequirementType
    {
        Item,
        Prop,
        Location,
        Sell,
        Buy,
        Kill
    }

    [System.Serializable]
    public class Requirement
    {
        public RequirementType type;
        public string description;
        public string parameter;
    }

    [System.Serializable]
    public class Quest
    {
        public string name;
        public string description;
        public List<Requirement> requirements;

        public override string ToString()
        {
            string txt = $"{name} \n{description}\n";
            int i = 0;
            foreach(Requirement requirement in requirements)
            {
                i++;
                txt += $"{i}. {requirement.description}\n";
            }
            return txt;
        }
    }

    public List<Quest> quests;

    

    public void Update()
    {
        if (meetRequirement())
        {
            quests.Remove(quests[0]);
        }
    }

    private bool meetRequirement()
    {
        return false;
    }

    private void OnGUI()
    {
        if (Time.timeScale == 0) return;
        GUILayout.BeginArea(new Rect(0, Screen.height - 800, 500, 500));
        GUILayout.Label(quests[0].ToString(), GameManager.debugStyle);
        GUILayout.EndArea();
    }
}
