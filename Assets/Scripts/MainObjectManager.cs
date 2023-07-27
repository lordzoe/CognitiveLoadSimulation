using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text ;
using System.IO ;
using System ;


/// <summary>
/// Class <c>MainObjectManager</c> This class is the core behaviour controller of the software. It recieves inputs from the input manager and sends outputs to visual manager, and instrinsic audio player
/// </summary>
public class MainObjectManager : MonoBehaviour
{

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

    [Tooltip("This variable sets number of trials per section, being A group or B group, normally should be 16")]
    public int NumberOfTrialsPerSection;

    [Tooltip("Number of trials to complete before a PAAS scale is shown. Best if this is a multiple of the above variable (NumberOfTrialPerSection)")]
    public int NumberOfTrialsPerSubSection;

    /// <summary>
    /// holds number of tutorial sections
    /// </summary>
    private int _numberOfTutorialSubSections = 26;





    [Tooltip("Length in seconds of the full length rest")]
    public int RestLengthSeconds; //how long rests should be

    [Tooltip("Length in seconds of the baseline collection")]
    public int BaselineLengthSeconds; //how long the baseline collection should be

    [Tooltip("Length in seconds of the easy and hard practice problems for PAAS scale teaching in the tutorial")]
    public int[] ExQuestionsLengthSeconds; //holds two values for the tutorial example question lengths



    //holds the objects for the questions

    [Tooltip("Containing list of all models used for group A \nLIST MUST BE IN CORRECT ORDER \nDo not modify this list unless you know what you are doing!!")]
    public List<TestingObject> GroupAOrderedListObjects;

    [Tooltip("Containing list of all models used for group B \nLIST MUST BE IN CORRECT ORDER \nDo not modify this list unless you know what you are doing!!")]
    public List<TestingObject> GroupBOrderedListObjects;


    //data structures, do not edit
    public List<AnswerData> Answers = new List<AnswerData>();

    public List<ClickData> Clicks = new List<ClickData>();

    public List<AudioTriggerData> AudioTD = new List<AudioTriggerData>();

    public List<FeedbackData> Feedback = new List<FeedbackData>();

    public TestingObject[] TutorialObjects = new TestingObject[26];

    /// <summary>
    /// is the experiment between two questions 
    /// </summary>
    public bool BetweenTrials = false;

    /// <summary>
    ///are we between tutorial subsections 
    /// </summary>
    private bool _betweenTutorialSubSections = false;

    /// <summary>
    /// are we on an example question in the tutorial
    /// </summary>
    private bool _onExampleTutorialQuestion = false;


    /// <summary>
    /// this is an enum that corresponds to the phase that the overall experiment is in
    /// </summary>
    public enum Phase
    {
        PreStart,//space before experiment begins
        Start, //beginning of experiment
        Calibration, //calibration of the headset
        Rest, //rest phase, done in between many sections
        Tutorial, //Tutorial to teach user about the experiment
        PreExperimental, //Baseline aquisition
        Experimental, //Main experiment
        Feedback, //PAAS scale feedback time
        PostExperiment //After the experiment waiting area
    }
    /// <summary>
    /// enum maps types of stimulus  during experimental phase
    /// </summary>
    public enum Stimulus
    {
        Control,
        Extrisnic,
        Intrinsic
    }

    /// <summary>
    ///holds which portion (slide) of the tutorial is currently being used 
    /// </summary>
    public int TutSubPhaseInd = 0;

    /// <summary>
    ///holds currently active phase 
    /// </summary>
    public Phase ActivePhase;

    /// <summary>
    ///holds currently active stimulus 
    /// </summary>
    public Stimulus ActiveStimulus;

    /// <summary>
    ///holds the previous phase 
    /// </summary>
    private Phase _previousPhase;

    /// <summary>
    ///Is the experiment running? (better catch it!!) NOTE: This will remain true even during feedback periods of the experiment
    /// </summary>
    public bool ExperimentRunning = false;


    /// <summary>
    /// When this bool is primed, the next click in input will switch phase to the feedback phase.
    /// </summary>
    public bool TimeForFeedback = false;


    [SerializeField]
    private VisualManager _visualManager; //holds visual manager

    [SerializeField]
    private IntrinsicAudioPlayer _intrinsicAudioPlayer; //holds instrinsic audioplayer

    /// <summary>
    ///timer for rest 
    /// </summary>
    private float _restTimer = 0;

    /// <summary>
    ///whether the current rest is a long rest or a short rest (1/2 the length of a long rest) 
    /// </summary>
    private bool _onShortRest = false;

    /// <summary>
    ///tutorial example question timer 
    /// </summary>
    private float _exQTimer = 0;

    /// <summary>
    ///timer for baseline collection 
    /// </summary>
    private float _preExpTimer = 0;

    /// <summary>
    /// currently active trial number (goes from 0 until <paramref name="NumberOfTrialsPerSection"/>)
    /// </summary>
    private int _trialCounter = 0; //

    /// <summary>
    /// currently active trial number until next feedback (goes from 0 until <paramref name="NumberOfTrialsPerSubSection"/>)
    /// </summary>
    private int _trialSubCounter = 0;

    /// <summary>
    /// is the current feedback phase initated as a subsection feedback, or the end of a section feedback (for example if it was on group A of molecules will next it move to group B or back to group A)
    /// </summary>
    private bool _onSubFeedback = false;

    /// <summary>
    /// Current length to wait for the example question during the tuturiall (this is the "what is x?" and the "mcdonalds" question)
    /// </summary>
    private int _exQuestionWaitLength;

    /// <summary>
    ///Holds if the expriement is in the first phase or the second phase (regardless of if that is A or B) 
    /// </summary>
    private bool _experimentInFirstSection = true;

    /// <summary>
    /// Timer for the pause between trials
    /// </summary>
    private int _betweenTrialCountdown = 0;

    /// <summary>
    /// Timer for being between tutorial subsections 
    /// </summary>
    private int _betweenTutorialSubsectionCountdown = 0;

    /// <summary>
    /// Holds the active experiment object, which holds both of the molecules within it that subject is being tested on
    /// </summary>
    private TestingObject _activeTestingObject = null;

    /// <summary>
    /// Holds the active tutorial object for user interaction during the tutorial!
    /// </summary>
    private TestingObject _activeTutorialTestingObject = null;

    //These variables all are the names used for the CSV files that corr
    private string _fileNameClicks = "clicks";

    private string _fileNameAnswers = "answers";

    private string _fileNameAudio = "audioTriggers";

    private string _fileNameFeedback = "feedback";

    private string _fileNameCollated = "dataTogether";

    private string _dateInfo; //holds the date to make sure files are uniquely named


    /// <summary>
    /// Method <c>OnApplicationQuit</c> Ensures the program closes before reaching the end of the full experiment, it will still log data
    /// </summary>
    private void OnApplicationQuit()
    {
        if (ActivePhase != Phase.PostExperiment) LogCSVData();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_numberOfTutorialSubSections != TutorialObjects.Length) //this is an error checker for out of bounds errors that will occur later
        {
            Debug.LogError("STOP, THEREWILL BE AN ISSUE UPCOMING");
        }
        ActivePhase = Phase.Start;//set the first state as the start Phase
        if (UseRandomAssignment) //if the type of trial conditions is assigned randomly, do that random assignment, via coin flip like randomness
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

        BeginCSVFiles();//build the CSV files with their titles and headings before data is added to them 
    }

    // Update is called once per frame
    void Update()
    {
        CheckPhase();
        DoCountdowns();
    }


    /// <summary>
    /// Method <c>CheckPhase</c> Looks what the current phase is, and takes actions based on that. These are actions that are required every update.
    /// </summary>
    private void CheckPhase()
    {
        if (ActivePhase != _previousPhase) //if the phase has just changed
        {
            SetNewPhaseText(); //set text visuals
            Debug.Log(ActivePhase); //Can delete this, just for clarity purpose
            if (ActivePhase == Phase.Feedback)
                Feedback.Add(new FeedbackData(Time.time, _previousPhase.ToString(), ActiveStimulus.ToString())); //if starting feedback phase, produce place for the data to be stored
            _previousPhase = ActivePhase; //set what the previous phase to be current phase
        }
        else
        {
            switch (ActivePhase) //run a repeating action based on current phase
            {
                case Phase.Start:

                    break;

                case Phase.Calibration:

                    break;

                case Phase.Tutorial:
                    _visualManager.SetTutorialVisuals(TutSubPhaseInd); //Do tutorial visuals
                    _visualManager.ShowTutorialInstructions(); //and the text visuals
                    switch (TutSubPhaseInd) //which "slide" of the tutorial is being viewed
                    {
                        case 19:
                            //these "slides" need the feedback data to be store
                            if (Feedback.Count == 0)//this if statement makes sure only one data point is added
                            {
                                Feedback.Add(new FeedbackData(Time.time, "Tutorial", "Baseline"));
                            }
                            break;

                        case 21:
                            //these "slides" need to have the timer set for the example questions
                            if (!_onExampleTutorialQuestion)
                            {
                                _onExampleTutorialQuestion = true;
                                _exQuestionWaitLength = ExQuestionsLengthSeconds[0];
                                Debug.Log("IM HERE");
                                _exQTimer = Time.time;
                            }
                            break;

                        case 22:
                            //these "slides" need the feedback data to be store
                            if (Feedback.Count == 1)//this if statement makes sure only one data point is added
                            {
                                Feedback.Add(new FeedbackData(Time.time, "Tutorial", "Easy"));
                            }
                            _onExampleTutorialQuestion = false;
                            break;

                        case 24:
                            //these "slides" need to have the timer set for the example questions
                            if (!_onExampleTutorialQuestion)
                            {
                                _onExampleTutorialQuestion = true;
                                _exQuestionWaitLength = ExQuestionsLengthSeconds[1];
                                _exQTimer = Time.time;
                            }
                            break;

                        case 25:
                            //these "slides" need the feedback data to be store
                            if (Feedback.Count == 2) //this if statement makes sure only one data point is added
                            {
                                Feedback.Add(new FeedbackData(Time.time, "Tutorial", "Hard"));
                            }
                            _onExampleTutorialQuestion = false;
                            break;

                        default:
                            break;
                    }

                    if (_betweenTutorialSubSections) //handles automatic movement between tutorial "slides" due to completion of example rotation problem
                    {
                        if (_betweenTutorialSubsectionCountdown > 0)
                        {
                            _betweenTutorialSubsectionCountdown--;
                        }
                        else
                        {
                            //if timer done
                            _betweenTutorialSubSections = false;
                            NextSubTutPhase(); //move to next phase
                            if (TutSubPhaseInd >= _numberOfTutorialSubSections) //if we have reached the end of the tutorial
                            {
                                ActivePhase = Phase.Rest; //move to rest phase
                                Debug.Log("DO WE EVER GET HERE?");
                            }
                            else
                            {
                                RunTutorialSection(); //otherwise run next tutorial seciton
                            }
                        }
                    }
                    //handles automatic movement between tutorial "slides" due to timers on example problems for PAAS scale

                    if (!(Time.time - _exQTimer < (_exQuestionWaitLength)) && _onExampleTutorialQuestion)
                    {
                        //timer is done
                        _onExampleTutorialQuestion = false;
                        NextSubTutPhase(); //move to next phase
                        RunTutorialSection(); //and run it
                    }


                    break;

                case Phase.PreExperimental:
                    //baseline timer
                    if (!(Time.time - _preExpTimer < (BaselineLengthSeconds)))
                    {
                        ActivePhase = Phase.Rest; //when timer is done move to new phase
                        DoRestingState(true);
                    }
                    break;

                case Phase.Rest:
                    //handles rest timer
                    int modulator = 1; //controls short/long rest length
                    if (_onShortRest)
                        modulator = 2;

                    if (!(Time.time - _restTimer < (RestLengthSeconds / modulator)))
                        ActivePhase = Phase.Feedback; //when timer is done, move to new phase                   
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
    /// Method <c>DoCountDowns</c> Handles between trials counter, which is complicated, and needs its own function as it occurs between feedback/rest and experiment phase (i think!)
    /// </summary>
    private void DoCountdowns()
    {
        if (BetweenTrials)
        {
            if (_betweenTrialCountdown > 0)
            {
                _betweenTrialCountdown--;
            }
            else
            {
                //timer done
                BetweenTrials = false;
                NextTrial(); //run next trial
            }
        }

    }

    /// <summary>
    /// Method <c>DetermineStimulusPhase</c> Sets Stimulus based on current settings, basically checks which portion of experiment we are currently in, and which stimulus that section should have
    /// </summary>
    void DetermineStimulusPhase()
    {
        if (!ControlFirst && _experimentInFirstSection || ControlFirst && !_experimentInFirstSection) // checking if the phase should have a control or stimulus
        {
            // and sets it to the proper stimulus based on initial settings
            //also this is just a cleaner way to write simple if statements
            if (ConditionIsExtrisinsic) ActiveStimulus = Stimulus.Extrisnic;
            else ActiveStimulus = Stimulus.Intrinsic;
        }
        else
        {
            ActiveStimulus = Stimulus.Control;
        }
    }


    /// <summary>
    /// Method <c>SetNewPhaseText</c> Runs switch based on phase and changes the text based on this.
    /// </summary>
    public void SetNewPhaseText() {
        _visualManager.ResetTextToHeadPos();//make sure text is in the write place based on camera
        switch (ActivePhase) //switch based on phase, and run correct text on visual manager (different class)
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
                _visualManager.RunRest(_onShortRest);
                break;

            case Phase.Experimental:
                if (!ExperimentRunning) //if the experiment hasnt started yet
                {
                    _visualManager.RunExperimentIntro();
                }
                break;

            case Phase.Feedback:
                _visualManager.RunFeedback();
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
    /// Method <c>RunPreExperiment</c> Handles the baseline aquisition.
    /// </summary>
    public void RunPreExperiment() {
        _preExpTimer = Time.time;
        if (!ConditionIsExtrisinsic)
        {
            if (!_intrinsicAudioPlayer.AudioOn)
            {
                _intrinsicAudioPlayer.StartIntrinsicAudio(3f, 5, 2); //start intrinsic audio
            }
        }
        else
        {
            _visualManager.ShowExtrinsicStimulus(true); //turn on extrinsic visuals
        }
    }

    /// <summary>
    /// Method <c>DoRestingState</c> Handles the resting state, mainly important to just stop visuals and such
    /// </summary>
    public void DoRestingState(bool ShortRest)
    {
        _intrinsicAudioPlayer.StopIntrinsicAudio();
        _visualManager.ShowExtrinsicStimulus(false);
        _restTimer = Time.time;
        _onShortRest = ShortRest;
    }

    /// <summary>
    /// Method <c>BeginExperiment</c> Starts the experiment
    /// </summary>
    public void BeginExperiment()
    {
        ExperimentRunning = true;
        _visualManager.RunExperimentQuestionText(ActiveStimulus == Stimulus.Intrinsic);
        RunTrial();

    }


    /// <summary>
    /// Method <c>RunTrial</c> Runs next trial
    /// </summary>
    void RunTrial()
    {
        Debug.Log("running trial section");
        if (ActiveStimulus == Stimulus.Intrinsic)
        {
            if (!_intrinsicAudioPlayer.AudioOn)
                _intrinsicAudioPlayer.StartIntrinsicAudio(3f, 5, 2); //runs intrinsic audio if its not already on
        }
        else
        {
            if (_intrinsicAudioPlayer.AudioOn)
                _intrinsicAudioPlayer.StopIntrinsicAudio();
        }


        if (ActiveStimulus == Stimulus.Extrisnic) //makes sure extrinsic visuals are on/off properaly
            _visualManager.ShowExtrinsicStimulus(true);
        else
            _visualManager.ShowExtrinsicStimulus(false);



        if ((_experimentInFirstSection && GroupAFirst) || (!_experimentInFirstSection && !GroupAFirst)) // the two situations where it should be showing group A
        {
            _activeTestingObject = GroupAOrderedListObjects[_trialCounter];
            _activeTestingObject.ProduceModels();
        }
        else //otherwise it must be group B
        {
            _activeTestingObject = GroupBOrderedListObjects[_trialCounter];
            _activeTestingObject.ProduceModels();
        }

        Answers.Add(new AnswerData(Time.time, ActiveStimulus.ToString(), _activeTestingObject.Model1.name)); //add new data file for answers, logging current time and name of testing object

        //advance counters
        _trialCounter++;
        _trialSubCounter++;
    }

    /// <summary>
    /// Method <c>NextSubTutPhase</c> Advances counter and resets the visuals to camera position
    /// </summary>
    public void NextSubTutPhase()
    {
        TutSubPhaseInd++;
        _visualManager.ResetTextToHeadPos();
    }

    /// <summary>
    /// Method <c>RunTutorialSection</c> Runs tutorial section
    /// </summary>
    public void RunTutorialSection()
    {
        Debug.Log("running tut section");
        if (this.transform.childCount == 2) //if there are already objects active, destroy them
        {
            Destroy(this.transform.GetChild(1).gameObject);
            Destroy(this.transform.GetChild(0).gameObject);
        }
        _activeTutorialTestingObject = null; //reset active object

        if (TutorialObjects[TutSubPhaseInd] != null) //if there should be objects in this phase
        {
            _activeTutorialTestingObject = TutorialObjects[TutSubPhaseInd]; //assign them to active object
            _activeTutorialTestingObject.ProduceModels(); //produce the models
        }
    }

    /// <summary>
    /// Method <c>CheckTutorialConditions</c> Checks if the two models that need to be matched have been matched or not, and if they have move to next "slide" of tutorial
    /// </summary>
    public void CheckTutorialConditions()
    {
        if (_activeTutorialTestingObject != null)
        {
            if (_activeTutorialTestingObject.ToBeMatched) //if there is an active object and its models need to be matched
            {
                float angBetween = //dont worry about this complicated math, it works 

                    (Vector3.Dot(_activeTutorialTestingObject.Model1.transform.up, _activeTutorialTestingObject.Model2.transform.up) * _activeTutorialTestingObject.MatchingVectors.x +
                    Vector3.Dot(_activeTutorialTestingObject.Model1.transform.right, _activeTutorialTestingObject.Model2.transform.right) * _activeTutorialTestingObject.MatchingVectors.y +
                    Vector3.Dot(_activeTutorialTestingObject.Model1.transform.forward, _activeTutorialTestingObject.Model2.transform.forward) * _activeTutorialTestingObject.MatchingVectors.z)
                    / (_activeTutorialTestingObject.MatchingVectors.x + _activeTutorialTestingObject.MatchingVectors.y + _activeTutorialTestingObject.MatchingVectors.z);

                if (angBetween > 0.85) //if they are matching 
                {
                    NextSubTutPhase(); //advance the phase
                    if (TutSubPhaseInd >= _numberOfTutorialSubSections)
                    {
                        ActivePhase = Phase.Rest;
                        Debug.Log("DO WE EVER GET HERE?");
                    }
                    else
                    {
                        RunTutorialSection(); //and run it
                    }
                    _betweenTutorialSubSections = true;
                    _betweenTutorialSubsectionCountdown = 30;
                }
            }
        }
    }



    /// <summary>
    /// Method <c>SetBetweenTrials</c> Puts program between trials (clears off the old game objects, and adds a short pause before starting next trial). <paramref name="timeToWait"/> Controls how long to wait before moving to next trial.
    /// This is the one input version which doesnt require there to have been an answer to a question given
    /// </summary>
    public void SetBetweenTrials(int timeToWait)
    {
        if (this.transform.childCount > 0) //remove active objects/models
        {
            Destroy(this.transform.GetChild(1).gameObject);
            Destroy(this.transform.GetChild(0).gameObject);
        }
        _visualManager.ResetTextToHeadPos(); //reset visuals to camera

        //set timer variables
        BetweenTrials = true; 
        _betweenTrialCountdown = timeToWait;
    }

    
    /// <summary>
    /// Method <c>SetBetweenTrials</c> Puts program between trials (clears off the old game objects, and adds a short pause before starting next trial). <paramref name="timeToWait"/> Controls how long to wait before moving to next trial.
    /// This is the two input version which  requires there to have been an answer to a question given, by bool <paramref name="answerWasYes"/> which stores if they answered yes or no
    /// </summary>
    public void SetBetweenTrials(int timeToWait, bool answerWasYes)
    {
        if (this.transform.childCount > 0)//remove active objects/models
        {
            Destroy(this.transform.GetChild(1).gameObject);
            Destroy(this.transform.GetChild(0).gameObject);
        }
        _visualManager.ResetTextToHeadPos(); //reset visuals to camera

        //set timer variables
        BetweenTrials = true;
        _betweenTrialCountdown = timeToWait;

        _visualManager.RunBetweenTrials(AnswerQuestion(answerWasYes)); //run the between trials visuals with with feedback for if the subject got the right answer or not,
                                                                       //by feeding in their answer into AnswerQuestion, which returns a bool corresponding to iff they got question wrong or right
    }

    /// <summary>
    /// Method <c>AnswerQuestion</c> Recieves participant answer <paramref name="didTheyAnswerYes"/> and checks if it is correct, and runs a log attempt
    /// </summary>
    public bool AnswerQuestion(bool didTheyAnswerYes)
    {

        bool isCorrect;

        if (didTheyAnswerYes == _activeTestingObject.Superimposable) //check answer
        {
            isCorrect = true;
        }
        else
        {
            isCorrect = false;
        }

        //log answer data in data structure
        Answers[Answers.Count - 1].TimeAnswerGiven = Time.time;
        Answers[Answers.Count - 1].CalcTimeDiff();
        Answers[Answers.Count - 1].AnswerCorrect = isCorrect;

        return isCorrect; //return if they were correct or not

    }

    /// <summary>
    /// Method <c>NextTrial</c> Moves the experiment to the next trial (via RunTrial() method), if its not time for there to be a feedback session, and if there is, getting that feedback session ready
    /// </summary>
    public void NextTrial() {

        if (_trialCounter == NumberOfTrialsPerSection) //if at the end of a section
        {
            Debug.Log("Time for Feeback " + _trialCounter + "__" + NumberOfTrialsPerSection);
            TimeForFeedback = true; //set that its time for feedback
            ActivePhase = Phase.Feedback; //change the phase
            _intrinsicAudioPlayer.StopIntrinsicAudio(); //stop audio
            _visualManager.ShowExtrinsicStimulus(false); //and visuals

        }
        else if (_trialSubCounter == NumberOfTrialsPerSubSection) //if at the end of subsection (ie enough trials have passed for there to be a mid section PAAS scale check)
        {
            Debug.Log("Time for Feeback micro version " + _trialSubCounter + "__" + NumberOfTrialsPerSubSection);

            TimeForFeedback = true; //set that its time for feedback
            ActivePhase = Phase.Feedback; //change the phase
            _onSubFeedback = true; //note that we are doing sub feedback and not full feedback
            _intrinsicAudioPlayer.StopIntrinsicAudio(); //stop audio 
            _visualManager.ShowExtrinsicStimulus(false); //and visuals
        }
        else //if its not time for feedback
        {
            _visualManager.RunExperimentQuestionText(ActiveStimulus == Stimulus.Intrinsic); //run experiment text, (checking if the text needs to change based on the active stimulus being intrinsic)
            Debug.Log("running next trial " + _trialCounter);
            RunTrial(); //advance to next trial
        }
    }


    
    /// <summary>
    /// Method <c>GetFeedback</c> Get feedback.
    /// <param name="score"> Is the score from feedback </param>
    /// </summary>
    public void GetFeedback(int score)
    {

        //log score in the data 
        Feedback[Feedback.Count - 1].Score = score;
        Feedback[Feedback.Count - 1].TimeEnd = Time.time;
        Feedback[Feedback.Count - 1].CalcTimeDiff();
        Debug.Log(score + " feedback score");

        TimeForFeedback = false; //set that its no longer feedback time

        if (ActivePhase == Phase.Tutorial)
        {
            NextSubTutPhase(); //if we are in the tutorial move forward with tutorial phase
        }
        else //otherwise if we are in the experimental phase
        {
            if (_onSubFeedback) //we have not reached end of a experimental section
            {
                SetBetweenTrials(0); //move to next trial with no delay
                ActivePhase = Phase.Experimental; //change phase
            }
            else //if we have to start the next experimental section
            {
                if (ExperimentRunning) //if the experiment is running
                {
                    _trialCounter = 0; //reset the trial counter
                    if (_experimentInFirstSection) //if we were in the first section previously
                    {
                        //Debug.Log("LOOK HERE " + Feedback[Feedback.Count - 1].NameOfSection);
                        if (Feedback[Feedback.Count - 1].NameOfSection.Equals("Rest")) //if the feedback just came from the end of a rest section
                        {
                            _experimentInFirstSection = false; //then we are not in the first section anymore, and should start second section
                            ActivePhase = Phase.Experimental; //move into experimental section
                            SetBetweenTrials(0); //run next section with no delay
                        }
                        else //if the last feedback didnt come from Rest (ie it must have come from an experiment)
                        {
                            ActivePhase = Phase.Rest; //do a rest phase
                            DoRestingState(true);
                        }                       
                    }
                    else //if we are in the second section
                    {
                        ActivePhase = Phase.PostExperiment; //end experiment
                        LogCSVData(); //and write data to CSV files
                    }
                }
                else //if the experiment hasnt started yet (ie we havent ever entered the experimental phase yet)
                {
                    if (_onShortRest) //if we just finished a short rest 
                    {
                        ActivePhase = Phase.Experimental; //move to experimental section
                        
                    }
                    else //if we on a long rest
                    {
                        ActivePhase = Phase.PreExperimental; //go to baseline phase
                        RunPreExperiment(); 
                    }

                }
            }
            //reset variables
            _trialSubCounter = 0;
            _onSubFeedback = false;
        }

    }

    /// <summary>
    /// Method <c>AddClickData</c> Takes the most recent click and does calculations on it versus the last audiotrigger to assign data
    /// </summary>
    public void AddClickData()
    {
        if (AudioTD.Count != 0) //if there has been an audio cue played
        {
            //figure out the time of the audio and the time of the click
            float nearestTime = AudioTD[AudioTD.Count - 1].Time;
            float clickTime = Clicks[Clicks.Count - 1].Time; 

            //and assign it
            Clicks[Clicks.Count - 1].TimeOfNearestAudio = nearestTime;
            Clicks[Clicks.Count - 1].TimeToNearestAudio = clickTime - nearestTime;

            //assign more info
            if (!AudioTD[AudioTD.Count - 1].CueWasClickedFor) //tells most recent audio cue that it recieved a response
            {
                AudioTD[AudioTD.Count - 1].CueWasClickedFor = true;
            }
            if (Clicks.Count == 1) //if this is the first click ever
            {
                Clicks[Clicks.Count - 1].CorrectClick = true;
            }
            else //otherwise check if the click before this one occured before the last audio cue, and if it did, this cue is the first one for the recent audio cue and was correct, otherwise it was incorrect
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
            if (Clicks[Clicks.Count - 1].TimeToNearestAudio < MaxTimeForResponse) //was the click within the max response time?
            {
                Clicks[Clicks.Count - 1].TooSlow = false;
            }
            else
            {
                Clicks[Clicks.Count - 1].TooSlow = true;
            }
        }
        else //if click was before audio cue has ever been played, set variables to indicate this
        {
            Clicks[Clicks.Count - 1].TimeToNearestAudio = -999;
            Clicks[Clicks.Count - 1].CorrectClick = false;
            Clicks[Clicks.Count - 1].TooSlow = false;
        }

    }


    /// <summary>
    /// Method <c>BeginCSVFile</c> Does the intro aspects of starting a CSV file, such as producing the file and adding the headers
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
    /// </summary>
    void LogCSVData()
    {
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

    /// <summary>
    /// Method <c>GenerateCollatedData</c> takes all the data structures and turns them into one large collated data structure
    /// </summary>
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

    

    
    //private int GetIndexFromTrialAndSection()
    //{
    //    Debug.Log(String.Format("Number of trials per Section = {0}, Current Section = {1}, trial number for this section = {2}", NumberOfTrialsPerSection, _sectionCounter, _trialCounter));

    //    return _trialCounter+(NumberOfTrialsPerSection * (_sectionCounter-1)) - 1;
    //}
}
