using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR;

public class ButtonMonitorEast : MonoBehaviour
{
    public SteamVR_Action_Boolean action;
    public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;

    void OnEnable()
    {
        if (action != null)
        {
            action.AddOnChangeListener(OnActionPressedOrReleased, inputSource);
        }
    }

    private void OnDisable()
    {
        if (action != null)
        {
            action.RemoveOnChangeListener(OnActionPressedOrReleased, inputSource);
        }
    }

    private void OnActionPressedOrReleased(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource, bool newstate)
    {
        if (newstate) // If the action is pressed (newstate is true)
        {
            Debug.Log("Yes");
        }
        else
        {
            Debug.Log("Action was pressed or released");
        }
    }
}
