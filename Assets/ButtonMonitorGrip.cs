using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ButtonMonitorGrip : MonoBehaviour
{
    public SteamVR_Action_Boolean action;
    public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;
    public SteamVR_Behaviour_Pose controllerPose;
    public LayerMask interactionLayers; // Specify which layers can be interacted with
    public float maxDistance = 10.0f; // Maximum distance for the raycast

    private GameObject targetObject; // The object currently being pointed at
    private GameObject objectInHand; // The object currently being held

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
        PointAndDetect();
    }

    void PointAndDetect()
    {
        RaycastHit hit;

        if (Physics.Raycast(controllerPose.transform.position, controllerPose.transform.forward, out hit, maxDistance, interactionLayers))
        {
            targetObject = hit.collider.gameObject;
        }
        else
        {
            targetObject = null;
        }
    }

    private void GrabObject()
    {
        objectInHand = targetObject;
        targetObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            objectInHand = null;
        }
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 2000;
        fx.breakTorque = 2000;
        return fx;
    }

    private void OnActionPressedOrReleased(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource, bool newstate)
    {
        if (newstate && targetObject)
        {
            GrabObject();
        }
        else if (!newstate && objectInHand)
        {
            ReleaseObject();
        }
    }
}
