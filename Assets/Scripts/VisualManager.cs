using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class VisualManager : MonoBehaviour
{
    //holds the main text object (for now)
    [SerializeField]
    private TMP_Text _mainText;

    [SerializeField]
    private TMP_Text _yesText;

    [SerializeField]
    private TMP_Text _noText;

    [SerializeField]
    private TMP_Text _ratingText;

    [SerializeField]
    private TMP_Text _stimulusText;

    [SerializeField]
    private TMP_Text _exText;

    [SerializeField]
    private GameObject _distraction;

    [SerializeField]
    private GameObject _PAAS;

    private TutorialSection[] _tutSections;

    [SerializeField]
    private GameObject[] _tutorialGraphics;
    //private string[] _tutorialInstructions = new string[]
    //{
    // /*0*/  "Welcome! \n To begin, you’ll be given a short tutorial on how to navigate through the VR experience. \n Use your VR controllers to move through the tutorial. \n You will be shown molecule pairs, where you will have to identify whether they are superimposable or non-superimposable.",
    // /*1*/  "Let’s go over how objects can be rotated in 3D space. \n For each example, rotate the astronaut on the right so it matches the one on the left. \n Use the ___ button on your VR controller to interact with the astronaut. \n This astronaut was rotated 180 degrees around the y-axis!",
    // /*2*/  "Well done!",
    // /*3*/  "Rotate the astronaut on the right so it matches the one on the left. \n Use the ___ button on your VR controller to interact with the astronaut. \n This astronaut was rotated 180 degrees around the x-axis!",
    // /*4*/    "Well done!",
    // /*5*/   "Rotate the astronaut on the right so it matches the one on the left. \n Use the ___ button on your VR controller to interact with the astronaut. \n This astronaut was rotated 180 degrees around the z-axis!",
    // /*6*/    "Well done!",
    // /*7*/    "In this study, we are going to focus on how molecules can be rotated in 3D space. \n Rotate the molecule on the right so it matches the one on the left. \n Use the ___ button on your VR controller to interact with the molecule.",
    // /*8*/    "Well done! \n This is an example of a superimposable molecule pair (in this case due to being achiral): \n Superimposable molecules are molecules that, even though they might look different when you rotate or flip them, you can line them up so they look exactly the same, atom for atom. \n As you can see, when you rotate the right molecule 180 around the x-axis to line them up, they are identical.",
    // /*9*/    "Two molecules can be superimposable even when they are Chiral however \n prove this to yourself by making these two molecules match up",
    // /*10*/    "Well done",
    // /*11*/    "The definition of a chiral molecule is that a chiral molecule is non-superimposable with its mirror image! These non-superimposable mirror images are called enantiomers. Try rotating these molecules around to prove to yourself that these mirrored molecules are non superimposable!",
    // /*12*/    "This is the same as with your hands, where your hands being mirror images of each other cannot be rotated so that they superimpose!",
    // /*13*/    "Sometimes it won't be obvious whether a pair is superimposable or not, it can be helpful to rotate the molecules around to try and match features in order to determine if they are superimposable or not! \n Try that right now with this chiral pair!",
    // /*14*/    "As you can see when you line up the rest of their structure, that these two molecules are chiral mirror images of each other (ie enantiomers) and therefore non-superimposable.",
    // /*15*/    "This is another example of a non-superimposable molecule pair, in this case constitutional isomers: Not only can non-superimposable molecules differ based on their configurations, but they also differ based on their connectivity. Constitutional isomers are non-superimposable molecules that have the same number and type of atoms, but the atoms are connected in a different order or pattern.",
    // /*16*/    "In this study, we're interested in understanding your VR experience while performing certain tasks. To help us with that, we'll be using a tool called the Paas scale that allows you to rate your perceived mental effort during the tasks. \n Think of mental effort as the amount of brain power or energy you feel you're using while doing something. The Paas scale breaks down mental effort into different parts. You'll be rating your overall mental effort based on how well you were able to pay attention, how difficult or demanding the task felt, any frustration or annoyance you experienced, how motivated you were, and how engaged or absorbed you were in the task.",
    // /*15*/    "To rate your experience, you'll use a simple scale using emoticons. ‘Stressed’ emoticons further to the right of the scale indicate that higher levels of mental effort, attention, workload, frustration, motivation, or engagement were used during your VR experience. So, if you felt very focused and motivated during a task, you might rate it with a higher number. If it felt easy and you weren't frustrated at all, you might rate it with ‘relaxed’ emoticons further to the left of the scale. Your honest ratings will help us understand how the tasks affected you mentally and psychologically. Please remember that there are no right or wrong answers, just your personal experience.",
    // /*16*/    "As practice, rate your current mental effort based the amount of brain power or energy you feel you’re using while doing this tutorial. Click the emoticon below that best reflects your overall level of mental effort, attention, workload, frustration, motivation, and engagement.",
    // /*17*/    "In a moment you will have to solve a problem that should require low mental effort, you will have 7 seconds, ready?",
    // /*18*/    "What is X?",
    // /*19*/    "Thank you! Click the emoticon below that best reflects your overall level of mental effort, attention, workload, frustration, motivation, and engagement.",
    // /*20*/    "In a moment you will have to solve a problem that should require high mental effort, you will have 12 seconds, ready?",
    // /*21*/    "",
    // /*22*/    "Thank you! Click the emoticon below that best reflects your overall level of mental effort, attention, workload, frustration, motivation, and engagement.",
    // /*23*/    "Excellent, that completes the tutorial portion of the study, we will now begin the study proper, thank you very much!"
    //};



    private string _activeTutorialInstruction = "";

    //if this is enabled, countdown will begin, and when countdown reaches 0, the text will be removed
    private bool _textIsTemporary;

    //countdown for temporary text, only used if _textIsTemporary is true
    private int _temporaryTextCountdown;

    //how long to set _temporaryTextCountdown to
    private int _lengthOfTemporaryTextCountdown = 900;

    //if this bool is enabled, there will be more text drawn from _additionalTextPieces once the _temporaryTextCountdown counter reaches 0, this text will also be temporary and obey the _lengthOfTemporaryTextCountdown length
    private bool _moreTextToCome;

    //holds additional pieces of text to be added in sequence, after the time designated in _lengthOfTemporaryTextCountdown
    private List<string> _additionalTextPieces;

    //points to indexes for _additionalTextPieces
    private int _additionalTextPiecesPointer = 0;
    // Start is called before the first frame update
    void Start()
    {
        _mainText.text = "NO TEXT, if this is showing, there is an error";
        _ratingText.text = "";
        ToggleYesNoText(false);
        _stimulusText.text = "";
        _exText.text = "";
        _distraction.SetActive(false);
        foreach(GameObject g in _tutorialGraphics)
        {
            g.SetActive(false);
        }
        _tutSections = new TutorialSection[]
     {
        /*0*/  new TutorialSection("Welcome! \n To begin, you’ll be given a short tutorial on how to navigate through the VR experience. \n Use your VR controllers to move through the tutorial. \n You will be shown molecule pairs, where you will have to identify whether they are superimposable or non-superimposable."),
     /*1*/  new TutorialSection("Let’s go over how objects can be rotated in 3D space. \n For each example, rotate the astronaut on the right so it matches the one on the left. \n Use the ___ button on your VR controller to interact with the astronaut. \n This astronaut was rotated 180 degrees around the y-axis!"),
     /*2*/  new TutorialSection("Well done!"),
     /*3*/  new TutorialSection("Rotate the astronaut on the right so it matches the one on the left. \n Use the ___ button on your VR controller to interact with the astronaut. \n This astronaut was rotated 180 degrees around the x-axis!"),
     /*4*/    new TutorialSection("Well done!"),
     /*5*/   new TutorialSection("Rotate the astronaut on the right so it matches the one on the left. \n Use the ___ button on your VR controller to interact with the astronaut. \n This astronaut was rotated 180 degrees around the z-axis!"),
     /*6*/    new TutorialSection("Well done!"),
     /*7*/    new TutorialSection("In this study, we are going to focus on how molecules can be rotated in 3D space. \n Rotate the molecule on the right so it matches the one on the left. \n Use the ___ button on your VR controller to interact with the molecule."),
     /*8*/    new TutorialSection("Well done! \n This is an example of a superimposable molecule pair (in this case due to being achiral): \n Superimposable molecules are molecules that, even though they might look different when you rotate or flip them, you can line them up so they look exactly the same, atom for atom. \n As you can see, when you rotate the right molecule 180 around the x-axis to line them up, they are identical."),
     /*9*/    new TutorialSection("Two molecules can be superimposable even when they are Chiral however \n prove this to yourself by making these two molecules match up"),
     /*10*/    new TutorialSection("Well done"),
     /*11*/    new TutorialSection("The definition of a chiral molecule is that a chiral molecule is non-superimposable with its mirror image! These non-superimposable mirror images are called enantiomers. Try rotating these molecules around to prove to yourself that these mirrored molecules are non superimposable!"),
     /*12*/    new TutorialSection("This is the same as with your hands, where your hands being mirror images of each other cannot be rotated so that they superimpose!"),
     /*13*/    new TutorialSection("Sometimes it won't be obvious whether a pair is superimposable or not, it can be helpful to rotate the molecules around to try and match features in order to determine if they are superimposable or not! \n Try that right now with this chiral pair!"),
     /*14*/   new TutorialSection( "As you can see when you line up the rest of their structure, that these two molecules are chiral mirror images of each other (ie enantiomers) and therefore non-superimposable."),
     /*15*/    new TutorialSection("This is another example of a non-superimposable molecule pair, in this case constitutional isomers: Not only can non-superimposable molecules differ based on their configurations, but they also differ based on their connectivity. Constitutional isomers are non-superimposable molecules that have the same number and type of atoms, but the atoms are connected in a different order or pattern."),
     /*16*/    new TutorialSection("In this study, we're interested in understanding your VR experience while performing certain tasks. To help us with that, we'll be using a tool called the Paas scale that allows you to rate your perceived mental effort during the tasks. \n Think of mental effort as the amount of brain power or energy you feel you're using while doing something. The Paas scale breaks down mental effort into different parts. You'll be rating your overall mental effort based on how well you were able to pay attention, how difficult or demanding the task felt, any frustration or annoyance you experienced, how motivated you were, and how engaged or absorbed you were in the task."),
     /*17*/   new TutorialSection( "To rate your experience, you'll use a simple scale using emoticons. ‘Stressed’ emoticons further to the right of the scale indicate that higher levels of mental effort, attention, workload, frustration, motivation, or engagement were used during your VR experience. So, if you felt very focused and motivated during a task, you might rate it with a higher number. If it felt easy and you weren't frustrated at all, you might rate it with ‘relaxed’ emoticons further to the left of the scale. Your honest ratings will help us understand how the tasks affected you mentally and psychologically. Please remember that there are no right or wrong answers, just your personal experience.", _tutorialGraphics[0]),
     /*18*/   new TutorialSection( "As practice, rate your current mental effort based the amount of brain power or energy you feel you’re using while doing this tutorial. Click the emoticon below that best reflects your overall level of mental effort, attention, workload, frustration, motivation, and engagement.", _tutorialGraphics[1]),
     /*19*/    new TutorialSection("In a moment you will have to solve a problem that should require low mental effort, you will have 7 seconds, ready?"),
     /*20*/    new TutorialSection("If x + 10 = 28 \n What is X?"),
     /*21*/    new TutorialSection("Thank you! Click the emoticon below that best reflects your overall level of mental effort, attention, workload, frustration, motivation, and engagement.",_tutorialGraphics[1]),
     /*22*/    new TutorialSection("In a moment you will have to solve a problem that should require high mental effort, you will have 12 seconds, ready?"),
     /*23*/    new TutorialSection("", _tutorialGraphics[2]),
     /*24*/    new TutorialSection("Thank you! Click the emoticon below that best reflects your overall level of mental effort, attention, workload, frustration, motivation, and engagement.",_tutorialGraphics[1]),
     /*25*/    new TutorialSection("Excellent, that completes the tutorial portion of the study, we will now begin the study proper, thank you very much!")
     };
    }

    // Update is called once per frame
    void Update()
    {
        if (_textIsTemporary)
        {
            HandleTemporaryText();
        }
    }


    /// <summary>
    /// Method <c>ShowCurrentStimulus</c> displays active stimulus with 
    /// </summary>
    public void ShowCurrentStimulus(MainObjectManager.Stimulus stimulus)
    {
        _stimulusText.text = stimulus.ToString();
    }

    /// <summary>
    /// Method <c>DoStimulus</c> runs stimulus
    /// </summary>
    public void DoStimulus(MainObjectManager.Stimulus stimulus, bool exInputNeeded)
    {
        if (stimulus == MainObjectManager.Stimulus.Extrisnic && exInputNeeded)
        {
            _exText.text = "Press right click";
        }
        else
        {
            _exText.text = "";
        }

        if (stimulus == MainObjectManager.Stimulus.Intrinsic)
        {
            _distraction.SetActive(true);
        }
        else
        {
            _distraction.SetActive(false);
        }
    }

    /// <summary>
    /// Method <c>HandleTemporaryText</c> Does temporary text switching and counting
    /// </summary>
    public void HandleTemporaryText()
    {
        //Debug.Log(_temporaryTextCountdown);
        if (_temporaryTextCountdown > 0)
        {

            _temporaryTextCountdown -= 1;
        }
        else
        {
            if (_moreTextToCome) {

                _temporaryTextCountdown = _lengthOfTemporaryTextCountdown;
                _mainText.text = _additionalTextPieces[_additionalTextPiecesPointer];
                _additionalTextPiecesPointer++;
                if (_additionalTextPiecesPointer >= _additionalTextPieces.Count)
                {
                    _moreTextToCome = false;
                    _additionalTextPieces = new List<string>();
                    _additionalTextPiecesPointer = 0;
                }
            }
            else
            {
                _mainText.text = "";
                _textIsTemporary = false;
            }

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
        _moreTextToCome = false;
        _additionalTextPieces = new List<string>();
        _additionalTextPiecesPointer = 0;

    }

    /// <summary>
    /// Method <c>RunIntro</c> Does the visuals actions for the start.
    /// </summary>
    public void RunStart() //NTC these all run only starting visuals, maybe rename?
    {
        _mainText.text = "Welcome to the experiment, it will begin shortly";
        ClearTemporaryTextPieces();
    }

    /// <summary>
    /// Method <c>RunCalibration</c> Does the visuals actions for the calibration.
    /// </summary>
    public void RunCalibration()
    {
        _mainText.text = "Your headset is being calibrated, please follow the instructions on the screen and from the experimenter";
        ClearTemporaryTextPieces();
    }

    /// <summary>
    /// Method <c>ShowTutorialInstructions</c> Handles text for tutorial.
    /// </summary>
    public void ShowTutorialInstructions()
    {
        _mainText.fontSize = 24;
        _mainText.text = _activeTutorialInstruction;


        /*_textIsTemporary = true;
        _temporaryTextCountdown = _lengthOfTemporaryTextCountdown;
        _moreTextToCome = true;
        _additionalTextPiecesPointer = 0;
        _additionalTextPieces = new List<string>()
        {
            "Tutorial instruction 1",
            "Tutorial instruction 2"
        };*/

    }

    /// <summary>
    /// Method <c>RunPreExperiment</c> Does the visual action for PreExperiment. Which im imagining as a sort of waiting room lobby area.
    /// </summary>
    public void RunPreExperiment() {
        _mainText.text = "The experiment is about to begin";
        ClearTemporaryTextPieces();
    }

    /// <summary>
    /// Method <c>RunExperimentIntro</c> Core text for experiment. Yes and no section.
    /// </summary>
    public void RunExperimentIntro() {
        _mainText.text = "When the molecules appear, please select if you think they are superimpossible or not superimpossible";
        ClearTemporaryTextPieces();
    }

    /// <summary>
    /// Method <c>RunExperimentQuestionText</c> Core text for experiment. Yes and no section.
    /// </summary>
    public void RunExperimentQuestionText()
    {
        _mainText.text = "Are these molecules superimpossible?";
        _mainText.fontSize = 16;
        _mainText.transform.localPosition = new Vector3(0, -200, 0);
        ToggleYesNoText(true);
    }

    /// <summary>
    /// Method <c>RunBetweenTrials</c> Just for between two trials
    /// </summary>
    public void RunBetweenTrials()
    {
        ToggleYesNoText(false);
        _mainText.fontSize = 36;
        _mainText.transform.localPosition = new Vector3(0, 0, 0);
        _mainText.text = "Thank you";
        _ratingText.text = "";
    }


    /// <summary>
    /// Method <c>RunFeedback</c> Visuals for the feedback section
    /// </summary>
    public void RunFeedback()
    {
        ToggleYesNoText(false);
        _mainText.fontSize = 16;
        _mainText.transform.localPosition = new Vector3(0, 200, 0);
        _mainText.text = "Please rate factor X from 1 to 7";
        _ratingText.text = "1	2	3	4	5	6	7";
        _stimulusText.text = "";
        _distraction.SetActive(false);

    }

    /// <summary>
    /// Method <c>RunPostExperiment</c> Does the visual action for post experiment.
    /// </summary>
    public void RunPostExperiment()
    {
        _mainText.text = "Thank you for participating in this experiment please remove your headset/follow instructions from the experimenter";
        _ratingText.text = "";
        _mainText.fontSize = 36;
        _mainText.transform.localPosition = new Vector3(0, 0, 0);
        ClearTemporaryTextPieces();
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


    private void ToggleYesNoText(bool on)
    {
        if (on)
        {
            _yesText.text = "Yes";
            _noText.text = "No";
        }
        else
        {
            _yesText.text = "";
            _noText.text = "";
        }
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
