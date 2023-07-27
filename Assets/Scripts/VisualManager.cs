using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/// <summary>
/// Class <c>VisualManager</c> This class recieves inputs from MainObjectManager in order to produce all the visuals on the screen
/// </summary>
public class VisualManager : MonoBehaviour
{
    //holds the main text object (for now)
    [SerializeField]
    private GameObject _extrinsicObject; 

    //Holds the main text information
    [SerializeField]
    private TMP_Text _mainText;

    //Holds the background for the main text
    [SerializeField]
    private GameObject _mainBackground;

    //Holds the UI elements for the correct and incorrect visuals
    [SerializeField]
    private GameObject _correct;

    [SerializeField]
    private GameObject _incorrect;

    //Holds UI element for PAAS scale
    [SerializeField]
    private GameObject _PAAS;

    //Array holding the text for the tutorial sections and the corresponding graphics. Each tutorial section is like a slide in a slideshow
    private TutorialSection[] _tutSections;

    //Array holding the graphics for the tutorial
    [SerializeField]
    private GameObject[] _tutorialGraphics;

    //Holds the main camera
    [SerializeField]
    private Camera _cam;

    //Holds the active tutorial instructions
    private string _activeTutorialInstruction = "";

    //if this is enabled, countdown will begin, and when countdown reaches 0, the text will be removed
    private bool _textIsTemporary;

    //countdown for temporary text, only used if _textIsTemporary is true
    private int _temporaryTextCountdown;

    //how long to set _temporaryTextCountdown to
    private int _lengthOfTemporaryTextCountdown = 900;

    //the amount of space between the camera and the UI 
    private float _camSpacing = 1200f;

    // Start is called before the first frame update
    void Start()
    {
        _mainText.text = "NO TEXT, if this is showing, there is an error";

        //Clear visuals
        ToggleYesNoWhatever(false); 
        TurnOffCorrectIncorrectMarkers();
        foreach (GameObject g in _tutorialGraphics)
        {
            g.SetActive(false);
        }

        //Set the text and related graphics for the tutorial section
        _tutSections = new TutorialSection[]
     {
     /*0*/     new TutorialSection("Welcome! \n\nTo begin, you’ll be given a short tutorial on how to navigate through the VR experience. \n\nUse your VR controllers to move through the tutorial. \nYou will be shown molecule pairs, where you will have to identify \nwhether they are superimposable or non-superimposable."),
     /*1*/     new TutorialSection("Let’s go over how objects can be rotated in 3D space. \n\nFor each example, rotate the astronaut on the right so it matches the one on the left. \n\nUse the ___ button on your VR controller to interact with the astronaut. \n\nThis astronaut was rotated 180 degrees around the y-axis!"),
     /*2*/     new TutorialSection("Well done!"),
     /*3*/     new TutorialSection("Rotate the astronaut on the right so it matches the one on the left. \n\nUse the ___ button on your VR controller to interact with the astronaut. \n\nThis astronaut was rotated 180 degrees around the x-axis!"),
     /*4*/     new TutorialSection("Well done!"),
     /*5*/     new TutorialSection("Rotate the astronaut on the right so it matches the one on the left. \n\nUse the ___ button on your VR controller to interact with the astronaut. \n\nThis astronaut was rotated 180 degrees around the z-axis!"),
     /*6*/     new TutorialSection("Well done!"),
     /*7*/     new TutorialSection("In this study, we are going to focus on how molecules can be rotated in 3D space. \n\nRotate the molecule on the right so it matches the one on the left. \n\nUse the ___ button on your VR controller to interact with the molecule."),
     /*8*/     new TutorialSection("Well done!"), 
     /*9*/     new TutorialSection("This is an example of a superimposable molecule pair (in this case due to being achiral): \n\nSuperimposable molecules are molecules that, even though they might look different \nwhen you rotate or flip them, you can line them up so they look exactly the same, atom for atom. \n\nAs you can see, when you rotate the right molecule 180 around the x-axis to line them up, they are identical."),
     /*10*/    new TutorialSection("Two molecules can be superimposable even when they are Chiral however \nprove this to yourself by making these two molecules match up"),
     /*11*/    new TutorialSection("Well done"),
     /*12*/    new TutorialSection("The definition of a chiral molecule is that a chiral molecule is non-superimposable with its mirror image! \n\nThese non-superimposable mirror images are called enantiomers. \n\nTry rotating these molecules around to prove to yourself that these mirrored molecules are non superimposable!"),
     /*13*/    new TutorialSection("This is the same as with your hands, where your hands being mirror \nimages of each other cannot be rotated so that they superimpose!"),
     /*14*/    new TutorialSection("Sometimes it won't be obvious whether a pair is superimposable or not, \nit can be helpful to rotate the molecules around to try and match features in order \nto determine if they are superimposable or not! \n\nTry that right now with this chiral pair!"),
     /*15*/    new TutorialSection("As you can see when you line up the rest of their structure, \nthat these two molecules are chiral mirror images of each other \n(ie enantiomers) and therefore non-superimposable."),
     /*16*/    new TutorialSection("This is another example of a non-superimposable molecule pair, in this case constitutional isomers: \nNot only can non-superimposable molecules differ based on their configurations, \nbut they also differ based on their connectivity. \n\nConstitutional isomers are non-superimposable molecules that have the same number and type of atoms, \nbut the atoms are connected in a different order or pattern."),
     /*17*/    new TutorialSection("In this study, we're interested in understanding your VR experience while performing certain tasks.\nTo help us with that, we'll be using a tool called the Paas scale\nthat allows you to rate your perceived mental effort during the tasks. \n\nThink of mental effort as the amount of brain power or energy you feel you're using while doing something.\nThe Paas scale breaks down mental effort into different parts.\nYou'll be rating your overall mental effort based on how well you were able to pay attention,\nhow difficult or demanding the task felt,\nany frustration or annoyance you experienced, how motivated you were,\nand how engaged or absorbed you were in the task."),
     /*18*/    new TutorialSection("To rate your experience, you'll use a simple scale using emoticons.\n‘Stressed’ emoticons further to the right of the scale indicate that higher levels of mental effort,\nattention, workload, frustration, motivation, or engagement were used during your VR experience.\nSo, if you felt very focused and motivated during a task, you might rate it with a higher number.\nIf it felt easy and you weren't frustrated at all,\nyou might rate it with ‘relaxed’ emoticons further to the left of the scale.\nYour honest ratings will help us understand how the tasks affected you mentally and psychologically.\n\nPlease remember that there are no right or wrong answers, just your personal experience.", _tutorialGraphics[0]),
     /*19*/    new TutorialSection("As practice, rate your current mental effort based the amount of brain power or energy you feel you’re\nusing while doing this tutorial. Click the emoticon below that best reflects your overall level of mental effort,\nattention, workload, frustration, motivation, and engagement.", _tutorialGraphics[1]),
     /*20*/    new TutorialSection("In a moment you will have to solve a problem that should require low mental effort,\nyou will have 7 seconds, ready?"),
     /*21*/    new TutorialSection("If x + 10 = 28 \n What is x?"),
     /*22*/    new TutorialSection("Thank you! Click the emoticon below that best reflects your overall level of mental effort,\nattention, workload, frustration, motivation, and engagement.",_tutorialGraphics[1]),
     /*23*/    new TutorialSection("In a moment you will have to solve a problem that should require high mental effort,\nyou will have 12 seconds, ready?"),
     /*24*/    new TutorialSection("", _tutorialGraphics[2]),
     /*25*/    new TutorialSection("Thank you! Click the emoticon below that best reflects your overall level of mental effort,\nattention, workload, frustration, motivation, and engagement.",_tutorialGraphics[1]),
     /*26*/    new TutorialSection("Excellent, that completes the tutorial portion of the study,\nwe will now begin the study proper, thank you very much!")
     };

        _extrinsicObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_textIsTemporary) //if there is temporary text, make it happen
        {
            HandleTemporaryText(); 
        }
        if (_mainText.text.Equals("")) //if there is no text to be displayed, turn off the background 
        {
            _mainBackground.SetActive(false);
        }
        else
        {   //otherwise keep it on
            _mainBackground.SetActive(true);
        }
    }

    /// <summary>
    /// Method <c>ResetTextToHeadPos</c> Reset the text position based on where the camera (head) is, so it always is in front of the face
    /// </summary>
    public void ResetTextToHeadPos()
    {
        //gt the canvas 
        var canv = _mainBackground.gameObject.transform.parent.GetComponent<Canvas>();
        if (canv.renderMode == RenderMode.WorldSpace)
        {
            //Get cam position and set the UI based on that 
            Vector3 camPos = _cam.transform.position;
            Vector3 camLookDir = _cam.transform.forward;
            canv.transform.position = camPos+camLookDir*_camSpacing;
            canv.transform.LookAt(canv.transform.position*2-camPos);
        }
        else
        {
        }
    }

    /// <summary>
    /// Method <c>ShowExtrinsicStimulus</c> displays extrinsic model
    /// </summary>
    public void ShowExtrinsicStimulus(bool activeVal)
    {
        _extrinsicObject.SetActive(activeVal); 
    }

    /// <summary>
    /// Method <c>HandleTemporaryText</c> Does temporary text switching and counting
    /// </summary>
    public void HandleTemporaryText()
    {
        //if the countdown isnt at zero
        if (_temporaryTextCountdown > 0)
        {
            _temporaryTextCountdown -= 1; //count down
        }
        else
        {
            _mainText.text = "";
            _textIsTemporary = false;
        }
    }

    /// <summary>
    /// Method <c>ClearTemporaryTextPieces</c> Clears all the pieces that would continue the countdown of a temporary text,
    /// so that if the temporary text is active while a switch occurs, it will not apply to the next set of text
    /// </summary>
    public void ClearTemporaryTextPieces()
    {
        _temporaryTextCountdown = 0;
        _textIsTemporary = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_mainBackground.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Method <c>RunIntro</c> Does the visuals actions for the start.
    /// </summary>
    public void RunStart() 
    {
        _mainText.text = "Welcome to the experiment, it will begin shortly";
        _mainText.fontSize = 36;
        _mainBackground.transform.localPosition = Vector3.zero;
        _mainText.alignment = TextAlignmentOptions.Center;
        _mainText.alignment = TextAlignmentOptions.Midline;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_mainBackground.GetComponent<RectTransform>());
        ClearTemporaryTextPieces();
    }

    /// <summary>
    /// Method <c>RunCalibration</c> Does the visuals actions for the calibration.
    /// </summary>
    public void RunCalibration()
    {
        _mainText.text = "Your headset is being calibrated,\nplease follow the instructions on the screen and from the experimenter";
        _mainText.fontSize = 36;
        _mainText.alignment = TextAlignmentOptions.Center;
        _mainText.alignment = TextAlignmentOptions.Midline;
        _mainBackground.transform.localPosition = Vector3.zero;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_mainBackground.GetComponent<RectTransform>());
        ClearTemporaryTextPieces();
    }

    /// <summary>
    /// Method <c>ShowTutorialInstructions</c> Handles text for tutorial.
    /// </summary>
    public void ShowTutorialInstructions()
    {
        _mainText.text = _activeTutorialInstruction;
        _mainText.fontSize = 36;
        _mainText.alignment = TextAlignmentOptions.TopLeft;
        _mainBackground.transform.localPosition = new Vector3(0,300,0);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_mainBackground.GetComponent<RectTransform>());
    }

    /// <summary>
    /// Method <c>RunPreExperiment</c> Does the visual action for PreExperiment. Which im imagining as a sort of waiting room lobby area.
    /// </summary>
    public void RunPreExperiment(bool Intrinsic) {
        _mainBackground.transform.localPosition = new Vector3(0, 300, 0);
        _mainText.fontSize = 36; 
        if (Intrinsic)
        {
            _mainText.text = "We will be collecting a baseline for 5 minutes,\nplease press the ___ key whenever you hear the letter \"L\"";
        }
        else
        {
            _mainText.text = "We will be collecting a baseline for 5 minutes, please keep your headset on";
            _textIsTemporary = true;
            _temporaryTextCountdown = 300;
        }
        _PAAS.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_mainBackground.GetComponent<RectTransform>());
        ClearTemporaryTextPieces();
    }

    public void RunRest(bool ShortRest)
    {
        if (ShortRest)
        {
            _mainText.text = "You will now be given a 1.5 minute rest where you will have no visuals, please keep your VR headset on";
        }
        else
        {
            _mainText.text = "You will now be given a 3 minute rest where you will have no visuals, please keep your VR headset on";

        }
        _mainText.fontSize = 36;
        _textIsTemporary = true;
        _temporaryTextCountdown = 300;
        _mainBackground.transform.localPosition = new Vector3(0, 0, 0);
        _PAAS.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_mainBackground.GetComponent<RectTransform>());
        ToggleYesNoWhatever(false);

    }

    /// <summary>
    /// Method <c>RunExperimentIntro</c> Core text for experiment. Yes and no section.
    /// </summary>
    public void RunExperimentIntro() {
        _mainText.text = "When the molecules appear, please select if you think they are superimpossible or not superimpossible";
        _mainText.fontSize = 36;
        _mainBackground.transform.localPosition = new Vector3(0, 0, 0);

        _PAAS.SetActive(false);
        ClearTemporaryTextPieces();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_mainBackground.GetComponent<RectTransform>());

    }

    /// <summary>
    /// Method <c>RunExperimentQuestionText</c> Core text for experiment. Yes and no section.
    /// </summary>
    public void RunExperimentQuestionText(bool isIntrinsic)
    {
        TurnOffCorrectIncorrectMarkers();
        if (isIntrinsic)
        {
            _mainText.text = "Are these molecules superimpossible?\nSelect the answer on your VR controller.\nRemember to press the ___ key whenever you hear the letter \"L\"";
        }
        else
        {
            _mainText.text = "Are these molecules superimpossible?\nSelect the answer on your VR controller.";
        }
        _mainText.fontSize = 32;
        _mainBackground.transform.localPosition = new Vector3(0, 300, 0);
        ToggleYesNoWhatever(true);
        _PAAS.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_mainBackground.GetComponent<RectTransform>());

    }

    /// <summary>
    /// Method <c>RunBetweenTrials</c> Just for between two trials
    /// </summary>
    public void RunBetweenTrials(bool answerCorrect)
    {

        ToggleYesNoWhatever(false);
        _PAAS.SetActive(false);
        _mainText.fontSize = 36;
        _mainBackground.transform.localPosition = new Vector3(0, 0, 0);
        if (answerCorrect)
        {
            _correct.SetActive(true);
            _mainText.text = "Thank you.\nThat was the correct answer.";

        }
        else
        {
            _incorrect.SetActive(true);
            _mainText.text = "Thank you.\nThat was the incorrect answer.";

        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_mainBackground.GetComponent<RectTransform>());

    }


    /// <summary>
    /// Method <c>RunFeedback</c> Visuals for the feedback section
    /// </summary>
    public void RunFeedback()
    {
        Debug.Log("here");
        _mainBackground.SetActive(true);
        ToggleYesNoWhatever(false);
        TurnOffCorrectIncorrectMarkers();
        _mainText.fontSize = 36;
        _mainBackground.transform.localPosition = new Vector3(0, 300, 0);
        _mainText.text = "Click the emoticon below that best reflects your overall level of mental effort,\nattention, workload, frustration, motivation, and engagement.";
        _PAAS.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_mainBackground.GetComponent<RectTransform>());
        
    }

    /// <summary>
    /// Method <c>RunPostExperiment</c> Does the visual action for post experiment.
    /// </summary>
    public void RunPostExperiment()
    {
        _mainText.text = "Thank you for participating in this experiment,\nplease remove your headset/follow instructions from the experimenter";
        _mainText.fontSize = 36;
        _mainBackground.transform.localPosition = new Vector3(0, 0, 0);
        _PAAS.SetActive(false);
        ClearTemporaryTextPieces();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_mainBackground.GetComponent<RectTransform>());
    }

    public void SetTutorialVisuals(int index)
    {
        foreach(GameObject g in _tutorialGraphics)
        {
            g.SetActive(false);
        }
        TutorialSection t = _tutSections[index];
        _activeTutorialInstruction = t.text;
        if (t.graphic != null)
        {
            t.graphic.SetActive(true);
        }
    }


    private void ToggleYesNoWhatever(bool on)
    {
        ///do something, probably show yes and no attached to the VR controller. 
    }

    private void TurnOffCorrectIncorrectMarkers()
    {
        _correct.SetActive(false);
        _incorrect.SetActive(false);
    }


    private class TutorialSection{
        public string text;
        public GameObject graphic;
        public bool timer;
        public TutorialSection(string textC)
        {
            text = textC;
        }
        public TutorialSection(string textC, GameObject graphicC)
        {
            text = textC;
            graphic = graphicC;
        }
        public TutorialSection(string textC, GameObject graphicC, bool timerC)
        {
            text = textC;
            graphic = graphicC;
            timer = timerC;
        }

    }

        

}
