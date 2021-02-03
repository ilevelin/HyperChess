using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfoController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timer, playerName, advantage;
    [SerializeField] GameObject backgroundItem, backgroundItem2;

    public void Initialize(Color color, string name, int initTime)
    {
        playerName.text = name;
        playerName.color = color;
        advantage.color = color;
        backgroundItem.GetComponent<Image>().color = color;
        backgroundItem2.GetComponent<Image>().color = color;
        UpdateTimer(initTime);
    }
    
    public void UpdateTimer(int time)
    {
        int h, m, s, ms, tmp;

        h = time / 3600000;
        tmp = time % 3600000;
        m = tmp / 60000;
        tmp = tmp % 60000;
        s = tmp / 1000;
        ms = tmp % 1000;

        string timeString = "";
        if (h > 0)
        {
            timeString = string.Format("{0}:{1:D2}:{2:D2}", h, m, s);
        }
        else if (m > 0)
        {
            timeString = string.Format("{0}:{1:D2}", m, s);
        }
        else
        {
            timeString = string.Format("{0}.{1:D3}", s, ms);
        }

        timer.text = timeString;
    }

    public void UpdateAdvantaje(int adv)
    {
        string number = "";
        if (adv != 0) number = "+";
        advantage.text = number + adv;
    }

    public void EliminatePlayer()
    {
        timer.text = "- OUT -";
        advantage.text = "--";
    }
}
