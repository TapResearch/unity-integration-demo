using UnityEngine;
using System;
using System.Collections;

public class TROrientationChanger : MonoBehaviour
{
    // Methods to set the orientation to portrait, landscape right or landscape left, ignoring landscape upside down.
    public void SetPortrait(Action onOrientationApplied)
    {
        Debug.Log("Unity C# TROrientationChanger: SetPortrait");
        StartCoroutine(ChangeOrientation(ScreenOrientation.Portrait, onOrientationApplied));
    }

    public void SetLandscapeLeft(Action onOrientationApplied)
    {
        Debug.Log("Unity C# TROrientationChanger: SetLandscapeLeft");
        StartCoroutine(ChangeOrientation(ScreenOrientation.LandscapeLeft, onOrientationApplied));
    }

    public void SetLandscapeRight(Action onOrientationApplied)
    {
        Debug.Log("Unity C# TROrientationChanger: SetLandscapeRight");
        StartCoroutine(ChangeOrientation(ScreenOrientation.LandscapeRight, onOrientationApplied));
    }

    // Set the orientation to auto-rotate (auto-rotate enabled)
    public void SetAutoRotate(Action onOrientationApplied)
    {
        Debug.Log("Unity C# TROrientationChanger: SetAutoRotate");
        StartCoroutine(ChangeOrientation(ScreenOrientation.AutoRotation, onOrientationApplied));
    }

    // Lock the current orientation (auto-rotate disabled)
    public void LockCurrentOrientation(Action onOrientationApplied)
    {
        Debug.Log("Unity C# TROrientationChanger: LockCurrentOrientation (turn off auto rotate)");
        StartCoroutine(ChangeOrientation(ScreenOrientation.AutoRotation, onOrientationApplied));
    }

    private IEnumerator ChangeOrientation(ScreenOrientation orientation, Action onOrientationApplied)
    {
        Screen.orientation = orientation;
        yield return null; // Wait for at least one frame

        onOrientationApplied.Invoke();
    }

}
