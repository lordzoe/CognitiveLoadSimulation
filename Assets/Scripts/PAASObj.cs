using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>PAASObj</c> This class is assigned to each of the PAAS emojis, and allows for the passing of values along, this will undergo some changes for VR
/// </summary>
public class PAASObj : MonoBehaviour
{
    public int Value = 0;

    [SerializeField]
    private InputManager _inputManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickedOn()
    {
        _inputManager.TakeFeedback(Value);
    }
}
