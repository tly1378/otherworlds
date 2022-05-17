using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationUI : MonoBehaviour
{
    public static InformationUI instance;

    public RectTransform HPBar;
    public RectTransform MaxHPBar;
    public RectTransform HVBar;
    public TMPro.TextMeshProUGUI TP;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        MaxHPBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Player.player.maxHP * 5);
        HPBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Player.player.HP * 5);
        HVBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Player.player.HV * 5);
        TP.text = (int)Player.player.temperature+"℃";
    }
}
