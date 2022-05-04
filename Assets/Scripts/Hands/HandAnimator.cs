using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using Unity.XR.Oculus;

[RequireComponent(typeof(Animator))]
public class HandAnimator : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private XRController _controller;

    private Animator _animator;

    private readonly List<Finger> gripFingers = new List<Finger>
    {
        new Finger(FingerType.Pinky),
        new Finger(FingerType.Ring),
        new Finger(FingerType.Middle)
    };

    private readonly List<Finger> pointFingers = new List<Finger>
    {
        new Finger(FingerType.Index),
        new Finger(FingerType.Thumb)
    };

    private Finger thumb = new Finger(FingerType.Thumb);

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update() 
    {
        if (!_controller.enableInputActions) return;
        // Get input values
        CheckGrip();
        CheckPointer();
        CheckThumb();

        // Smooth inputs
        SmoothFinger(gripFingers);
        SmoothFinger(pointFingers);
        SmoothFinger(thumb);

        // Set inputs
        AnimateFingers(gripFingers);
        AnimateFingers(pointFingers);
        AnimateFingers(thumb);
    }

    private void CheckGrip()
    {
        if (_controller.inputDevice.TryGetFeatureValue(
            CommonUsages.grip, out float gripValue))
        {
            SetFingerTargets(gripFingers, gripValue);
        }
    }

    private void CheckPointer()
    {
        if (_controller.inputDevice.TryGetFeatureValue(
            CommonUsages.trigger, out float gripValue))
        {
            SetFingerTargets(pointFingers, gripValue);
        }
    }

    private void CheckThumb()
    {
        bool primaryAxisTouch, primaryTouch, secondaryTouch;
        _controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out primaryAxisTouch);
	    _controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out primaryTouch);
	    _controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out secondaryTouch);

        bool down = primaryAxisTouch | primaryTouch | secondaryTouch;
        thumb.Target = down ? 1.0f : 0.0f;
    }

    private void SetFingerTargets(List<Finger> fingers, float target)
    {
        foreach(Finger finger in fingers)
        {
            finger.Target = target;
        }
    }

    private void SmoothFinger(IEnumerable<Finger> fingers)
    {
        foreach(Finger finger in fingers)
        {
            float t = _speed * Time.unscaledDeltaTime;
            finger.Current = Mathf.MoveTowards(finger.Current, finger.Target, t);
        }
    }

    private void SmoothFinger(Finger finger)
    {
        float t = _speed * Time.unscaledDeltaTime;
        finger.Current = Mathf.MoveTowards(finger.Current, finger.Target, t);
    }

    private void AnimateFingers(IEnumerable<Finger> fingers)
    {
        foreach(Finger finger in fingers)
        {
            AnimateFingers(finger.Type.ToString(), finger.Current);
        }
    }

    private void AnimateFingers(Finger finger)
    {
        AnimateFingers(finger.Type.ToString(), finger.Current);
    }

    private void AnimateFingers(string finger, float blend)
    {
        _animator.SetFloat(finger, blend);
    }
}