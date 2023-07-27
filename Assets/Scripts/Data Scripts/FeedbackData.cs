using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>FeedbackData</c> This class stores feedback data
/// </summary>
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
