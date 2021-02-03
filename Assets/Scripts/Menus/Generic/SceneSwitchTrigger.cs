using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitchTrigger : MonoBehaviour
{
    [SerializeField] private GameScene scene = GameScene.MAIN_MENU;
    private SceneSwitcher sceneSwitcher;

    private void Start()
    {
        sceneSwitcher = GameObject.FindGameObjectWithTag("MainLibrary").GetComponent<SceneSwitcher>();
    }

    public void SwitchScene()
    {
        sceneSwitcher.LoadScene(scene);
    }
}
