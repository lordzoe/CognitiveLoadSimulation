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

    [Tooltip("This variable sets the max time allowed for a response to an intrinsic audio cue, after which a click will be flagged as too late")]
    public float MaxTimeForResponse = 4f;


    //Number of trials per section
    public int NumberOfTrialsPerSection;

    //Number of trials per sub-section (controls mid group feedbackphase)
    public int NumberOfTrialsPerSubSection;

    private int _numberOfTutorialSubSections = 26; //holds number of tutorial sections

    public int RestLengthSeconds; //how long rests should be

    public int BaselineLengthSeconds; //how long the baseline collection should be

    public int[] ExQuestionsLengthSeconds; //holds two values for the tutorial example question lengths

    private int _exQuestionWaitLength; //holds the correct question waiting time based on which portion of tutorial we in

    //holds the objects for the questions

    [Tooltip("Containing list of all models used for group A \nLIST MUST BE IN CORRECT ORDER \nDo not modify this list unless you know what you are doing!!")]
    public List<TestingObject> GroupAOrderedListObjects;

    [Tooltip("Containing list of all models used for group B \nLIST MUST BE IN CORRECT ORDER \nDo not modify this list unless you know what you are doing!!")]

    public List<TestingObject> GroupBOrderedListObjects;


    //data structures 
    public List<AnswerData> Answers = new List<AnswerData>();

    public List<ClickData> Clicks = new List<ClickData>();

    public List<AudioTriggerData> AudioTD = new List<AudioTriggerData>();

    public List<FeedbackData> Feedback = new List<FeedbackData>();

    public TestingObject[] TutorialObjects = new TestingObject[26];


    //is the experiment between two questions 
    public bool BetweenTrials = false;

    //are we between tutorial subsections 
    private bool _betweenTutorialSubSections = false;

    //are we on an example question in the tutorial
    private bool _onExampleTutorialQuestion = false; 


    public enum Phase
    {
        PreStart,
        Start,
        Calibration,
        Rest,
        Tutorial, //should this be renamed to tutorial?
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
        Learning,
        Feedback
    }

    public int TutSubPhaseInd = 0;

    public Phase phase;

    public Stimulus ActiveStimulus;

    private Phase _previousPhase;

    //Is the experiment running? (better catch it!!) NOTE: This will remain true even during feedback periods of the experiment
    public bool ExperimentRunning = false;

    //When this bool is primed, the next click in input will switch phase to the feedback phase.
    public bool TimeForFeedback = false;

    [SerializeField]
    private VisualManager _visualManager;

    [SerializeField]
    private IntrinsicAudioPlayer _intrinsicAudioPlayer;

    private float _restTimer = 0;

    public bool OnShortRest = false;

    private float _exQTimer = 0;

    private float _preExpTimer = 0;

    private int _trialCounter = 0;

    private int _trialSubCounter = 0;

    private bool _onSubFeedback = false;

    //Holds if the expriement is in the first phase or the second phase (regardless of if that is A or B)
    private bool _experimentInFirstPhase = true;

    private int _betweenTrialCountdown = 0;

    private int _betweenTutorialSubsectionCountdown = 0;

    //Holds the active testing object 
    private TestingObject _activeTestingObject = null;

    private TestingObject _activeTutorialTestingObject = null;

    //FILE INFOS
    private string _fileNameClicks = "clicks";

    private string _fileNameAnswers = "answers";

    private string _fileNameAudio = "audioTriggers";

    private string _fileNameFeedback = "feedback";

    private string _fileNameCollated = "dataTogether";

    private string _dateInfo; //holds the date to make sure files are uniquely named

    //if the program closes before ending, log data
    private void OnApplicationQuit()
    {
        if (phase != Phase.PostExperiment) LogCSVData(); 
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_numberOfTutorialSubSections != TutorialObjects.Length)
        {
            Debug.LogError("STOP, THEREWILL BE AN ISSUE UPCOMING");
        }
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
    }

    // Update is called once per frame
    void Update()
    {
        CheckPhase();
        
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
        if (_betweenTutorialSubSections)
        {
            if (_betweenTutorialSubsectionCountdown > 0)
            {
                _betweenTutorialSubsectionCountdown--;
            }
            else
            {
                _betweenTutorialSubSections = false;
                NextSubTutPhase();
                if (TutSubPhaseInd >= _numberOfTutorialSubSections)
                {
                    phase = Phase.Rest;
                }
                else
                {
                    RunTutorialSection();
                }
            }
        }
        if (_onExampleTutorialQuestion)
        {
            if (Time.time - _exQTimer < (_exQuestionWaitLength))
            {
                
            }
            else
            {
                _onExampleTutorialQuestion = false;
                NextSubTutPhase();
                RunTutorialSection();
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
            Debug.Log(phase);
        }
        else
        {
            switch (phase)
            {
                case Phase.Start:

                    break;

                case Phase.Calibration:

                    break;

                case Phase.Tutorial:
                    _visualManager.SetTutorialVisuals(TutSubPhaseInd);
                    _visualManager.ShowTutorialInstructions();
                    switch (TutSubPhaseInd)
                    {
                        case 19:
                            if (Feedback.Count == 0)
                            {
                                Feedback.Add(new FeedbackData(Time.time, "Tutorial", "Baseline"));
                            }
                            break;

                        case 21:
                            if (!_onExampleTutorialQuestion)
                            {
                                _onExampleTutorialQuestion = true;
                                _exQuestionWaitLength = ExQuestionsLengthSeconds[0];
                                Debug.Log("IM HERE");
                                _exQTimer = Time.time;
                            }
                            break;

                        case 22:
                            if (Feedback.Count == 1)
                            {
                                Feedback.Add(new FeedbackData(Time.time, "Tutorial", "Easy"));
                            }
                            _onExampleTutorialQuestion = false;
                            break;

                        case 24:
                            if (!_onExampleTutorialQuestion)
                            {
                                _onExampleTutorialQuestion = true;
                                _exQuestionWaitLength = ExQuestionsLengthSeconds[1];
                                _exQTimer = Time.time;
                            }
                            break;

                        case 25:
                            if (Feedback.Count == 2)
                            {
                                Feedback.Add(new FeedbackData(Time.time, "Tutorial", "Hard"));
                            }
                            _onExampleTutorialQuestion = false;
                            break;

                        default:
                            break;
                    }
                    // CheckTutorialConditions();
                    break;

                case Phase.PreExperimental:
                    if (Time.time - _preExpTimer < (BaselineLengthSeconds))
                    {
                        //Debug.Log(Time.time - _preExpTimer);
                    }
                    else
                    {
                        phase = Phase.Rest;
                        DoRestingState(true);
                    }
                    break;

                case Phase.Rest:
                    int modulator = 1;
                    if (OnShortRest)
                    {
                        modulator = 2;
                    }
                    if (Time.time - _restTimer < (RestLengthSeconds / modulator))
                    {

                    }
                    else
                    {
                        phase = Phase.Feedback;
                    }
                    break;

                case Phase.Experimental:
                    DetermineStimulusPhase();

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
        _visualManager.ResetTextToHeadPos();

        switch (phase)
        {
            case Phase.Start:
                _visualManager.RunStart();
                break;

            case Phase.Calibration:
                _visualManager.RunCalibration();
                break;

            case Phase.Tutorial:
                
                break;

            case Phase.PreExperimental:
                _visualManager.RunPreExperiment(!ConditionIsExtrisinsic);
                break;

            case Phase.Rest:
                _visualManager.RunRest(OnShortRest);
                break;

            case Phase.Experimental:
                if (!ExperimentRunning)
                {
                    _visualManager.RunExperimentIntro();
                }
                break;

            case Phase.Feedback:
                _visualManager.RunFeedback();
                Feedback.Add(new FeedbackData(Time.time, _previousPhase.ToString(), ActiveStimulus.ToString()));//hmm
                break;

            case Phase.PostExperiment:
                _visualManager.RunPostExperiment();
                break;
            default:
                Debug.LogError("YOU SHOULD NOT BE HERE CHECK CODE");
                break;
        }
    }


    public void RunPreExperiment() {
        Debug.Log("HEREEREE");
        _preExpTimer = Time.time;
        if (!ConditionIsExtrisinsic)
        {
            if (!_intrinsicAudioPlayer.AudioOn)
            {
                _intrinsicAudioPlayer.StartIntrinsicAudio(3f, 5, 2);
            }
        }
        else
        {
            _visualManager.ShowExtrinsicStimulus(true);
        }
    }

    public void DoRestingState(bool ShortRest)
    {
        _intrinsicAudioPlayer.StopIntrinsicAudio();
        _visualManager.ShowExtrinsicStimulus(false);
        _restTimer = Time.time;
        OnShortRest = ShortRest;
    }

    /// <summary>
    /// Method <c>BeginExperiment</c> Starts the experiment
    /// </summary>
    public void BeginExperiment()
    {
        ExperimentRunning = true;
        _visualManager.RunExperimentQuestionText(ActiveStimulus==Stimulus.Intrinsic);
        RunTrial();

    }



    /// <summary>
    /// Method <c>RunTrial</c> Runs the trial as calculated in <paramref name="ConvertTrialAndSectionCounterToIndex"/> from <paramref name="OrderedListOfTestObjects"/>
    /// </summary>
    void RunTrial()
    {
        Debug.Log("running trial section");
        if (ActiveStimulus == Stimulus.Intrinsic)
        {
            if (!_intrinsicAudioPlayer.AudioOn)
            {
                _intrinsicAudioPlayer.StartIntrinsicAudio(3f, 5, 2);
            }
        }
        if (ActiveStimulus == Stimulus.Extrisnic)
        {
            _visualManager.ShowExtrinsicStimulus(true);
        }
        else
        {
            _visualManager.ShowExtrinsicStimulus(false);
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
        Debug.Log("running tut section");
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
            if (_activeTutorialTestingObject.ToBeMatched)
            {
                float angBetween =

                    (Vector3.Dot(_activeTutorialTestingObject.Model1.transform.up, _activeTutorialTestingObject.Model2.transform.up) * _activeTutorialTestingObject.MatchingVectors.x +
                    Vector3.Dot(_activeTutorialTestingObject.Model1.transform.right, _activeTutorialTestingObject.Model2.transform.right) * _activeTutorialTestingObject.MatchingVectors.y +
                    Vector3.Dot(_activeTutorialTestingObject.Model1.transform.forward, _activeTutorialTestingObject.Model2.transform.forward) * _activeTutorialTestingObject.MatchingVectors.z)
                    / (_activeTutorialTestingObject.MatchingVectors.x + _activeTutorialTestingObject.MatchingVectors.y + _activeTutorialTestingObject.MatchingVectors.z);

                /* Debug.DrawRay(_activeTutorialTestingObject.Model2.transform.position, _activeTutorialTestingObject.Model1.transform.up, Color.red);
                 Debug.DrawRay(_activeTutorialTestingObject.Model2.transform.position, _activeTutorialTestingObject.Model2.transform.up, Color.blue);

                 */

                if (angBetween > 0.85)
                {
                    NextSubTutPhase();
                    Debug.Log("here we be");
                    if (TutSubPhaseInd >= _numberOfTutorialSubSections)
                    {
                        phase = Phase.Rest;
                    }
                    else
                    {
                        RunTutorialSection();
                    }
                    _betweenTutorialSubSections = true;
                    _betweenTutorialSubsectionCountdown = 30;
                }
            }
        }
    }

    /// <summary>
    /// Method <c>AnswerQuestion</c> Recieves participant answer <paramref name="yesOrNo"/> and checks if it is correct, and runs a log attempt
    /// <param name="yesOrNo"> Does the participant think the molecule is chiral or not, 0 is no, 1 is yes </param>
    /// </summary>
    public bool AnswerQuestion(bool didTheyAnswerYes)
    {
        
        bool isCorrect;

        if (didTheyAnswerYes == _activeTestingObject.Superimposable)
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
        return isCorrect;

    }

    /// <summary>
    /// Method <c>SetBetweenTrials</c> Puts program between trials (clears off the old game objects, and adds a short pause before starting next trial)
    /// </summary>
    public void SetBetweenTrials(int timeToWait)
    {
        if (this.transform.childCount > 0)
        {
            Destroy(this.transform.GetChild(1).gameObject);
            Destroy(this.transform.GetChild(0).gameObject);
        }
        _visualManager.ResetTextToHeadPos();
        BetweenTrials = true;
        _betweenTrialCountdown = timeToWait;
    }

    /// <summary>
    /// Method <c>SetBetweenTrials</c> Puts program between trials (clears off the old game objects, and adds a short pause before starting next trial)
    /// </summary>
    public void SetBetweenTrials(int timeToWait, bool answer)
    {
        if (this.transform.childCount > 0)
        {
            Destroy(this.transform.GetChild(1).gameObject);
            Destroy(this.transform.GetChild(0).gameObject);
        }
        BetweenTrials = true;
        _visualManager.ResetTextToHeadPos();
        _betweenTrialCountdown = timeToWait;
        _visualManager.RunBetweenTrials(AnswerQuestion(answer));
    }

    /// <summary>
    /// Method <c>NextTrial</c> Moves the experiment to the next trial
    /// </summary>
    public void NextTrial() {

        if (_trialCounter == NumberOfTrialsPerSection)
        {
            Debug.Log("Time for Feeback " + _trialCounter + "__" + NumberOfTrialsPerSection);
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
            _visualManager.ShowExtrinsicStimulus(false);

        }
        else
        {
            _visualManager.RunExperimentQuestionText(ActiveStimulus == Stimulus.Intrinsic);
            Debug.Log("running next trial " + _trialCounter);
            RunTrial();
        }
    }


    //
    /// <summary>
    /// Method <c>GetFeedback</c> Get feedback.
    /// <param name="score"> Is the score from feedback </param>
    /// </summary>
    public void GetFeedback(int score)
    {

        Debug.Log(score + " feedback score");
        Feedback[Feedback.Count - 1].Score = score;
        Feedback[Feedback.Count - 1].TimeEnd = Time.time;
        Feedback[Feedback.Count - 1].CalcTimeDiff();

        TimeForFeedback = false;

        if (phase == Phase.Tutorial)
        {
            NextSubTutPhase();
        }
        else
        {
            if (_onSubFeedback)
            {
                SetBetweenTrials(0);
                phase = Phase.Experimental;
            }
            else
            {
                if (ExperimentRunning)
                {
                    _trialCounter = 0;
                    if (_experimentInFirstPhase)
                    {
                        Debug.Log("LOOK HERE " + Feedback[Feedback.Count - 1].NameOfSection);
                        if (Feedback[Feedback.Count - 1].NameOfSection.Equals("Rest"))
                        {
                            _experimentInFirstPhase = false;
                            phase = Phase.Experimental;
                            SetBetweenTrials(0);
                        }
                        else
                        {
                            phase = Phase.Rest;
                            DoRestingState(true);
                        }
                        //LogCSVData();
                    }
                    else
                    {
                        phase = Phase.PostExperiment;
                        LogCSVData();
                    }
                }
                else
                {
                    if (OnShortRest)
                    {
                        phase = Phase.Experimental;
                        if (ExperimentRunning)
                        {
                            SetBetweenTrials(0);
                        }
                    }
                    else
                    {
                        phase = Phase.PreExperimental;
                        RunPreExperiment();
                    }

                }
            }
            _trialSubCounter = 0;
            _onSubFeedback = false;
        }

    }




   

    /// <summary>
    /// Method <c>BeginCSVFile</c> Does the intro aspects of starting a CSV file, such as producing the file and adding the headers
    /// 
    /// </summary>
    void BeginCSVFiles()
    {
        _dateInfo = "___" + System.DateTime.Now.Year.ToString() + "_" + System.DateTime.Now.Month.ToString() + "_" + System.DateTime.Now.Day.ToString() + "_" + System.DateTime.Now.Hour.ToString() + "_" + System.DateTime.Now.Minute.ToString() + "_" + System.DateTime.Now.Second.ToString();

        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + _fileNameAnswers + _dateInfo + ".csv"))
        {
            data.WriteLine("Answers");
            data.WriteLine("Time Question Shown , Time Question Answered , Time Difference, Was Answer Correct?, Stimulus Type, Question Name ");
            data.Flush();
        }



        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + _fileNameClicks + _dateInfo + ".csv"))
        {

            data.WriteLine("Clicks");
            data.WriteLine("Time , Nearest AudioCue Time, Time Difference, Was it a Valid Click?, Click Within Max Time?, Section");
            data.Flush();
        }


        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + _fileNameAudio + _dateInfo + ".csv"))
        {

            data.WriteLine("Audio");
            data.WriteLine("Time, Section, Was audio cue responded to?");
            data.Flush();
        }

        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + _fileNameFeedback + _dateInfo + ".csv"))
        {

            data.WriteLine("Feedback");
            data.WriteLine("Number, Score, Time Answer Given, Time Spent on the Screen, Associated Section, Stimulus ");
            data.Flush();
        }


        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + _fileNameCollated + _dateInfo + ".csv"))
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
        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + _fileNameAnswers + _dateInfo + ".csv"))
        {
            foreach (AnswerData aD in Answers)
            {
                String toWrite = aD.TimeQuestionShown + "," + aD.TimeAnswerGiven + "," + aD.TimeDiff + "," + aD.AnswerCorrect + "," + aD.StimulusType + "," + aD.QuestionName;
                data.WriteLine(toWrite);
            }
            data.Flush();
        }



        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + _fileNameClicks + _dateInfo + ".csv"))
        {

            foreach (ClickData cD in Clicks)
            {
                string toWrite;

                if (cD.CorrectClick)
                {
                    toWrite = cD.Time + "," + cD.TimeOfNearestAudio + "," + cD.TimeToNearestAudio + "," + "Yes," + !cD.TooSlow + "," + cD.Section;
                }
                else
                {
                    toWrite = cD.Time + ",N/A,N/A,NO,N/A,"+cD.Section;
                }
                data.WriteLine(toWrite);
            }
            data.Flush();
        }


        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + _fileNameAudio + _dateInfo + ".csv"))
        {
            foreach (AudioTriggerData aTD in AudioTD) {
                data.WriteLine(aTD.Time + "," + aTD.Section+","+(aTD.CueWasClickedFor?"Was responded to":"Was not responded to"));
            }
            data.Flush();
        }

        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + _fileNameFeedback + _dateInfo + ".csv"))
        {


            int num = 1;
            foreach (FeedbackData fD in Feedback)
            {
                String toWrite = num + "," + fD.Score + "," + fD.TimeEnd + "," + fD.TimeDifference + "," + fD.NameOfSection + "," + fD.NameOfStimulus;
                data.WriteLine(toWrite);
                num++;
            }

            data.Flush();
        }


        List<CollatedData> collatedData = GenerateCollatedData();

        using (StreamWriter data = File.AppendText(Application.dataPath + "/CSVOutput/" + _fileNameCollated + _dateInfo + ".csv"))
        {

            foreach (CollatedData colD in collatedData)
            {
                data.WriteLine(colD.Time+","+colD.Type+","+ colD.Info);
            }
            data.Flush();
        }
    }


    public List<CollatedData> GenerateCollatedData()
    {
        List<CollatedData> retList = new List<CollatedData>();
        foreach (AnswerData aD in Answers)
        {
            retList.Add(new CollatedData(aD.TimeAnswerGiven, "Answer", (aD.AnswerCorrect ? "Correct" : "Incorrect") + " on Q: " + aD.QuestionName+ " with stimulus: "+aD.StimulusType));
        }

        foreach (ClickData cD in Clicks)
        {
            retList.Add(new CollatedData(cD.Time, "Click", (cD.CorrectClick ? "Valid" : "Invalid") + " & " + (cD.TooSlow ? "Too Slow" : "OnTime")+ " in section "+cD.Section));
        }
        foreach (AudioTriggerData aTD in AudioTD)
        {
            retList.Add(new CollatedData(aTD.Time, "Audio Trigger", "In section " + aTD.Section + (aTD.CueWasClickedFor? " was responded to":" was missed")));
        }
        foreach (FeedbackData fD in Feedback)
        {
            retList.Add(new CollatedData(fD.TimeEnd, "Feedback", "Of score: " + fD.Score));
        }

        retList.Sort((x, y) => x.Time.CompareTo(y.Time));
        
        return retList;
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
            if (!AudioTD[AudioTD.Count - 1].CueWasClickedFor)
            {
                AudioTD[AudioTD.Count - 1].CueWasClickedFor = true;
            }
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

    public void NextSubTutPhase()
    {
        TutSubPhaseInd++;
        _visualManager.ResetTextToHeadPos();
    }
    //private int GetIndexFromTrialAndSection()
    //{
    //    Debug.Log(String.Format("Number of trials per Section = {0}, Current Section = {1}, trial number for this section = {2}", NumberOfTrialsPerSection, _sectionCounter, _trialCounter));

    //    return _trialCounter+(NumberOfTrialsPerSection * (_sectionCounter-1)) - 1;
    //}
}
