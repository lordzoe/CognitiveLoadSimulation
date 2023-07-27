using UnityEngine;
using System.Collections;
using System.IO;

[RequireComponent(typeof(AudioSource))]
public class IntrinsicAudioPlayer : MonoBehaviour
{

    [SerializeField]
    private MainObjectManager _mainObjectManager;


    public AudioClip[] FillerAudio = new AudioClip[25]; //Array holding the audio cues that do not require response (ie filler)
    public AudioClip Trigger; //Holds the trigger audio, the letter "L"


    private AudioSource _audioSource; //Player for the audio

   

    public bool AudioOn = false; //holds whether or not the audio is playing or not


    private int _minIntTrig; //mininum number of filler audio clips to be played before the next trigger audio
    private int _maxIntTrig; //max number of filler audio clips to be played before the next trigger audio

    private int _numIntTillNextTrig; //number of filler audio cues played since the last trigger audio cue
    private int _numIntSinceLastTrig; //number of filler audio cues before the next trigger audio cue
    private bool _lastAudioCueWasTrig; // was the last audio cue played the trigger audio or not 

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        //StartIntrinsicAudio(3f, 5, 2);



    }

    void Update()
    {

    }

    /// <summary>
    /// Method <c>StartIntrinsicAudio</c> Starts repeating of the PlayAudioWithRandomChance method, which plays the intrinsic audio, at a time interval defined by <paramref name="intervalTime"/>
    /// Average number of intervals before a trigger is played is defined by <paramref name="averageIntervalsForTrigger"/>,
    /// bounded by a mininum and a maximum number of intervals as defined by <paramref name="minAndMaxRange"/>, which is the number of intervals above and below the average that are allowed
    /// Example, 3f, 5, 2, will have intervals of 3 seconds with the number of intnervals before the next trigger ranging from 3-7, weighted towards 5
    /// </summary>
    public void StartIntrinsicAudio(float intervalTime, int averageIntervalsForTrigger, int minAndMaxRange)
    {
        //reset the variables
        AudioOn = true;
        _lastAudioCueWasTrig = true;
        _numIntSinceLastTrig = 0; 

        //calculate min and max
        _minIntTrig = averageIntervalsForTrigger - minAndMaxRange;
        _maxIntTrig = averageIntervalsForTrigger + minAndMaxRange;

        //turn on the audio
        InvokeRepeating("PlayAudioWithRandomChance", 0f, intervalTime);
    }

    /// <summary>
    /// Method <c>StopIntrinsicAudio</c> Stops repeating of the PlayAudioWithRandomChance method, which plays the intrinsic audio.
    /// </summary>
    public void StopIntrinsicAudio()
    {
        AudioOn = false;

        //turn off the audio
        CancelInvoke("PlayAudioWithRandomChance");
    }

    /// <summary>
    /// Method <c>PlayAudioWithRandomChance</c> Plays an audio file either randomly selected from array, or on random chance, a trigger audio.
    /// If trigger audio is played, flag it with a bool and record time of play
    /// Importantly, the time till next trig, doesnt include the just played audio so if A is said, and then the next cue should be in 4, it will go for example, T,D,G,L. 
    /// </summary>
    void PlayAudioWithRandomChance()
    {
        if (_lastAudioCueWasTrig) //if the last cue was the trigger audio calculate when the next audio trigger should be
        {
            _numIntTillNextTrig = CalculateNextTrig();
            _lastAudioCueWasTrig = false;
        }

        if (_numIntTillNextTrig == _numIntSinceLastTrig) //if the correct number of filler audio cues have been played, play the trigger audio
        {
            _audioSource.PlayOneShot(Trigger); //play the audio

            //reset the values, and flag that the trigger audio was just played
            _numIntSinceLastTrig = 0;
            _lastAudioCueWasTrig = true;

            //save the data of when the audio cue was played to the list of audio cue data in _mainObjectManager
            _mainObjectManager.AudioTD.Add(new AudioTriggerData(Time.time, _mainObjectManager.ActivePhase.ToString()));
        }
        else //if not time for the trigger audio
        {
            //play filler audio and advance the counter by one
            _audioSource.PlayOneShot(FillerAudio[Mathf.FloorToInt(UnityEngine.Random.value * FillerAudio.Length)]); ;
            _numIntSinceLastTrig++;
        }


    }

    /// <summary>
    /// Method <c>CalculateNextTrig</c> calculates how many intervals until the next trig using the BoxMuller distrubution
    /// 
    /// </summary>
    int CalculateNextTrig()
    {
        return Mathf.FloorToInt(RandomBoxMuller.Range(_minIntTrig, _maxIntTrig + 1)); //google randombox muller, but the TLDR is that it creates a bell curve distribution 
    }

    
}