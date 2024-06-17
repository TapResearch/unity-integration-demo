using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TRWaiter : MonoBehaviour
{
    
    public void Wait(float waitTime, Action onComplete)
    {
        Debug.Log("Unity C# TRWaiter: Wait " + waitTime);
        StartCoroutine(DoWait(waitTime, onComplete));
    }

    private IEnumerator DoWait(float waitTime, Action onComplete)
    {
        float elapsedTime = 0f;
        Debug.Log("Unity C# TRWaiter: DoWait before while loop");
 
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            Debug.Log("Unity C# TRWaiter: DoWait inside while loop " + elapsedTime);
            yield return null;
        }

        Debug.Log("Unity C# TRWaiter: DoWait after while loop calling onComplet");
        onComplete.Invoke();
    }

}
