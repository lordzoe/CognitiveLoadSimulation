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



    //Number of trials per section
    public int NumberOfTrialsPerSection = 4;

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

    public Phase phase;

    public Stimulus activeStimulus;

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

    //Which of the questions (trial) the experiment is on (goes from 1 to NumberOfSections*NumberOfTrials, inclusive).
    private int _trialCounter = 1;
   
    //Holds if the expriement is in the first phase or the second phase (regardless of if that is A or B)
    private bool _experimentInFirstPhase = true;

    private int _betweenTrialCountdown = 0;

    //Holds the active testing object 
    private TestingObject _activeTestingObject = null;

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
        GroupAOrderedListObjects[6].ProduceObjects();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPhase();
        if (ExInputNeeded)
        {
            _timeSinceExInput++;
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

        GroupAOrderedListObjects[6].testRot();
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
                activeStimulus = Stimulus.Extrisnic;
            }
            else
            {
                activeStimulus = Stimulus.Intrinsic;
            }
        }
        else
        {
            activeStimulus = Stimulus.Control;
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

                    break;

                case Phase.PreExperimental:

                    break;

                case Phase.Experimental:
                    DetermineStimulusPhase();
                    _visualManager.ShowCurrentStimulus(activeStimulus);
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
                _visualManager.RunIntroduction();
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
        Debug.Log("did they answer yes? " + didTheyAnswerYes);
        Debug.Log("is it superimpossible/achiral? " + _activeTestingObject.Superimposable);
        Debug.Log("were they correct?? " + isCorrect);//EVETUALLY THIS WILL GO TO A CSV FILE
    }

    /// <summary>
    /// Method <c>SetBetweenTrials</c> Puts program between trials
    /// </summary>
    public void SetBetweenTrials()
    {
        if (this.transform.childCount > 0)
        {
            Destroy(this.transform.GetChild(1).gameObject);
            Destroy(this.transform.GetChild(0).gameObject);
        }
        BetweenTrials = true;
        _betweenTrialCountdown = 100;
        _visualManager.RunBetweenTrials();
    }

    /// <summary>
    /// Method <c>NextTrial</c> Moves the experiment to the next trial
    /// </summary>
    public void NextTrial() {
        Debug.Log("running next trial");
        _trialCounter++;
        if (_trialCounter > NumberOfTrialsPerSection)
        {
            Debug.Log("Time for Feeback");
            TimeForFeedback = true;
            phase = Phase.Feedback;
        }
        else
        {
            _visualManager.RunExperimentQuestionText();
            RunTrial();
        }
    }

    /// <summary>
    /// Method <c>GetFeedback</c> Recieves participant answer in form of<paramref name="mouseX"/> and records the rating (TODO)
    /// <param name="mouseX"> Is the mouseX position, used to calculate the answer </param>
    /// </summary>
    public void GetFeedback(float mouseX)
    {
        float score = mouseX / Screen.width * 7.0f;
        Debug.Log(score);
        TimeForFeedback = false;
        _trialCounter = 0;
        if (_experimentInFirstPhase)
        {
            _experimentInFirstPhase = false;
            SetBetweenTrials();
            phase = Phase.Experimental;
        }
        else
        {
            phase = Phase.PostExperiment;
        }

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

    //private int GetIndexFromTrialAndSection()
    //{
    //    Debug.Log(String.Format("Number of trials per Section = {0}, Current Section = {1}, trial number for this section = {2}", NumberOfTrialsPerSection, _sectionCounter, _trialCounter));

    //    return _trialCounter+(NumberOfTrialsPerSection * (_sectionCounter-1)) - 1;
    //}
}
