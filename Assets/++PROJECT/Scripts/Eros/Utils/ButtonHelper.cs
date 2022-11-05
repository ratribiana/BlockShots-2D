using UnityEngine;

public class ButtonHelper : MonoBehaviour
{
    private void Awake()
    {
        this.GetComponent<UnityEngine.UI.Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
}
