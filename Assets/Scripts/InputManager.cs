using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>InputManager</c> This class is the core input controller, and passes information to the main object manager based on inputs. It tries to do as little thinking as possible except in interpretting
/// inputs. It recieves only from the buttons on the paas scale images
/// </summary>
public class InputManager : MonoBehaviour
{
    /// <summary>
    /// holds MainObjectManager
    /// </summary>
    [SerializeField]
    private MainObjectManager _mainObjectManager;

    /// <summary>
    /// hold camera object, this should be set to the VR camera when in VR
    /// </summary>
    [SerializeField]
    private Camera _camera;

    /// <summary>
    /// holds whichever object is currently being grabbed
    /// </summary>
    private GameObject _attachedObject;

    private Vector2 _lastMousePos;

    /// <summary>
    /// Sets the time in frame updates between trials (90 is roughly 3 seconds)
    /// </summary>
    private int _timeToWaitBetweenTrials = 90; //not in seconds 
    
    // Start is called before the first frame update
    void Start()
    {

    }

    

    // Update is called once per frame
    void Update()
    {
        Vector2 mPos = Input.mousePosition;

        //this all handles stuff around moving objects with mouse and should be deleted once VR is implemented
        if (_attachedObject != null)
        {
            Vector2 diffMPos = mPos - _lastMousePos;
            Vector3 directionVec = new Vector3(diffMPos.x, diffMPos.y, 0);
            float mag = -directionVec.magnitude;
            Vector3 crossVec = Vector3.Cross(Vector3.forward, directionVec);
            _attachedObject.transform.RotateAround(_attachedObject.transform.position, crossVec, mag);
        }

        if (_mainObjectManager.ActivePhase == MainObjectManager.Phase.Tutorial)        //if the active phase is the tutorial

        {
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) { //if there is no click, check if there is a matching, for VR this should check if thereis an object being actively held by the vr controller 
                _mainObjectManager.CheckTutorialConditions();
            }
        }

        if (Input.GetMouseButtonDown(0)) //do left click
            RunMainClick();



        if (Input.GetMouseButtonDown(1)) //do right click
            RunSecClick();

        if (Input.GetMouseButtonUp(1)) //when right click released, remove an attached object
            _attachedObject = null;

        if (Input.GetMouseButtonDown(2)) //jump to end of tutorial on scroll wheel click
        {
            Debug.Log("Pressed middle click.");
            if (_mainObjectManager.ActivePhase == MainObjectManager.Phase.Tutorial)
            {
                EndTutorial();
            }
        }

        _lastMousePos = mPos;
    }

    /// <summary>
    /// Method <c>TakeFeedback</c> This function is called by the buttons on the PAAS scale, to pass score to the main object manager, the score value is determined by which button is clicked on (see PAASObj class)
    /// </summary>
    public void TakeFeedback(int score)
    {
        _mainObjectManager.GetFeedback(score);
    }

    /// <summary>
    /// Method <c>RunMainClick</c> Runs the results of left clicks. Mainly used for skipping through phases
    /// </summary>
    void RunMainClick()
    {
        Debug.Log("Pressed primary button.");
        Vector3 mousePos = Input.mousePosition;
        Debug.Log(mousePos.x);
        Debug.Log(mousePos.y);
        switch (_mainObjectManager.ActivePhase) //switch checks current phase and moves to appropriate next phase based on that
        {
            case MainObjectManager.Phase.Start:
                Debug.Log("phase was start");
                EndStart();
                break;

            case MainObjectManager.Phase.Calibration:
                Debug.Log("phase was calib");
                EndCalibration();
                break;

            case MainObjectManager.Phase.Tutorial: 
                Debug.Log(_mainObjectManager.TutSubPhaseInd);
                if(_mainObjectManager.TutSubPhaseInd!=19&& _mainObjectManager.TutSubPhaseInd != 22&& _mainObjectManager.TutSubPhaseInd != 25) //if not on a tutorial phase that requires feedback
                {
                    _mainObjectManager.NextSubTutPhase();
                }
                if (_mainObjectManager.TutSubPhaseInd >= _mainObjectManager.TutorialObjects.Length) //if at the end of tutorial, end it
                {
                    EndTutorial();
                }
                else //otherwise move to next section
                {
                    _mainObjectManager.RunTutorialSection();
                }
                break;

            case MainObjectManager.Phase.PreExperimental:

                EndPreExperimental();
                break;

            case MainObjectManager.Phase.Rest:
                EndRestState();
                break;

            case MainObjectManager.Phase.Experimental:
                if (!_mainObjectManager.BetweenTrials)
                {
                    if (_mainObjectManager.ActivePhase == MainObjectManager.Phase.Experimental && !_mainObjectManager.ExperimentRunning) //if the experiment hasnt begun yet
                    {
                        BeginExperiment(); //start experiment
                        break;
                    }
                    
                    if (_mainObjectManager.ExperimentRunning && !_mainObjectManager.TimeForFeedback) //if the experiment is running, answer a question based on click
                    {
                        AnswerExperiment(mousePos.x);
                        break;
                    }
                }
                break;

            case MainObjectManager.Phase.Feedback:
               

                break;

            case MainObjectManager.Phase.PostExperiment:

                break;
            default:
                Debug.LogError("YOU SHOULD NOT BE HERE CHECK CODE");
                break;
        }


        //CheckStart(); //less efficient memory and effort wise than putting a if statement here, but its easier to read visually. 
    }


    /// <summary>
    /// Method <c>RunSecClick</c> Runs the results of right click, mainly moving objects and doing audio triggers, this will be changed during VR integration so skipping commenting
    /// </summary>
    void RunSecClick()
    {
        Debug.Log("Pressed Secondary Button");
        if (_mainObjectManager.ActivePhase == MainObjectManager.Phase.Experimental && _mainObjectManager.ActiveStimulus == MainObjectManager.Stimulus.Intrinsic) 
        {
            _mainObjectManager.Clicks.Add(new ClickData(Time.time, _mainObjectManager.ActivePhase.ToString()));
            _mainObjectManager.AddClickData();
        }

        if (_mainObjectManager.ActivePhase == MainObjectManager.Phase.PreExperimental && !_mainObjectManager.ConditionIsExtrisinsic)
        {
            _mainObjectManager.Clicks.Add(new ClickData(Time.time, _mainObjectManager.ActivePhase.ToString()));
            _mainObjectManager.AddClickData();
        }

        if (_mainObjectManager.ActivePhase == MainObjectManager.Phase.Tutorial) {

            if (_attachedObject == null)
            {
                RaycastHit hit;
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Transform objectHit = hit.transform;
                    Debug.Log(objectHit.name+" hit"); 
                    if (hit.transform.parent.transform.parent != null)
                    {
                        _attachedObject = hit.transform.parent.transform.parent.transform.gameObject;
                        Debug.Log(_attachedObject.name+" selected");
                    }
                }
            }
            else
            {
                _attachedObject = null;
            }
        }
    }



    /// <summary>
    /// Method <c>EndStart</c> End Start Phase, and advance Phase to calibration.
    /// </summary>
    void EndStart() {
        _mainObjectManager.ActivePhase = MainObjectManager.Phase.Calibration;
    }

    /// <summary>
    /// Method <c>EndCalibration</c> End Calibration Phase, and advance Phase to tutorial.
    /// </summary>
    void EndCalibration() {
        _mainObjectManager.ActivePhase = MainObjectManager.Phase.Tutorial;
    }

    /// <summary>
    /// Method <c>EndTutorial</c> End Tutorial Phase, and advance Phase to PreExperimental.
    /// </summary>
    void EndTutorial() {
        _mainObjectManager.ActivePhase = MainObjectManager.Phase.Rest;
        _mainObjectManager.DoRestingState(false);

    }

    /// <summary>
    /// Method <c>EndPreExperimental</c> End PreExperimental Phase, and advance Phase to Experimental.
    /// </summary>
    void EndPreExperimental()
    {
        _mainObjectManager.ActivePhase = MainObjectManager.Phase.Rest;
        _mainObjectManager.DoRestingState(true);
    }

    /// <summary>
    /// Method <c>EndRestState</c> End Rest Phase, and advance Phase to Feedback.
    /// </summary>
    void EndRestState()
    {
        _mainObjectManager.ActivePhase = MainObjectManager.Phase.Feedback;
    }

    /// <summary>
    /// Method <c>BeginExperiment</c> Begin the experiment.
    /// </summary>
    void BeginExperiment()
    {
        _mainObjectManager.BeginExperiment();
    }

    /// <summary>
    /// Method <c>ChangeToFeedBackPhase</c> Switch phase to feedback phase.
    /// </summary>
    void ChangeToFeedBackPhase()
    {
            _mainObjectManager.ActivePhase = MainObjectManager.Phase.Feedback;
    }

    /// <summary>
    /// Method <c>AnswerExperiment</c> Checks which side of the screen the click is on, and then sends an answer to the experimental section based on that. This will be removed by VR integration
    /// </summary>
    void AnswerExperiment(float mouseX)
    {
        bool answerYes;

        if (mouseX < Screen.width / 2f)
            answerYes = true;
        else
            answerYes = false;
       
       _mainObjectManager.SetBetweenTrials(_timeToWaitBetweenTrials,answerYes);
    }



    


}
