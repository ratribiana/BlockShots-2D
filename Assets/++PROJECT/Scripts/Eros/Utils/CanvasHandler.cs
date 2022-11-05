using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all of the basic handling for UI
/// </summary>
public class CanvasHandler : MonoBehaviour
{
    public bool tempLoading;

    #region Transition Handler
    public void ChangeScene(string name)
    {
        Transition.instance.StartTransition(name,Transition.LOADTYPE.Scene);
    }
    public void SwitchUI(object target, object current)
    {
        Transition.instance.StartTransition(target, current, Transition.LOADTYPE.UI);
    }

    IEnumerator Start()
    {
        float time = 0;

        if (tempLoading)
        {
            while (time < 5)
            {
                yield return new WaitForSeconds(1);
                time++;
            }
            ChangeScene("06 Battle Screen");
        }
        yield return null;
    }
    #endregion
}
