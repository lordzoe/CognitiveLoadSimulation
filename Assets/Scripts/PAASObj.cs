using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
