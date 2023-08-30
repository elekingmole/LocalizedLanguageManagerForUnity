using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using UnityEngine.Events;

public class LocalizedLanguageManager : MonoBehaviour
{
    private Dictionary<string, string> lls = new Dictionary<string, string>();
    private TextReader textReader;
    public bool isReadFinished;
    private bool isWithCoroutine;
    private string defaultLanguage = "eng";
    private string forcedLanguage = "";
    private string fileExtension = ".llf";
    private string manuallyInputPath = "";
    private PathMode pathMode = PathMode.fromResources;

    public LocalizedLanguageManager SetFileExtension(string extension)
    {
        fileExtension = extension;
        return this;
    }

    public LocalizedLanguageManager SetReadMode(bool iswithcoroutine)
    {
        isWithCoroutine = iswithcoroutine;
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
            targetFilename = Application.systemLanguage switch
            {
                SystemLanguage.Afrikaans => "afr",
                SystemLanguage.Arabic => "ara",
                SystemLanguage.Basque => "bas",
                SystemLanguage.Belarusian => "bel",
                SystemLanguage.Bulgarian => "bul",
                SystemLanguage.Catalan => "cat",
                SystemLanguage.Chinese => "chi",
                SystemLanguage.Czech => "cze",
                SystemLanguage.Danish => "dan",
                SystemLanguage.Dutch => "dut",
                SystemLanguage.English => "eng",
                SystemLanguage.Estonian => "est",
                SystemLanguage.Faroese => "far",
                SystemLanguage.Finnish => "fin",
                SystemLanguage.French => "fre",
                SystemLanguage.German => "ger",
                SystemLanguage.Greek => "gre",
                SystemLanguage.Hebrew => "heb",
                SystemLanguage.Hungarian => "hun",
                SystemLanguage.Icelandic => "ice",
                SystemLanguage.Indonesian => "ind",
                SystemLanguage.Italian => "ita",
                SystemLanguage.Japanese => "jpn",
                SystemLanguage.Korean => "kor",
                SystemLanguage.Latvian => "lat",
                SystemLanguage.Lithuanian => "lit",
                SystemLanguage.Norwegian => "nor",
                SystemLanguage.Polish => "pol",
                SystemLanguage.Portuguese => "por",
                SystemLanguage.Romanian => "rom",
                SystemLanguage.Russian => "rus",
                SystemLanguage.SerboCroatian => "ser",
                SystemLanguage.Slovak => "svk",
                SystemLanguage.Slovenian => "svn",
                SystemLanguage.Spanish => "spa",
                SystemLanguage.Swedish => "swe",
                SystemLanguage.Thai => "tha",
                SystemLanguage.Turkish => "tur",
                SystemLanguage.Ukrainian => "ukr",
                SystemLanguage.Vietnamese => "vie",
                SystemLanguage.ChineseSimplified => "chs",
                SystemLanguage.ChineseTraditional => "cht",
                SystemLanguage.Unknown => "unk",
                _ => "eng"
            };
        }
        else
        {
            targetFilename = forcedLanguage;
        }

        targetFilename = (pathMode, targetFilename.Length) switch
        {
            (PathMode.fromResources, _) => targetFilename,
            (_, 0) => defaultLanguage + fileExtension,
            _ => targetFilename += fileExtension
        };

        if (pathMode == PathMode.fromResources)
        {
            LoadTextDataFromResources(targetFilename);
        }
        else
        {
            if (isWithCoroutine)
            {
                StartCoroutine(LoadTextDataWithCoroutine(targetFilename));
            }
            else
            {
                LoadTextData(targetFilename);
            }
        }
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

    public void LoadTextDataFromResources(string targetFilename)
    {
#if UNITY_EDITOR
        var textAsset = Resources.Load<TextAsset>("jpn");
        Debug.Log(textAsset.text);
        string[] data = textAsset.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (string s in data)
        {
            StoreToList(s);
        }

        isReadFinished = true;

#elif UNITY_ANDROID

#endif

    }


    public void LoadTextData(string targetFilename)
    {
        string path = "";
        string textBuffer = "";
#if UNITY_EDITOR
        path = pathMode switch
        {
            PathMode.streamingAsset => Application.streamingAssetsPath + "\\" + targetFilename,
            PathMode.persistentData => Application.persistentDataPath + "\\" + targetFilename,
        };

        FileStream file = new(path, FileMode.Open, FileAccess.Read);
        textReader = new StreamReader(file);
#elif UNITY_ANDROID
        WWW www = null;

        switch(pathMode){
            case PathMode.streamingAsset:
                path = "jar:file://" + Application.dataPath + "!/assets" + "/" + targetFilename; 
        
                www = new WWW(path);
                while(!www.isDone){}

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
        while(!www.isDone){}
		textReader = new StringReader(www.text);
#endif

        string[] tempString = null;
        while ((textBuffer = textReader.ReadLine()) != null)
        {
            StoreToList(textBuffer);
        }

        isReadFinished = true;
    }

    public IEnumerator LoadTextDataWithCoroutine(string targetFilename)
    {
        string path = "";
        string textBuffer = "";
#if UNITY_EDITOR
        path = Application.streamingAssetsPath + "\\" + targetFilename;
        FileStream file = new(path, FileMode.Open, FileAccess.Read);
        textReader = new StreamReader(file);
        yield return new WaitForSeconds(0f);
#elif UNITY_ANDROID
        WWW www = null;

        switch(pathMode){
            case PathMode.streamingAsset:
                path = "jar:file://" + Application.dataPath + "!/assets" + "/" + targetFilename; 
        
                www = new WWW(path);
                while(!www.isDone){
                    yield return www;
                }

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
		while(!www.isDone){
            yield return www;
        }
		textReader = new StringReader(www.text);
#endif

        string[] tempString = null;
        while ((textBuffer = textReader.ReadLine()) != null)
        {
            StoreToList(textBuffer);
        }

        isReadFinished = true;
    }

    private void StoreToList(string targetString)
    {
        string[] tempString;

        if (targetString.Contains("="))
        {
            tempString = targetString.Split("=");
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

    public enum PathMode
    {
        streamingAsset = 0,
        persistentData = 1,
        manuallyInput = 2,
        fromResources = 3,
    }
}
