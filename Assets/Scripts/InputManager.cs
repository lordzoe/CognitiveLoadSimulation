using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    [SerializeField]
    private MainObjectManager _mainObjectManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
            RunClick();

        if (Input.GetMouseButtonDown(0))
        {

        }

        if (Input.GetMouseButtonDown(1))
            Debug.Log("Pressed secondary button.");

        if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle click.");
    }

    /// <summary>
    /// Method <c>RunClick</c> Runs the results of clicks
    /// </summary>
    void RunClick()
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
                Debug.Log("phase was intro");
                EndIntroduction();

                break;

            case MainObjectManager.Phase.PreExperimental:
                EndPreExperimental();
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
        _mainObjectManager.phase = MainObjectManager.Phase.PreExperimental;
    }

    /// <summary>
    /// Method <c>EndPreExperimental</c> End PreExperimental Phase, and advance Phase to Experimental.
    /// </summary>
    void EndPreExperimental()
    {
        _mainObjectManager.phase = MainObjectManager.Phase.Experimental;
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
