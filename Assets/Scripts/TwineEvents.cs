using System.Collections.Generic;

[System.Serializable]
public class TwineEvents
{
    public List<TwineEvent> events;
   
}
[System.Serializable]
public class TwineEvent {
    public string kickoff;
    public List<TwineChoices> choices;
}


[System.Serializable]
public class TwineChoices
{
    public string buttonText;
    public string eventName;
}