using UnityEngine;
using Valve.VR;

public class ButtonMonitorTrigger : MonoBehaviour
{
    public SteamVR_Action_Boolean action;
    public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;
    public MainObjectManager mainObjectManager; // Reference to the script containing AddClickData

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
        // When the trigger button is pressed
        if (newstate && mainObjectManager != null)
        {
            mainObjectManager.AddClickData();
        }
    }
}
