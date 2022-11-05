using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KnobHelper : MonoBehaviour
{
    PlayerController controller;
    Vector3 initialMousePos;

    private void Awake()
    {
        controller = GetComponentInParent<PlayerController>();
    }


    void OnMouseDrag()
    {
        Vector3 inputVector = controller.transform.position - GetMousePos();
        inputVector = (inputVector.magnitude > 2.0f) ? inputVector.normalized*2 : inputVector;

        transform.localPosition = -inputVector;
        controller.UpdateKnobHandlers(transform);
    }

    Vector3 GetMousePos()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }
}
