using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTriggerData
{
    public float Time;
    public string Section;
    public AudioTriggerData(float timeC, string sectionC)
    {
        Time = timeC;
        Section=sectionC;
    }
}
