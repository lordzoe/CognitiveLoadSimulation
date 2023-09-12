using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ButtonMonitorDpad : MonoBehaviour
{
    public SteamVR_Action_Boolean action;
    public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;

    public SteamVR_Behaviour_Pose pose; 
    public float maxDistance = 100f;
    private GameObject pointedObject = null;

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

    void Update()
    {
        CheckPointedObject();
    }

    void CheckPointedObject()
    {
        Ray ray = new Ray(pose.transform.position, pose.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance); // Get all intersections

        GameObject closestSphere = null;
        float closestDistance = maxDistance;

        foreach (var hit in hits)
        {
            if (hit.transform.name.StartsWith("Sphere"))
            {
                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    closestSphere = hit.transform.gameObject;
                }
            }
        }

        pointedObject = closestSphere;
    }

    private void OnActionPressedOrReleased(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource, bool newstate)
    {
        if (newstate && pointedObject) // If the action is pressed (newstate is true) and an object is pointed at
        {
            Debug.Log($"Selected object: {pointedObject.name}");
        }
    }
}