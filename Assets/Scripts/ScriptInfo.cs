using System.Collections.Generic;

[System.Serializable]
public class ScriptInfo
{
    public List<LevelScriptEvent> events;
   
}

[System.Serializable]
public class LevelScriptEvent {
    public string eventType;
    public string kickoff;
    public float timeCode;
    public bool skip;
    public bool remove;
    public bool startWave;
    public string nextEvent;
    public string skipToEvent;
    public List<LevelScriptScreens> screens;
}


[System.Serializable]
public class LevelScriptScreens
{
    public string character;
    public string text;
    public string sfx;
}