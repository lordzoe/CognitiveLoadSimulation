using UnityEngine;
using System.Collections;
using System.IO;

[RequireComponent(typeof(AudioSource))]
public class IntrinsicAudioPlayer : MonoBehaviour
{

    [SerializeField]
    private MainObjectManager _mainObjectManager;

    public AudioClip[] FillerAudio = new AudioClip[25];
    public AudioClip Trigger;
    AudioSource audioSource;

    public bool TriggerCalled = false;
    public float TimeAtTriggerCalled = 0f;

    public bool AudioOn = false;


    private int _avgIntTrig;
    private int _minIntTrig;
    private int _maxIntTrig;

    private int _numIntTillNextTrig;
    private int _numIntSinceLastTrig;
    private bool _lastIntWasTrig;


    //private int _c = 0;
    //private int[] _gTest = new int[10];




    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //StartIntrinsicAudio(3f, 5, 2);



    }

    void Update()
    {
        //for (int i = 0; i < 10; i++)
        //{
        //    _gTest[Mathf.FloorToInt(RandomBoxMuller.Range(3, 7 + 1))] += 1;
        //}
        //if (UnityEngine.Random.value > 0.99)
        //{

        //}
        //_c++;
        //if (_c == 500)
        //{
        //    for (int i = 0; i < _gTest.Length; i++)
        //    {
        //        Debug.Log(i + ">>> " + _gTest[i]);
        //    }
        //}

    }

    //void Play()
    //{
    //    audioSource.PlayOneShot(fillerAudio[0], 0.7F);
    //}

    /// <summary>
    /// Method <c>StartIntrinsicAudio</c> Starts repeating of the PlayAudioWithRandomChance method, which plays the intrinsic audio, at a time interval defined by <paramref name="intervalTime"/>
    /// Average number of intervals before a trigger is played is defined by <paramref name="averageIntervalsForTrigger"/>,
    /// bounded by a mininum and a maximum number of intervals as defined by <paramref name="minAndMaxRange"/>, which is the number of intervals above and below the average that are allowed
    /// Example, 3f, 5, 2, will have intervals of 3 seconds with the number of intnervals before the next trigger ranging from 3-7, weighted towards 5
    /// </summary>
    public void StartIntrinsicAudio(float intervalTime, int averageIntervalsForTrigger, int minAndMaxRange)
    {
        AudioOn = true;
        _lastIntWasTrig = true;
        _numIntSinceLastTrig = 0;

        InvokeRepeating("PlayAudioWithRandomChance", 0f, intervalTime);
        Debug.Log("hello");
        _avgIntTrig = averageIntervalsForTrigger;
        _minIntTrig = averageIntervalsForTrigger - minAndMaxRange;
        _maxIntTrig = averageIntervalsForTrigger + minAndMaxRange;
    }

    

    /// <summary>
    /// Method <c>StopIntrinsicAudio</c> Stops repeating of the PlayAudioWithRandomChance method, which plays the intrinsic audio.
    /// </summary>
    public void StopIntrinsicAudio()
    {
        AudioOn = false;

        CancelInvoke("PlayAudioWithRandomChance");
    }

    /// <summary>
    /// Method <c>PlayAudioWithRandomChance</c> Plays an audio file either randomly selected from array, or on random chance, a trigger audio.
    /// If trigger audio is played, flag it with a bool and record time of play
    /// Importantly, the time till next trig, doesnt include the just played audio so if A is said, and then the next cue should be in 4, it will go for example, T,D,G,L. 
    /// </summary>
    void PlayAudioWithRandomChance()
    {
        if (_lastIntWasTrig)
        {
            _numIntTillNextTrig = CalculateNextTrig();
            _lastIntWasTrig = false;
            Debug.Log(_numIntTillNextTrig);
        }

        if (_numIntTillNextTrig == _numIntSinceLastTrig)
        {
            audioSource.PlayOneShot(Trigger);
            _numIntSinceLastTrig = 0;
            _lastIntWasTrig = true;
            _mainObjectManager.AudioTD.Add(new AudioTriggerData(Time.time));
        }
        else
        {
            audioSource.PlayOneShot(FillerAudio[Mathf.FloorToInt(UnityEngine.Random.value * FillerAudio.Length)]); ;
            _numIntSinceLastTrig++;
        }


    }

    /// <summary>
    /// Method <c>CalculateNextTrig</c> calculates how many intervals until the next trig using the BoxMuller distrubution
    /// 
    /// </summary>
    int CalculateNextTrig()
    {
        return Mathf.FloorToInt(RandomBoxMuller.Range(_minIntTrig, _maxIntTrig + 1));
    }

    
}