using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameConfigurationController : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameObject, initialTimeObject, addedTimeObject, delayTimeObject;
    [SerializeField] private Image colorIndicator;
    public string playerName;
    public int initialTime, addedTime, delayTime;
    public bool canStart;

    public void Initialize(Color playerColor)
    {
        colorIndicator.color = playerColor;
    }

    public void CheckStrings()
    {
        canStart = true;
        playerName = playerNameObject.text;

        // NAME: Not empty.
        if (playerNameObject.text.Length == 0)
            canStart = false;

        // INITIAL TIME: Number, Not negative.
        if (initialTimeObject.text.Length == 0)
        {
            initialTime = 0;
        }
        else if (int.TryParse(initialTimeObject.text, out initialTime))
        {
            if(initialTime >= 0)
                initialTimeObject.textComponent.color = Color.black;
            else
            {
                canStart = false;
                initialTimeObject.textComponent.color = Color.red;
            }
        }
        else
        {
            canStart = false;
            initialTimeObject.textComponent.color = Color.red;
        }

        // ADDED TIME: Number, Not negative.
        if (addedTimeObject.text.Length == 0)
        {
            addedTime = 0;
        }
        else if (int.TryParse(addedTimeObject.text, out addedTime))
        {
            if (addedTime >= 0)
                addedTimeObject.textComponent.color = Color.black;
            else
            {
                canStart = false;
                addedTimeObject.textComponent.color = Color.red;
            }
        }
        else
        {
            canStart = false;
            addedTimeObject.textComponent.color = Color.red;
        }

        // DELAY: Number, Not negative.
        if (delayTimeObject.text.Length == 0)
        {
            delayTime = 0;
        }
        else if (int.TryParse(delayTimeObject.text, out delayTime))
        {
            delayTimeObject.textComponent.color = Color.black;
        }
        else
        {
            canStart = false;
            delayTimeObject.textComponent.color = Color.red;
        }

    }
}
