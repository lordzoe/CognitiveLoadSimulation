using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackData
{
    public float Score;
    public float TimeStart;
    public float TimeEnd;
    public float TimeDifference;
    public string NameOfStimulus;

    public FeedbackData(float timeStartC, string nameOfStimulusC)
    {
        TimeStart = timeStartC;
        NameOfStimulus = nameOfStimulusC;
    }

    public void CalcTimeDiff()
    {
        TimeDifference = TimeEnd - TimeStart;
    }

    //            data.WriteLine("Number, Score, Time Given, Time Spent on the Screen, Associated Stimulus Type ");

}
