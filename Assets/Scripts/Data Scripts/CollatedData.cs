using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>ClickData</c> This class stores collated data from all other data types
/// </summary>
public class CollatedData 
{

    public float Time;
    public string Type;
    public string Info;
    public CollatedData(float timeC, string typeC, string infoC)
    {
        Time = timeC;
        Type = typeC;
        Info = infoC;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
