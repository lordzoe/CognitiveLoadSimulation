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
        ToggleYesNoText(false);
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
    /// Method <c>RunIntroduction</c> Does the visuals actions for the introduction.
    /// </summary>
    public void RunIntroduction()
    {
        _mainText.text = "This is the introduction activity, please follow the coming instructions";
        _textIsTemporary = true;
        _temporaryTextCountdown = _lengthOfTemporaryTextCountdown;
        _moreTextToCome = true;
        _additionalTextPiecesPointer = 0;
        _additionalTextPieces = new List<string>()
        {
            "Tutorial instruction 1",
            "Tutorial instruction 2"
        };

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
        _mainText.text = "When the molecules appear, please select if you think they are superimpossible or not superimpossible (ie if they are achiral or chiral)";
        ClearTemporaryTextPieces();
    }

    /// <summary>
    /// Method <c>RunExperimentQuestionText</c> Core text for experiment. Yes and no section.
    /// </summary>
    public void RunExperimentQuestionText()
    {
        _mainText.text = "Are these molecules non-superimpossible (chiral enatiomers)?";
        _mainText.fontSize = 16;
        _mainText.transform.position += new Vector3(0, -200, 0);
        ToggleYesNoText(true);
    }

    /// <summary>
    /// Method <c>RunBetweenTrials</c> Just for between two trials
    /// </summary>
    public void RunBetweenTrials()
    {
        ToggleYesNoText(false);
        _mainText.fontSize = 36;
        _mainText.transform.position += new Vector3(0, 200, 0);
        _mainText.text = "Thank you";
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
        

}
