using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using System.Text; 

public class LocalizationTest : MonoBehaviour
{
    public static string resultTxt = "";
    private GUIStyle rectStyle = new GUIStyle();
    public LocalizedLanguageManager LLM = null;

    void Start()
    {
        rectStyle.wordWrap = true;
        rectStyle.fontSize = 50;
        rectStyle.alignment = TextAnchor.MiddleLeft;
        LLM = GetComponentInParent<LocalizedLanguageManager>();
        LLM.SetPathMode(LocalizedLanguageManager.PathMode.streamingAsset).SetReadMode(true)./* SetForcedLanguage("unk").*/SetLanguage();
    }

    void Update()
    {
        if (LLM.isReadFinished && resultTxt.Length == 0)
        {
            resultTxt = "[title] " + LLM.GetLocalizedString("title") + "\n" + 
                        "[start] " + LLM.GetLocalizedString("start")+ "\n" +
                        "[splitTest] " + LLM.GetLocalizedString("splitTest");
        }
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(10, Screen.height / 2, Screen.width, 350), resultTxt, rectStyle);
    }
}
