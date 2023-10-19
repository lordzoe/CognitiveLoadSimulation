/*using System.Collections;
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

}*/

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private MainObjectManager _mainObjectManager;

    [SerializeField]
    private Camera _camera;

    private GameObject _attachedObject;
    private int _timeToWaitBetweenTrials = 90;

    public SteamVR_Action_Boolean grabGrip;
    public SteamVR_Action_Boolean interactUI;
    public SteamVR_Action_Boolean westDpad;
    public SteamVR_Action_Boolean eastDpad;
    public SteamVR_Action_Boolean centerDpad;
    public SteamVR_Action_Boolean triggerPress;
    public SteamVR_Behaviour_Pose leftHandPose;
    public SteamVR_Behaviour_Pose rightHandPose;

    void Start()
    {
        // Initialize SteamVR actions
        grabGrip = SteamVR_Actions._default.GrabGrip;
        interactUI = SteamVR_Actions._default.InteractUI;
        westDpad = SteamVR_Actions._default.WestDpad;
        eastDpad = SteamVR_Actions._default.EastDpad;
        centerDpad = SteamVR_Actions._default.CenterDpad;
    }

    // Fields from ButtonMonitorGrip.cs
    public LayerMask interactionLayers; // Specify which layers can be interacted with
    public float maxDistance = 10.0f; // Maximum distance for the raycast
    private GameObject targetObject; // The object currently being pointed at
    private GameObject objectInHand; // The object currently being held

    void Update()
    {
        // Handling GrabGrip action
        PointAndDetect();

        if (grabGrip.GetStateDown(SteamVR_Input_Sources.LeftHand) || grabGrip.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            if (targetObject)
            {
                GrabObject();
            }
            else
            {
                RaycastHit hit;
                Ray ray = _camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)); // center of the screen
                if (Physics.Raycast(ray, out hit))
                {
                    _attachedObject = hit.transform.gameObject;
                }
            }
        }
        if (grabGrip.GetStateUp(SteamVR_Input_Sources.LeftHand) || grabGrip.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
            else
            {
                _attachedObject = null;
            }
        }

        void PointAndDetect()
        {
            RaycastHit hit;
            if (Physics.Raycast(leftHandPose.transform.position, leftHandPose.transform.forward, out hit, maxDistance, interactionLayers))
            {
                targetObject = hit.collider.gameObject;
            }
            else
            {
                targetObject = null;
            }
        }

        private void GrabObject()
        {
            objectInHand = targetObject;
            targetObject = null;

            var joint = AddFixedJoint();
            joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
        }

        private void ReleaseObject()
        {
            if (GetComponent<FixedJoint>())
            {
                GetComponent<FixedJoint>().connectedBody = null;
                Destroy(GetComponent<FixedJoint>());
                objectInHand = null;
            }
        }

        private FixedJoint AddFixedJoint()
        {
            FixedJoint fx = gameObject.AddComponent<FixedJoint>();
            fx.breakForce = 2000;
            fx.breakTorque = 2000;
            return fx;
        }

        // Handling InteractUI action
        if (interactUI.GetStateDown(SteamVR_Input_Sources.LeftHand) || interactUI.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            RunMainClick();
        }

        // Handling WestDpad and EastDpad actions
        if (westDpad.GetStateDown(SteamVR_Input_Sources.LeftHand) || westDpad.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            // Record "Yes" response
            AnswerExperiment(true);
        }
        if (eastDpad.GetStateDown(SteamVR_Input_Sources.LeftHand) || eastDpad.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            // Record "No" response
            AnswerExperiment(false);
        }

        // Handling CenterDpad action
        if (centerDpad.GetStateDown(SteamVR_Input_Sources.LeftHand) || centerDpad.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            // Record that they heard letter "L"
            _mainObjectManager.RecordLetterL();  
        }

        {
        CheckControllerInput(leftHandPose);
        CheckControllerInput(rightHandPose);
        }

        void CheckControllerInput(SteamVR_Behaviour_Pose controllerPose)
        {
        if (triggerPress.GetStateDown(controllerPose.inputSource))
        {
            HandleTriggerPress(controllerPose.transform);
        }
        }

        void HandleTriggerPress(Transform controllerTransform)
        {
        RaycastHit hit;
        if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, out hit))
        {
            if (hit.transform.name.StartsWith("Sphere")) 
            {
                // Extracting the specific sphere number
                int sphereNumber = int.Parse(hit.transform.name.Replace("Sphere", ""));
                
                // Here we assume you want to pass this as a score:
                TakeFeedback(sphereNumber);
            }
        }
        else 
        {
            RunMainClick();
        }
        }

        public void TakeFeedback(int score)
        {
        _mainObjectManager.GetFeedback(score);
        }

        void RunMainClick()
        {
            Debug.Log("Pressed primary button.");

            switch (_mainObjectManager.ActivePhase)
            {
                case MainObjectManager.Phase.Start:
                    EndStart();
                    break;

                case MainObjectManager.Phase.Calibration:
                    EndCalibration();
                    break;

                case MainObjectManager.Phase.Tutorial:
                    EndTutorial();
                    break;

                case MainObjectManager.Phase.Rest:
                    // Assuming you have another state to distinguish between resting states
                    if (_mainObjectManager.IsPreExperimentalRest)
                    {
                        EndPreExperimental();
                    }
                    else
                    {
                        EndRestState();
                    }
                    break;

                // Add other phases as necessary

                default:
                    Debug.LogWarning("Unexpected phase or end of sequence reached.");
                    break;
            }
        }

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
    void EndPreExperimental() {
    _mainObjectManager.ActivePhase = MainObjectManager.Phase.Rest;
    _mainObjectManager.DoRestingState(true);
    }

    /// <summary>
    /// Method <c>EndRestState</c> End Rest Phase, and advance Phase to Feedback.
    /// </summary>
    void EndRestState() {
    _mainObjectManager.ActivePhase = MainObjectManager.Phase.Feedback;
    }

    /// <summary>
    /// Method <c>BeginExperiment</c> Begin the experiment.
    /// </summary>
    void BeginExperiment() {
    _mainObjectManager.BeginExperiment();
    }

    /// <summary>
    /// Method <c>ChangeToFeedBackPhase</c> Switch phase to feedback phase.
    /// </summary>
    void ChangeToFeedBackPhase() {
    _mainObjectManager.ActivePhase = MainObjectManager.Phase.Feedback;
    }

    /// <summary>
    /// Method <c>AnswerExperiment</c> Sends an answer to the experimental section based on the input boolean.
    /// </summary>
    /// <param name="answerYes">If true, the answer is "yes". Otherwise, the answer is "no".</param>
    void AnswerExperiment(bool answerYes) {
    _mainObjectManager.SetBetweenTrials(_timeToWaitBetweenTrials, answerYes);
    }
}



/*make sure these functions are integrated into InputManager.cs

Here are the Vive VR controller actions to integrate and link both InputManager.cs and ButtonMonitor.cs:
1) GrabGrip: The user can point the controller at an object and press down continuously on the grip key to rotate the object from a distance.
2) InteractUI: The user can move to the next phase by pressing the trigger key.
3) West D-pad: The user can press the west key of the d-pad to record a "Yes" response.
4) East D-pad: The user can press the east key of the d-pad to record a "No" response.
5) Center D-pad: The user can press the center key of the d-pad to record that they heard letter "L" in the audio from an n-back(0) task

Implement for both (left, right) Vive VR controllers
Remove any mouse click control*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using static ButtonMonitorWest;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private MainObjectManager _mainObjectManager;

    [SerializeField]
    private Camera _camera;

    private GameObject _attachedObject;
    private int _timeToWaitBetweenTrials = 90;

    public SteamVR_Action_Boolean grabGrip;
    public SteamVR_Action_Boolean interactUI;
    public ButtonMonitorWest westDpad;
    public SteamVR_Action_Boolean eastDpad;
    public SteamVR_Action_Boolean centerDpad;
    public SteamVR_Action_Boolean triggerPress;
    public SteamVR_Behaviour_Pose leftHandPose;
    public SteamVR_Behaviour_Pose rightHandPose;

    void Start()
    {
        // Initialize SteamVR actions
        grabGrip = SteamVR_Actions._default.GrabGrip;
        interactUI = SteamVR_Actions._default.InteractUI;
        westDpad = new ButtonMonitorWest();
        eastDpad = new SteamVR_Action_Boolean();
        //centerDpad = SteamVR_Actions._default.CenterDpad;
    }

    // Fields from ButtonMonitorGrip.cs
    public LayerMask interactionLayers; // Specify which layers can be interacted with
    public float maxDistance = 10.0f; // Maximum distance for the raycast
    private GameObject targetObject; // The object currently being pointed at
    private GameObject objectInHand; // The object currently being held

    void Update()
    {
        PointAndDetect();

        HandleGrabGrip();
        HandleInteractUI();
        HandleDpadInputs();
        CheckControllerInput(leftHandPose);
        CheckControllerInput(rightHandPose);
    }

    void HandleGrabGrip()
    {
        if (grabGrip.GetStateDown(SteamVR_Input_Sources.LeftHand) || grabGrip.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            if (targetObject)
            {
                GrabObject();
            }
            else
            {
                RaycastHit hit;
                Ray ray = _camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
                if (Physics.Raycast(ray, out hit))
                {
                    _attachedObject = hit.transform.gameObject;
                }
            }
        }

        if (grabGrip.GetStateUp(SteamVR_Input_Sources.LeftHand) || grabGrip.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
            else
            {
                _attachedObject = null;
            }
        }
    }

    void HandleInteractUI()
    {
        if (interactUI.GetStateDown(SteamVR_Input_Sources.LeftHand) || interactUI.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            RunMainClick();
        }
    }

    void HandleDpadInputs()
    {
        if (westDpad.action.GetStateDown(SteamVR_Input_Sources.LeftHand) || westDpad.action.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            Debug.Log("HandleDpadInputs: westPad is clicked");
            AnswerExperiment(true);
        }

        if (eastDpad.GetStateDown(SteamVR_Input_Sources.LeftHand) || eastDpad.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            Debug.Log("Maybe don't need the button monitor west?");
            AnswerExperiment(false);
        }

        if (centerDpad.GetStateDown(SteamVR_Input_Sources.LeftHand) || centerDpad.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            //_mainObjectManager.RecordLetterL();
        }
    }

        void PointAndDetect()
        {
            RaycastHit hit;
            if (Physics.Raycast(leftHandPose.transform.position, leftHandPose.transform.forward, out hit, maxDistance, interactionLayers))
            {
                targetObject = hit.collider.gameObject;
            }
            else
            {
                targetObject = null;
            }
        }

        private void GrabObject()
        {
            objectInHand = targetObject;
            targetObject = null;

            var joint = AddFixedJoint();
            joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
        }

        private void ReleaseObject()
        {
            if (GetComponent<FixedJoint>())
            {
                GetComponent<FixedJoint>().connectedBody = null;
                Destroy(GetComponent<FixedJoint>());
                objectInHand = null;
            }
        }

        private FixedJoint AddFixedJoint()
        {
            FixedJoint fx = gameObject.AddComponent<FixedJoint>();
            fx.breakForce = 2000;
            fx.breakTorque = 2000;
            return fx;
        }

    void CheckControllerInput(SteamVR_Behaviour_Pose controllerPose)
    {
        if (triggerPress.GetStateDown(controllerPose.inputSource))
        {
            HandleTriggerPress(controllerPose.transform);
        }
    }

    void HandleTriggerPress(Transform controllerTransform)
    {
        RaycastHit hit;
        if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, out hit))
        {
            if (hit.transform.name.StartsWith("Sphere"))
            {
                int sphereNumber = int.Parse(hit.transform.name.Replace("Sphere", ""));
                TakeFeedback(sphereNumber);
            }
        }
        else
        {
            RunMainClick();
        }
    }

        public void TakeFeedback(int score)
        {
        _mainObjectManager.GetFeedback(score);
        }

        void RunMainClick()
        {
            Debug.Log("Pressed primary button.");

            switch (_mainObjectManager.ActivePhase)
            {
                case MainObjectManager.Phase.Start:
                    EndStart();
                    break;

                case MainObjectManager.Phase.Calibration:
                    EndCalibration();
                    break;

                case MainObjectManager.Phase.Tutorial:
                    EndTutorial();
                    break;

                case MainObjectManager.Phase.Rest:
                    // Assuming you have another state to distinguish between resting states
                    /*if (_mainObjectManager.IsPreExperimentalRest)
                    {
                        EndPreExperimental();
                    }
                    else
                    {
                        EndRestState();
                    }*/
                    break;

                // Add other phases as necessary

                default:
                    Debug.LogWarning("Unexpected phase or end of sequence reached.");
                    break;
            }
        }

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
    void EndPreExperimental() {
    _mainObjectManager.ActivePhase = MainObjectManager.Phase.Rest;
    _mainObjectManager.DoRestingState(true);
    }

    /// <summary>
    /// Method <c>EndRestState</c> End Rest Phase, and advance Phase to Feedback.
    /// </summary>
    void EndRestState() {
    _mainObjectManager.ActivePhase = MainObjectManager.Phase.Feedback;
    }

    /// <summary>
    /// Method <c>BeginExperiment</c> Begin the experiment.
    /// </summary>
    void BeginExperiment() {
    _mainObjectManager.BeginExperiment();
    }

    /// <summary>
    /// Method <c>ChangeToFeedBackPhase</c> Switch phase to feedback phase.
    /// </summary>
    void ChangeToFeedBackPhase() {
    _mainObjectManager.ActivePhase = MainObjectManager.Phase.Feedback;
    }

    /// <summary>
    /// Method <c>AnswerExperiment</c> Sends an answer to the experimental section based on the input boolean.
    /// </summary>
    /// <param name="answerYes">If true, the answer is "yes". Otherwise, the answer is "no".</param>
    void AnswerExperiment(bool answerYes) {
    _mainObjectManager.SetBetweenTrials(_timeToWaitBetweenTrials, answerYes);
    }

}
