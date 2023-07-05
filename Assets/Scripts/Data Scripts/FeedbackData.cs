using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackData
{
    public float Score;
    public float TimeStart;
    public float TimeEnd;
    public float TimeDifference;
    public string NameOfSection;
    public string NameOfStimulus;

    public FeedbackData(float timeStartC, string nameOfSectionC, string nameOfStimulusC)
    {
        TimeStart = timeStartC;
        NameOfSection = nameOfSectionC;
        NameOfStimulus = nameOfStimulusC;
    }

    public void CalcTimeDiff()
    {
        TimeDifference = TimeEnd - TimeStart;
    }

    //            data.WriteLine("Number, Score, Time Given, Time Spent on the Screen, Associated Stimulus Type ");

}
