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


    [Tooltip("This controls if the first set of tests is a control or experimental group. To be controlled by researcher.")]
    public bool ControlFirst = true;

    [Tooltip("Controls experimental condition type \n 0 = Intrinsic \n 1 = Extrinsic \n To be controlled by researcher.")]
    public int ExperimentalCondition = 0;

    

    [Tooltip("Containing list of all models used for testing \nLIST MUST BE IN CORRECT ORDER \nDo not modify this list unless you know what you are doing!!")]
    public List<TestingObject> OrderedListOfTestObjects;



    //Number of trials per section
    public int NumberOfTrialsPerSection = 2;

    //Number of sections. (Should be 2).
    public int NumberofSections = 2;

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

    public Phase phase;

    private Phase _previousPhase;

    //Is the experiment running? (better catch it!!) NOTE: This will remain true even during feedback periods of the experiment
    public bool ExperimentRunning = false;

    //When this bool is primed, the next click in input will switch phase to the feedback phase.
    public bool TimeForFeedback = false;

    
    

    [SerializeField]
    private VisualManager _visualManager;

    //Which of the questions (trial) the experiment is on (goes from 1 to NumberOfSections*NumberOfTrials, inclusive).
    private int _trialCounter = 1;
    //Which section (first or second) the experiment is on (should remain on either 1 or 2).
    private int _sectionCounter = 1;

    private int _betweenTrialCountdown = 0;
    public bool BetweenTrials = false;

    // Start is called before the first frame update
    void Start()
    {
        phase = Phase.Start;
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
    }

    /// <summary>
    /// Method <c>CheckPhase</c> Looks what the current phase is, and takes actions based on that. 
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
                _visualManager.RunExperimentIntro();
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
        OrderedListOfTestObjects[GetIndexFromTrialAndSection()].ProduceObjects();//STC idk if trial number needs to be passed, as it is currently avaliable by the class...
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
        else
        {
            didTheyAnswerYes = false;
        }
        bool isCorrect;

        if (didTheyAnswerYes != OrderedListOfTestObjects[GetIndexFromTrialAndSection()].SuperimposableMirrorImage)
        {
            isCorrect = true;
        }
        else
        {
            isCorrect = false;
        }
        Debug.Log("did they answer yes? " + didTheyAnswerYes);
        Debug.Log("is it superimpossible/achiral? " + OrderedListOfTestObjects[GetIndexFromTrialAndSection()].SuperimposableMirrorImage);
        Debug.Log("were they correct?? " + isCorrect);//EVETUALLY THIS WILL GO TO A CSV FILE
    }

    /// <summary>
    /// Method <c>SetBetweenTrials</c> Puts program between trials
    /// </summary>
    public void SetBetweenTrials()
    {
        Destroy(this.transform.GetChild(1).gameObject);
        Destroy(this.transform.GetChild(0).gameObject);
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
            phase = MainObjectManager.Phase.Feedback;
        }
        else
        {
            _visualManager.RunExperimentQuestionText();
            RunTrial();
        }



    }

    private int GetIndexFromTrialAndSection()
    {
        Debug.Log(String.Format("Number of trials per Section = {0}, \n Current Section = {1}, \n trial number for this section = {2}", NumberOfTrialsPerSection, _sectionCounter, _trialCounter));

        return _trialCounter+(NumberOfTrialsPerSection * (_sectionCounter-1)) - 1;
    }
}
