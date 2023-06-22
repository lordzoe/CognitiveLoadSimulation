using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text ;
using System.IO ;
using System ;

public class MainObjectManager : MonoBehaviour
{
    //when this bool is enabled by the user,
    //the code will randomly assign order of control/experimental conditions,
    //as well as type of experimental conditions
    [Tooltip("When this bool is enabled by the user, the code will randomly assign order of control/experimental conditions, as well as type of experimental conditions.")]
    public bool UseRandomAssignment = false;


    [Tooltip("This controls if the first set of tests is a control or experimental group. To be controlled by researcher, or randomized.")]
    public bool ControlFirst;

    [Tooltip("Controls experimental condition type \n false = Intrinsic \n true = Extrinsic \n To be controlled by researcher, or randomized.")]
    public bool ConditionIsExtrisinsic;

    [Tooltip("Controls Experimental Order of A/B group \n false = B First \n true = A first \n To be controlled by researcher, or randomized.")]

    public bool GroupAFirst;


    [Tooltip("Containing list of all models used for group A \nLIST MUST BE IN CORRECT ORDER \nDo not modify this list unless you know what you are doing!!")]
    public List<TestingObject> GroupAOrderedListObjects;

    [Tooltip("Containing list of all models used for group B \nLIST MUST BE IN CORRECT ORDER \nDo not modify this list unless you know what you are doing!!")]

    public List<TestingObject> GroupBOrderedListObjects;


    public List<AnswerData> Answers = new List<AnswerData>();

    //instrinsic data stuff
    public List<ClickData> Clicks = new List<ClickData>();

    public List<AudioTriggerData> AudioTD = new List<AudioTriggerData>();

    public List<FeedbackData> Feedback = new List<FeedbackData>();

    public TestingObject[] TutorialObjects = new TestingObject[10];




    [Tooltip("This variable sets the max time allowed for a response to an intrinsic audio cue, after which a click will be flagged as too late")]
    public float MaxTimeForResponse = 4f;


    //Number of trials per section
    public int NumberOfTrialsPerSection;

    //Number of trials per sub-section (controls mid group feedbackphase)
    public int NumberOfTrialsPerSubSection;


    public bool BetweenTrials = false;



    public enum Phase
    {
        PreStart,
        Start,
        Calibration,
        Introduction, //should this be renamed to tutorial?
        PreExperimental,
        Experimental,
        Feedback,
        PostExperiment
    }

    public enum Stimulus
    {
        Control,
        Extrisnic,
        Intrinsic
    }

    public enum TutPhase
    {
        Interacting,
        Learning
    }

    public int TutSubPhaseInd = 0;

    public Phase phase;

    public Stimulus ActiveStimulus;

    public TutPhase TutorialPhase;

    private Phase _previousPhase;



    /*[SerializeField]
    //determines if the assignement of interventions is random (true) or determined by researcher (false)
    private bool _randomAssignmentOfInterventions = true; 

    [SerializeField]
    //holds if the intervention will be Exstrinsic (true) or instrinsic (false)
    private bool _isInterventionEx;

    [SerializeField]
    //holds if the intervention will be in the First half (true) or second half (false)
    private bool _isInterventionFirst;*/


    //Is the experiment running? (better catch it!!) NOTE: This will remain true even during feedback periods of the experiment
    public bool ExperimentRunning = false;

    //When this bool is primed, the next click in input will switch phase to the feedback phase.
    public bool TimeForFeedback = false;

    //When this bool is true, requires user to to do input 
    public bool ExInputNeeded = false;

    //stores time since ExInput was asked (ie how long does it take user 
    private int _timeSinceExInput = 0;

    [SerializeField]
    private VisualManager _visualManager;

    [SerializeField]
    private IntrinsicAudioPlayer _intrinsicAudioPlayer;



    private int _trialCounter = 0;

    private int _trialSubCounter = 0;

    private bool _onSubFeedback = false;

    //Holds if the expriement is in the first phase or the second phase (regardless of if that is A or B)
    private bool _experimentInFirstPhase = true;

    private int _betweenTrialCountdown = 0;

    //checks if the stimulus type JUST changed, and allowing for only single runs of some methods
    private bool _FirstChange = true;

    //Holds the active testing object 
    private TestingObject _activeTestingObject = null;

    private TestingObject _activeTutorialTestingObject = null;


    //FILE INFOS
    private string fileNameClicks = "clicks";

    private string fileNameAnswers = "answers";

    private string fileNameAudio = "audioTriggers";

    private string fileNameFeedback = "feedback";

    private string fileNameCollated = "dataTogether";



    private string dateInfo;







    // Start is called before the first frame update
    void Start()
    {
        phase = Phase.Start;
        if (UseRandomAssignment)
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                ConditionIsExtrisinsic = true;
            }
            else
            {
                ConditionIsExtrisinsic = true;

            }
            if (UnityEngine.Random.value > 0.5f)
            {
                ControlFirst = true;
            }
            else
            {
                ControlFirst = false;

            }

            if (UnityEngine.Random.value > 0.5f)
            {
                GroupAFirst = true;
            }
            else
            {
                GroupAFirst = false;

            }
        }


        BeginCSVFiles();
        /*using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + fileName + dateInfo + ".csv"))
        {

           // data.WriteLine("Clicks, , , , , , Audio Cues");
            data.WriteLine("5 , Click, 3.5, 1.5, , , 3.5 ");
            data.Flush();
        }*/ //test showing that you can write below easily 

        //GroupAOrderedListObjects[6].ProduceObjects(); //DELETE


    }

    // Update is called once per frame
    void Update()
    {
        CheckPhase();
        if (ExInputNeeded)
        {
            _timeSinceExInput++; //DELETE
        }
        if (BetweenTrials)
        {
            if (_betweenTrialCountdown > 0)
            {
                _betweenTrialCountdown--;
            }
            else
            {
                BetweenTrials = false;
                NextTrial();
            }
        }

        // GroupAOrderedListObjects[6].testRot(); //DELETE
    }



    /// <summary>
    /// Method <c>DetermineStimulusPhase</c> Sets Stimulus based on current settings. 
    /// </summary>
    void DetermineStimulusPhase()
    {
        if (!ControlFirst && _experimentInFirstPhase || ControlFirst && !_experimentInFirstPhase) // checking if the phase should have a control or stimulus
        {

            // and sets it to the proper stimulus
            if (ConditionIsExtrisinsic)
            {
                ActiveStimulus = Stimulus.Extrisnic;

            }
            else
            {
                ActiveStimulus = Stimulus.Intrinsic;
            }
        }
        else
        {
            ActiveStimulus = Stimulus.Control;
        }
    }
    /// <summary>
    /// Method <c>CheckPhase</c> Looks what the current phase is, and takes actions based on that. These are actions that are required every update.
    /// </summary>
    void CheckPhase()
    {
        if (phase != _previousPhase)
        {
            SetNewPhaseText();
            _previousPhase = phase;
        }
        else
        {
            switch (phase)
            {
                case Phase.Start:

                    break;

                case Phase.Calibration:

                    break;

                case Phase.Introduction:
                    _visualManager.SetTutorialInstructions(TutSubPhaseInd);
                    _visualManager.ShowTutorialInstructions();
                   // CheckTutorialConditions();
                    break;

                case Phase.PreExperimental:

                    break;

                case Phase.Experimental:
                    DetermineStimulusPhase();
                    _visualManager.ShowCurrentStimulus(ActiveStimulus);

                    //if (!ExInputNeeded) {
                    //    if (UnityEngine.Random.value > 0.995) //random chance for a input to be needed
                    //    {
                    //        ExInputNeeded = true;
                    //        _timeSinceExInput = 0;
                    //    }
                    //}
                    //if (ExperimentRunning)
                    //{
                    //    _visualManager.DoStimulus(activeStimulus, ExInputNeeded);
                    //}

                    break;

                case Phase.Feedback:

                    break;

                case Phase.PostExperiment:

                    break;
                default:
                    Debug.LogError("YOU SHOULD NOT BE HERE CHECK CODE");
                    break;
            }
        }

    }

    /// <summary>
    /// Method <c>SetNewPhaseText</c> Runs switch based on phase and changes the text based on this.
    /// </summary>
    public void SetNewPhaseText() {
        switch (phase)
        {
            case Phase.Start:
                _visualManager.RunStart();
                break;

            case Phase.Calibration:
                _visualManager.RunCalibration();
                break;

            case Phase.Introduction:
                break;

            case Phase.PreExperimental:
                _visualManager.RunPreExperiment();
                break;

            case Phase.Experimental:
                if (!ExperimentRunning)
                {
                    _visualManager.RunExperimentIntro();
                }
                break;

            case Phase.Feedback:
                _visualManager.RunFeedback();
                Feedback.Add(new FeedbackData(Time.time, ActiveStimulus.ToString()));
                break;

            case Phase.PostExperiment:
                _visualManager.RunPostExperiment();
                break;
            default:
                Debug.LogError("YOU SHOULD NOT BE HERE CHECK CODE");
                break;
        }
    }

    /// <summary>
    /// Method <c>BeginExperiment</c> Starts the experiment
    /// </summary>
    public void BeginExperiment()
    {
        ExperimentRunning = true;
        _visualManager.RunExperimentQuestionText();
        RunTrial();

    }



    /// <summary>
    /// Method <c>RunTrial</c> Runs the trial as calculated in <paramref name="ConvertTrialAndSectionCounterToIndex"/> from <paramref name="OrderedListOfTestObjects"/>
    /// </summary>
    void RunTrial()
    {

        if (ActiveStimulus == Stimulus.Intrinsic)
        {
            if (!_intrinsicAudioPlayer.AudioOn)
            {
                _intrinsicAudioPlayer.StartIntrinsicAudio(3f, 5, 2);
            }
        }
        if ((_experimentInFirstPhase && GroupAFirst) || (!_experimentInFirstPhase && !GroupAFirst)) // the two situations where it should be showing group A
        {
            _activeTestingObject = GroupAOrderedListObjects[_trialCounter];
            _activeTestingObject.ProduceObjects();
        }
        else //otherwise it must be group B
        {
            _activeTestingObject = GroupBOrderedListObjects[_trialCounter];
            _activeTestingObject.ProduceObjects();
        }

        Answers.Add(new AnswerData(Time.time, ActiveStimulus.ToString(), _activeTestingObject.Model1.name));
        _trialCounter++;
        _trialSubCounter++;
    }


    public void RunTutorialSection()
    {
        if (this.transform.childCount == 2)
        {
            Destroy(this.transform.GetChild(1).gameObject);
            Destroy(this.transform.GetChild(0).gameObject);
        }
        _activeTutorialTestingObject = null;
        if (TutorialObjects[TutSubPhaseInd] != null)
        {
            _activeTutorialTestingObject = TutorialObjects[TutSubPhaseInd];
            _activeTutorialTestingObject.ProduceObjects();
        }
    }

    public void CheckTutorialConditions()
    {
        if (_activeTutorialTestingObject != null)
        {
            float angBetween =
                (Vector3.Dot(_activeTutorialTestingObject.Model1.transform.up, _activeTutorialTestingObject.Model2.transform.up)+
                Vector3.Dot(_activeTutorialTestingObject.Model1.transform.right, _activeTutorialTestingObject.Model2.transform.right) +
                Vector3.Dot(_activeTutorialTestingObject.Model1.transform.forward, _activeTutorialTestingObject.Model2.transform.forward))/3f;
            Debug.Log(angBetween);

            if (_activeTutorialTestingObject.ToBeMatched && angBetween>0.98)
            {
                TutSubPhaseInd++;
                Debug.Log("here we be");
                RunTutorialSection();
            }
        }
    }

    /// <summary>
    /// Method <c>AnswerQuestion</c> Recieves participant answer <paramref name="yesOrNo"/> and checks if it is correct, and runs a log attempt
    /// <param name="yesOrNo"> Does the participant think the molecule is chiral or not, 0 is no, 1 is yes </param>
    /// </summary>
    public void AnswerQuestion(int yesOrNo)
    {
        bool didTheyAnswerYes;
        if (yesOrNo == 1)
        {
            didTheyAnswerYes = true;
        }
        else //NOTE: CAN I GET RID OF THIS INT CONVERSION BULLSHIT?
        {
            didTheyAnswerYes = false;
        }
        bool isCorrect;

        if (didTheyAnswerYes != _activeTestingObject.Superimposable)
        {
            isCorrect = true;
        }
        else
        {
            isCorrect = false;
        }
        Answers[Answers.Count - 1].TimeAnswerGiven = Time.time;
        Answers[Answers.Count - 1].CalcTimeDiff();
        Answers[Answers.Count - 1].AnswerCorrect = isCorrect;

        Debug.Log("did they answer yes? " + didTheyAnswerYes);
        Debug.Log("is it superimpossible/achiral? " + _activeTestingObject.Superimposable);
        Debug.Log("were they correct?? " + isCorrect);//EVETUALLY THIS WILL GO TO A CSV FILE
    }

    /// <summary>
    /// Method <c>SetBetweenTrials</c> Puts program between trials (clears off the old game objects, and adds a short pause before starting next trial)
    /// </summary>
    public void SetBetweenTrials()
    {
        if (this.transform.childCount > 0)
        {
            Destroy(this.transform.GetChild(1).gameObject);
            Destroy(this.transform.GetChild(0).gameObject);
        }
        BetweenTrials = true;
        _betweenTrialCountdown = 30;
        _visualManager.RunBetweenTrials();
    }

    /// <summary>
    /// Method <c>NextTrial</c> Moves the experiment to the next trial
    /// </summary>
    public void NextTrial() {
       
        if (_trialCounter == NumberOfTrialsPerSection)
        {
            Debug.Log("Time for Feeback "+_trialCounter+"__"+NumberOfTrialsPerSection);
            TimeForFeedback = true;
            phase = Phase.Feedback;
            _intrinsicAudioPlayer.StopIntrinsicAudio();

        }
        else if (_trialSubCounter == NumberOfTrialsPerSubSection)
        {
            Debug.Log("Time for Feeback micro version " + _trialSubCounter + "__" + NumberOfTrialsPerSubSection);

            TimeForFeedback = true;
            phase = Phase.Feedback;
            _onSubFeedback = true;
            _intrinsicAudioPlayer.StopIntrinsicAudio();

        }
        else 
        {
            _visualManager.RunExperimentQuestionText();
            Debug.Log("running next trial " + _trialCounter);
            RunTrial();
        }
    }


    //
    /// <summary>
    /// Method <c>GetFeedback</c> NEEDS UPDATE TO ALLOW FOR INBETWEE ADDS Recieves participant answer in form of<paramref name="mouseX"/> and records the rating (TODO)
    /// <param name="mouseX"> Is the mouseX position, used to calculate the answer </param>
    /// </summary>
    public void GetFeedback(float mouseX)
    {
        float score = mouseX / Screen.width * 7.0f;
        Debug.Log(score);
        Feedback[Feedback.Count - 1].Score = score;
        Feedback[Feedback.Count - 1].TimeEnd = Time.time;
        Feedback[Feedback.Count - 1].CalcTimeDiff();

        TimeForFeedback = false;
        if (_onSubFeedback)
        {
            SetBetweenTrials();
            phase = Phase.Experimental;
        }
        else
        {

            _trialCounter = 0;
            if (_experimentInFirstPhase)
            {
                _experimentInFirstPhase = false;
                SetBetweenTrials();
                phase = Phase.Experimental;
                LogCSVData();
            }
            else
            {
                phase = Phase.PostExperiment;
            }
        }
        _trialSubCounter = 0;
        _onSubFeedback = false;
        

    }


    /// <summary>
    /// Method <c>ExInputCheck</c> Run when there is a input for the extrinsic stimulus and print out how long since extrisinc stimulus
    /// </summary>
    public void ExInputCheck()
    {
        if (ExInputNeeded)
        {
            ExInputNeeded = false;
            Debug.Log(_timeSinceExInput);
            _timeSinceExInput = 0;
        }
    }

    /// <summary>
    /// Method <c>BeginCSVFile</c> Does the intro aspects of starting a CSV file, such as producing the file and adding the headers
    /// 
    /// </summary>
    void BeginCSVFiles()
    {
        dateInfo = "___" + System.DateTime.Now.Year.ToString() + "_" + System.DateTime.Now.Month.ToString() + "_" + System.DateTime.Now.Day.ToString() + "_" + System.DateTime.Now.Hour.ToString() + "_" + System.DateTime.Now.Minute.ToString() + "_" + System.DateTime.Now.Second.ToString();

        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + fileNameAnswers + dateInfo + ".csv"))
        {
            data.WriteLine("Answers");
            data.WriteLine("Time Question Shown , Time Question Answered , Time Difference, Was Answer Correct?, Stimulus Type, Question Name ");
            data.Flush();
        }



        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + fileNameClicks + dateInfo + ".csv"))
        {

            data.WriteLine("Clicks");
            data.WriteLine("Time , Nearest AudioCue Time, Time Difference, Was it a Valid Click?, Click Within Max Time? ");
            data.Flush();
        }


        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + fileNameAudio + dateInfo + ".csv"))
        {

            data.WriteLine("Audio");
            data.WriteLine("Time");
            data.Flush();
        }

        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + fileNameFeedback + dateInfo + ".csv"))
        {

            data.WriteLine("Feedback");
            data.WriteLine("Number, Score, Time Answer Given, Time Spent on the Screen, Associated Stimulus Type ");
            data.Flush();
        }


        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + fileNameCollated + dateInfo + ".csv"))
        {

            data.WriteLine("Collated Data");
            data.WriteLine("Event Time , Event Type, Extra Info ");
            data.Flush();
        }



    }


    /// <summary>
    /// Method <c>LogCSVData</c> Fills out the rest of the CSV files
    /// 
    /// </summary>
    void LogCSVData()
    {

        Debug.Log("WRITING CSV");
        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + fileNameAnswers + dateInfo + ".csv"))
        {
            foreach(AnswerData aD in Answers)
            {
                String toWrite = aD.TimeQuestionShown + "," + aD.TimeAnswerGiven + "," + aD.TimeDiff + "," + aD.AnswerCorrect + "," + aD.StimulusType + "," + aD.QuestionName;
                data.WriteLine(toWrite) ;
            }
            data.Flush();
        }



        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + fileNameClicks + dateInfo + ".csv"))
        {

            foreach(ClickData cD in Clicks)
            {
                string toWrite;
               
                if (cD.CorrectClick)
                {
                    toWrite = cD.Time + "," + cD.TimeOfNearestAudio + "," + cD.TimeToNearestAudio+","+"Yes,"+ !cD.TooSlow;
                }
                else
                {
                     toWrite = cD.Time + ",N/A,N/A,NO,N/A";
                }
                data.WriteLine(toWrite);
            }
            data.Flush();
        }


        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + fileNameAudio + dateInfo + ".csv"))
        {

            data.WriteLine("Audio");
            data.WriteLine("Time");
            foreach(AudioTriggerData aTD in AudioTD) {
                data.WriteLine(aTD.Time);
            }
            data.Flush();
        }

        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + fileNameFeedback + dateInfo + ".csv"))
        {

            
            int num = 1;
            foreach (FeedbackData fD in Feedback)
            {
                String toWrite = num + "," + fD.Score + "," + fD.TimeEnd + "," + fD.TimeDifference + "," + fD.NameOfStimulus;
                data.WriteLine(toWrite);
                num++;
            }
            
            data.Flush();
        }


        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + fileNameCollated + dateInfo + ".csv"))
        {

            data.WriteLine("TODO");
            data.Flush();
        }
    }


    /// <summary>
    /// Method <c>AddClickData</c> Takes the most recent click and does calculations on it versus the last audiotrigger to assign data
    /// </summary>
    public void AddClickData()
    {
        if (AudioTD.Count != 0)
        {
            float nearestTime = AudioTD[AudioTD.Count - 1].Time;
            float clickTime = Clicks[Clicks.Count - 1].Time;
            Clicks[Clicks.Count - 1].TimeOfNearestAudio = nearestTime;
            Clicks[Clicks.Count - 1].TimeToNearestAudio = clickTime - nearestTime;
            if (Clicks.Count == 1)
            {
                Clicks[Clicks.Count - 1].CorrectClick = true;
            }
            else
            {
                if (Clicks[Clicks.Count - 2].Time < nearestTime)
                {
                    Clicks[Clicks.Count - 1].CorrectClick = true;
                }
                else
                {
                    Clicks[Clicks.Count - 1].CorrectClick = false;

                }
            }
            if (Clicks[Clicks.Count - 1].TimeToNearestAudio < MaxTimeForResponse)
            {
                Clicks[Clicks.Count - 1].TooSlow = false;
            }
            else {
                Clicks[Clicks.Count - 1].TooSlow = true;
            }
        }
        else
        {
            Clicks[Clicks.Count - 1].TimeToNearestAudio = -999;
            Clicks[Clicks.Count - 1].CorrectClick = false;
            Clicks[Clicks.Count - 1].TooSlow = false;
        }

    }
    //private int GetIndexFromTrialAndSection()
    //{
    //    Debug.Log(String.Format("Number of trials per Section = {0}, Current Section = {1}, trial number for this section = {2}", NumberOfTrialsPerSection, _sectionCounter, _trialCounter));

    //    return _trialCounter+(NumberOfTrialsPerSection * (_sectionCounter-1)) - 1;
    //}
}
