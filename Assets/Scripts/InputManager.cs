using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    [SerializeField]
    private MainObjectManager _mainObjectManager;

    [SerializeField]
    private Camera _camera;

    private GameObject _attachedObject;

    private Vector2 _lastMousePos;

    private float _mouseMovementAdjuster = 0.001f;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    

    // Update is called once per frame
    void Update()
    {
        Vector2 mPos = Input.mousePosition;

        if (_attachedObject != null)
        {
            Vector2 diffMPos = mPos - _lastMousePos;
            //diffMPos *= _mouseMovementAdjuster;
            Vector3 directionVec = new Vector3(diffMPos.x, diffMPos.y, 0);
            float mag = -directionVec.magnitude;
            Vector3 crossVec = Vector3.Cross(Vector3.forward, directionVec);
            _attachedObject.transform.RotateAround(_attachedObject.transform.position, crossVec, mag);
            // _attachedObject.transform.position += new Vector3(diffMPos.x,diffMPos.y,0);
        }

        if (_mainObjectManager.phase == MainObjectManager.Phase.Introduction)
        {
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
                _mainObjectManager.CheckTutorialConditions();
            }
        }

        if (Input.GetMouseButtonDown(0))
            RunMainClick();



        if (Input.GetMouseButtonDown(1))
            RunSecClick();

        if (Input.GetMouseButtonUp(1))
            _attachedObject = null;

        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("Pressed middle click.");
            if (_mainObjectManager.phase == MainObjectManager.Phase.Introduction)
            {
                EndIntroduction();
            }
        }

        _lastMousePos = mPos;
    }


    public void TakeFeedback(int score)
    {
        _mainObjectManager.GetFeedback(score);
    }

    /// <summary>
    /// Method <c>RunMainClick</c> Runs the results of left clicks
    /// </summary>
    void RunMainClick()
    {
        Debug.Log("Pressed primary button.");
        Vector3 mousePos = Input.mousePosition;
        Debug.Log(mousePos.x);
        Debug.Log(mousePos.y);
        switch (_mainObjectManager.phase)
        {
            case MainObjectManager.Phase.Start:
                Debug.Log("phase was start");
                EndStart();
                break;

            case MainObjectManager.Phase.Calibration:
                Debug.Log("phase was calib");
                EndCalibration();
                break;

            case MainObjectManager.Phase.Introduction:
                //Debug.Log("phase was intro");
                Debug.Log(_mainObjectManager.TutSubPhaseInd);
                if(_mainObjectManager.TutSubPhaseInd!=18&& _mainObjectManager.TutSubPhaseInd != 21&& _mainObjectManager.TutSubPhaseInd != 24)
                {
                    Debug.Log("here");
                    _mainObjectManager.TutSubPhaseInd++;
                }
                if (_mainObjectManager.TutSubPhaseInd >= _mainObjectManager.TutorialObjects.Length)
                {
                    EndIntroduction();
                }
                else
                {
                    _mainObjectManager.RunTutorialSection();
                }

                //EndIntroduction();

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
                    if (_mainObjectManager.phase == MainObjectManager.Phase.Experimental && !_mainObjectManager.ExperimentRunning)
                    {
                        BeginExperiment();
                        break;
                    }
                    /*if (_mainObjectManager.TimeForFeedback && _mainObjectManager.ExperimentRunning)
                    {
                        ChangeToFeedBackPhase();
                        break;
                    }*/
                    if (_mainObjectManager.ExperimentRunning && !_mainObjectManager.TimeForFeedback)
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
    /// Method <c>RunSecClick</c> Runs the results of right click
    /// </summary>
    void RunSecClick()
    {
        Debug.Log("Pressed Secondary Button");
        if (_mainObjectManager.phase == MainObjectManager.Phase.Experimental && _mainObjectManager.ActiveStimulus == MainObjectManager.Stimulus.Intrinsic)
        {
            _mainObjectManager.Clicks.Add(new ClickData(Time.time, _mainObjectManager.phase.ToString()));
            _mainObjectManager.AddClickData();
        }

        if (_mainObjectManager.phase == MainObjectManager.Phase.PreExperimental && !_mainObjectManager.ConditionIsExtrisinsic)
        {
            _mainObjectManager.Clicks.Add(new ClickData(Time.time, _mainObjectManager.phase.ToString()));
            _mainObjectManager.AddClickData();
        }

        if (_mainObjectManager.phase == MainObjectManager.Phase.Introduction) {

            if (_attachedObject == null)
            {
                RaycastHit hit;
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Transform objectHit = hit.transform;
                    if (hit.transform.parent.transform.parent != null)
                    {
                        _attachedObject = hit.transform.parent.transform.parent.transform.gameObject;
                    }
                }
            }
            else
            {
                _attachedObject = null;
            }
        }

        //if (_mainObjectManager.phase == MainObjectManager.Phase.Feedback)
        //{
        //    Debug.Log("got here");
        //    RaycastHit hit;
        //    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        Debug.Log("and here");
        //        Transform objectHit = hit.transform;
        //        PAASObj p = objectHit.gameObject.GetComponent<PAASObj>();
        //        Debug.Log(objectHit.name);
        //        Debug.Log(p.name);
        //        if (p != null)
        //        {
        //            Debug.Log(p.Value+" clicked value");
        //        }
        //    }
        //}

    }



    /// <summary>
    /// Method <c>EndStart</c> End Start Phase, and advance Phase to calibration.
    /// </summary>
    void EndStart() {
        _mainObjectManager.phase = MainObjectManager.Phase.Calibration;
    }

    /// <summary>
    /// Method <c>EndCalibration</c> End Calibration Phase, and advance Phase to introduction.
    /// </summary>
    void EndCalibration() {
        _mainObjectManager.phase = MainObjectManager.Phase.Introduction;
    }

    /// <summary>
    /// Method <c>EndIntroduction</c> End Introduction Phase, and advance Phase to PreExperimental.
    /// </summary>
    void EndIntroduction() {
        _mainObjectManager.phase = MainObjectManager.Phase.Rest;
        _mainObjectManager.DoRestingState(false);

    }

    /// <summary>
    /// Method <c>EndPreExperimental</c> End PreExperimental Phase, and advance Phase to Experimental.
    /// </summary>
    void EndPreExperimental()
    {
        _mainObjectManager.phase = MainObjectManager.Phase.Rest;
        _mainObjectManager.DoRestingState(true);
    }

    void EndRestState()
    {
        _mainObjectManager.phase = MainObjectManager.Phase.Feedback;
        /*if (_mainObjectManager.OnShortRest) {
            _mainObjectManager.phase = MainObjectManager.Phase.Experimental;
            if (_mainObjectManager.ExperimentRunning)
            {
                _mainObjectManager.SetBetweenTrials();
            }
        }
        else
        {
            _mainObjectManager.phase = MainObjectManager.Phase.PreExperimental;
            _mainObjectManager.RunPreExperiment();
        }*/
    }

    /// <summary>
    /// Method <c>BeginExperiment</c> Begin the experiment.
    /// </summary>
    void BeginExperiment()
    {
        _mainObjectManager.BeginExperiment();
    }

    

    //STC PERHAPS THE ABOVE METHODS COULD BE REDUCED TO ONE, ADVANCE PHASE, BUT FOR RIGHT NOW IM KEEPING IT AS GENERAL AS POSSIBLE.
    // yes it is inefficient, but these are like almost abstract methods right now, and i suspect they will be much more full by the end of things






    /// <summary>
    /// Method <c>ChangeToFeedBackPhase</c> Switch phase to feedback phase.
    /// </summary>
    void ChangeToFeedBackPhase()
    {
            _mainObjectManager.phase = MainObjectManager.Phase.Feedback;
    }



    

    /// <summary>
    /// Method <c>AnswerExperiment</c> Checks which side of the screen the click is on, and then sends an answer to the experimental section based on that.
    /// </summary>
    void AnswerExperiment(float mouseX)
    {
        int leftOrRight=0;//left is 1, right is 0
        
            if (mouseX < Screen.width / 2f)
            {
                leftOrRight = 1;
            }
            else
            {
                leftOrRight = 0;
            }
            Debug.Log("left(1) or right(0)?  "+leftOrRight);
            _mainObjectManager.AnswerQuestion(leftOrRight);
            _mainObjectManager.SetBetweenTrials();
    }



    


}
