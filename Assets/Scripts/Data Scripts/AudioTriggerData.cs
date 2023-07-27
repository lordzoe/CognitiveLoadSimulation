using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>AudioTriggerData</c> This class stores audio trigger data
/// </summary>
public class AudioTriggerData
{
    public float Time;
    public string Section;
    public bool CueWasClickedFor = false;
    public AudioTriggerData(float timeC, string sectionC)
    {
        Time = timeC;
        Section=sectionC;
    }
}
