
using UnityEngine;
using System.Collections.Generic;


/* JSONutility cannot serialize Dictionary<string, int> or Dictionary<string, bool> */
[System.Serializable]
public class DictionaryStringInt
{
    public string key;
    public int value;
}

[System.Serializable]
public class DictionaryStringFloat
{
    public string key;
    public float value;
}

/* JSONutility cannot serialize Dictionary<string, int> or Dictionary<string, bool> */
[System.Serializable]
public class DictionaryStringBool
{
    public string key;
    public bool value;
}

[System.Serializable]
public class SaveData
{
    public List<DictionaryStringInt> levelRecords = new List<DictionaryStringInt>();
    public List<DictionaryStringBool> levelUnlocked = new List<DictionaryStringBool>();

    public List<DictionaryStringBool> transportUnlocked = new List<DictionaryStringBool>();
    public List<DictionaryStringBool> transportSelected = new List<DictionaryStringBool>();
    public List<DictionaryStringInt> transportLevels = new List<DictionaryStringInt>();

    public List<DictionaryStringFloat> gameOptionsVolume = new List<DictionaryStringFloat>();
    public int playerBalance;
}