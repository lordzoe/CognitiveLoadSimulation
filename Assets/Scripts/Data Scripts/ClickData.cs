using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickData 
{
    public float Time;
    public float TimeOfNearestAudio;
    public float TimeToNearestAudio;
    public bool CorrectClick;
    public bool TooSlow;
    public ClickData(float timeC)
    {
        Time = timeC;
    }


}
