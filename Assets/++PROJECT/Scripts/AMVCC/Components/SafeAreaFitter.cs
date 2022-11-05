using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    // Unknown is obsolete in ScreenOrientation
    private ScreenOrientation screenOrientation = ScreenOrientation.Portrait;

    private void Update()
    {
        // USING SCREEN ORIENTATION
        if ((Screen.orientation == ScreenOrientation.LandscapeLeft) &&
            (screenOrientation != ScreenOrientation.LandscapeLeft))
        {
            StartCoroutine(C_ChangedOrientation());
            screenOrientation = ScreenOrientation.LandscapeLeft;
        }
        else if ((Screen.orientation == ScreenOrientation.LandscapeRight) &&
            (screenOrientation != ScreenOrientation.LandscapeRight))
        {
            StartCoroutine(C_ChangedOrientation());
            screenOrientation = ScreenOrientation.LandscapeRight;
        }
    }

    private IEnumerator C_ChangedOrientation()
    {
        yield return new WaitForSeconds(.5f);
        
        var rectTransform = GetComponent<RectTransform>();
        var safeArea = Screen.safeArea;
        var anchorMin = safeArea.position;
        var anchorMax = anchorMin + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
