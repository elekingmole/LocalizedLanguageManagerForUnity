using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using UnityEngine.Events;

public class LocalizedLanguageManager : MonoBehaviour
{
    Dictionary<string, string> lls = new Dictionary<string, string>();
    TextReader textReader;
    public bool isReadFinished;

    string defaultLanguage = "eng";
    string forcedLanguage = "";
    string fileExtension = ".llf";
    string manuallyInputPath = "";
    PathMode pathMode = PathMode.streamingAsset;

    public LocalizedLanguageManager SetFileExtension(string extension)
    {
        fileExtension = extension;
        return this;
    }

    public LocalizedLanguageManager SetPathMode(PathMode mode)
    {
        pathMode = mode;
        return this;
    }

    public LocalizedLanguageManager SetManuallyInputPath(string path)
    {
        manuallyInputPath = path;
        return this;
    }

    public LocalizedLanguageManager SetDefaultLanguage(string filename)
    {
        defaultLanguage = filename;
        return this;
    }

    public LocalizedLanguageManager SetForcedLanguage(string filename)
    {
        forcedLanguage = filename;
        return this;
    }

    public void SetLanguage()
    {
        string targetFilename = "";

        if (forcedLanguage.Length == 0)
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Afrikaans:
                    targetFilename = "afr";
                    break;
                case SystemLanguage.Arabic:
                    targetFilename = "ara";
                    break;
                case SystemLanguage.Basque:
                    targetFilename = "bas";
                    break;
                case SystemLanguage.Belarusian:
                    targetFilename = "bel";
                    break;
                case SystemLanguage.Bulgarian:
                    targetFilename = "bul";
                    break;
                case SystemLanguage.Catalan:
                    targetFilename = "cat";
                    break;
                case SystemLanguage.Chinese:
                    targetFilename = "chi";
                    break;
                case SystemLanguage.Czech:
                    targetFilename = "cze";
                    break;
                case SystemLanguage.Danish:
                    targetFilename = "dan";
                    break;
                case SystemLanguage.Dutch:
                    targetFilename = "dut";
                    break;
                case SystemLanguage.English:
                    targetFilename = "eng";
                    break;
                case SystemLanguage.Estonian:
                    targetFilename = "est";
                    break;
                case SystemLanguage.Faroese:
                    targetFilename = "far";
                    break;
                case SystemLanguage.Finnish:
                    targetFilename = "fin";
                    break;
                case SystemLanguage.French:
                    targetFilename = "fre";
                    break;
                case SystemLanguage.German:
                    targetFilename = "ger";
                    break;
                case SystemLanguage.Greek:
                    targetFilename = "gre";
                    break;
                case SystemLanguage.Hebrew:
                    targetFilename = "heb";
                    break;
                case SystemLanguage.Icelandic:
                    targetFilename = "ice";
                    break;
                case SystemLanguage.Indonesian:
                    targetFilename = "ind";
                    break;
                case SystemLanguage.Italian:
                    targetFilename = "ita";
                    break;
                case SystemLanguage.Japanese:
                    targetFilename = "jpn";
                    break;
                case SystemLanguage.Korean:
                    targetFilename = "kor";
                    break;
                case SystemLanguage.Latvian:
                    targetFilename = "lat";
                    break;
                case SystemLanguage.Lithuanian:
                    targetFilename = "lit";
                    break;
                case SystemLanguage.Norwegian:
                    targetFilename = "nor";
                    break;
                case SystemLanguage.Polish:
                    targetFilename = "pol";
                    break;
                case SystemLanguage.Portuguese:
                    targetFilename = "por";
                    break;
                case SystemLanguage.Romanian:
                    targetFilename = "rom";
                    break;
                case SystemLanguage.Russian:
                    targetFilename = "rus";
                    break;
                case SystemLanguage.SerboCroatian:
                    targetFilename = "ser";
                    break;
                case SystemLanguage.Slovak:
                    targetFilename = "svk";
                    break;
                case SystemLanguage.Slovenian:
                    targetFilename = "svn";
                    break;
                case SystemLanguage.Spanish:
                    targetFilename = "spa";
                    break;
                case SystemLanguage.Swedish:
                    targetFilename = "swe";
                    break;
                case SystemLanguage.Thai:
                    targetFilename = "tha";
                    break;
                case SystemLanguage.Turkish:
                    targetFilename = "tur";
                    break;
                case SystemLanguage.Ukrainian:
                    targetFilename = "ukr";
                    break;
                case SystemLanguage.Vietnamese:
                    targetFilename = "vie";
                    break;
                case SystemLanguage.ChineseSimplified:
                    targetFilename = "chs";
                    break;
                case SystemLanguage.ChineseTraditional:
                    targetFilename = "cht";
                    break;
                case SystemLanguage.Unknown:
                    break;
                case SystemLanguage.Hungarian:
                    targetFilename = "hun";
                    break;
                default:
                    break;
            }

            if (targetFilename.Length == 0)
            {
                targetFilename = defaultLanguage + fileExtension;
            }
            else
            {
                targetFilename += fileExtension;
            }
        }
        else
        {
            targetFilename = forcedLanguage + fileExtension;
        }

        StartCoroutine(LoadTextData(targetFilename));
    }

    public string GetLocalizedString(string key)
    {
        if (isReadFinished)
        {
            if (lls.ContainsKey(key))
            {
                return lls[key];
            }
            else
            {
                string temp = "[key] ";
                foreach (string s in lls.Keys)
                {
                    temp += s + "/";
                }
                temp += "\n[value] ";
                foreach (string s in lls.Values)
                {
                    temp += s + "/";
                }

                return temp;
            }
        }
        else
        {
            return "Not Ready";
        }
    }


    public IEnumerator LoadTextData(string targetFilename)
    {
        string path = "";
        string textBuffer = "";
#if UNITY_EDITOR
        path = Application.streamingAssetsPath + "\\" + targetFilename;
		FileStream file = new FileStream(path,FileMode.Open,FileAccess.Read);
		textReader = new StreamReader(file);
		yield return new WaitForSeconds(0f);
#elif UNITY_ANDROID
        WWW www = null;

        switch(pathMode){
            case PathMode.streamingAsset:
                path = "jar:file://" + Application.dataPath + "!/assets" + "/" + targetFilename; 
        
                www = new WWW(path);
                yield return www;
                if(www.text.Length == 0){
                    path = "jar:file://" + Application.dataPath + "!/assets" + "/" + defaultLanguage + fileExtension;
                }

                break;
            case PathMode.persistentData:
                if(!File.Exists(Application.persistentDataPath + "/" +targetFilename)){
                   targetFilename = defaultLanguage + fileExtension;
                }

                path = "file://"+ Application.persistentDataPath + "/" +targetFilename;
                break;
            case PathMode.manuallyInput:
                if(!File.Exists(manuallyInputPath + "/"+ targetFilename)){
                   targetFilename = defaultLanguage + fileExtension;
                }
                path = "file://"+ manuallyInputPath + "/"+ targetFilename;
                break;
        }

		www = new WWW(path);
		yield return www;
		textReader = new StringReader(www.text);
#endif

        string[] tempString = null;
        while ((textBuffer = textReader.ReadLine()) != null)
        {
            if (textBuffer.Contains("="))
            {
                tempString = textBuffer.Split('=');

                if (tempString.Length > 2)
                {
                    for (int i = 2; i < tempString.Length; i++)
                    {
                        tempString[1] += "=" + tempString[i];
                    }
                }

                lls.Add(tempString[0], tempString[1]);
            }
        }

        isReadFinished = true;
    }

    public enum PathMode
    {
        streamingAsset = 0,
        persistentData = 1,
        manuallyInput = 2,
    }
}
