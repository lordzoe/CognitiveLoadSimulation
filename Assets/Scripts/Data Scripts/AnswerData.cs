using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerData
{
    public float TimeQuestionShown;
    public float TimeAnswerGiven;
    public float TimeDiff;
    public bool AnswerCorrect;
    public string StimulusType;
    public string QuestionName;

    public AnswerData(float timeQuestionShownC, string stimulusTypeC, string questionNameC)
    {
        TimeQuestionShown = timeQuestionShownC;
        StimulusType = stimulusTypeC;
        QuestionName = questionNameC;

    }

    public void CalcTimeDiff()
    {
        TimeDiff = TimeAnswerGiven - TimeQuestionShown;
    }

}
