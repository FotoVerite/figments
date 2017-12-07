using System.Collections.Generic;

[System.Serializable]
public class ScriptInfo
{
    public List<LevelScriptEvent> events;
    public List<LevelScriptEventCollections> eventCollections;

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
    public string music;
    public List<LevelScriptScreens> screens;
}


[System.Serializable]
public class LevelScriptScreens
{
    public string character;
    public string text;
    public string sfx;

    public string music;
    public float musicVolume;
}

[System.Serializable]
public class LevelScriptEventCollections {
    public List<string> events;
    public string kickoff;
}