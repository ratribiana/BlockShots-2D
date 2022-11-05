using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Trajectory Handler Settings")]
    [SerializeField] GameObject knobParent;
    [SerializeField] Transform shootPoint;
    LineRenderer _knobRenderer;

    [Header("Trajectory Settings")]
    [SerializeField] int dotsNumber;
    [SerializeField] float dotsSpacing;
    [SerializeField] GameObject dotsParent;
    [SerializeField] GameObject dotPrefab;
    Transform[] dotsList;
    Vector2 dotsPos;
    float timeStamp;

    [Header("Temporary values")]
    [SerializeField] GameObject projectile;
    [SerializeField] float pushForce;

    //PRIVATE VARIABLES FOR SHOOTING
    Vector2 force;
    Vector2 direction;
    float distance;

    private void Start()
    {
        //SETUP LINE RENDERER FOR KNOBS
        _knobRenderer = knobParent.AddComponent<LineRenderer>();
        _knobRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _knobRenderer.widthMultiplier = .15f;
        _knobRenderer.positionCount = 2;
        _knobRenderer.sortingOrder = 1;

        //SETUP LINE RENDERER POSITIONS FOR KNOB
        for (int i = 0; i < _knobRenderer.positionCount; i++)
        {
            _knobRenderer.SetPosition(i, knobParent.transform.GetChild(i).position);
        }
        PrepareDots();
    }
    private void Update()
    {

    }

    public void Shoot()
    {
        Projectile missile = Instantiate(projectile, shootPoint.position, Quaternion.identity).GetComponent<Projectile>();
        missile.Shoot(force);
    }

    #region Trajectory Handlers
    /// <summary>
    /// Prepares the dots to visualize trajectory of the projectile, MUST be called only ONCE
    /// </summary>
    void PrepareDots()
    {
        dotsList = new Transform[dotsNumber];
        dotPrefab.transform.localScale = Vector3.one * .4f;

        float scale = .4f;
        float scaleFactor = scale / dotsNumber;

        for (int i = 0; i < dotsNumber; i++)
        {
            dotsList[i] = Instantiate(dotPrefab, dotsParent.transform).transform;
            dotsList[i].localScale = Vector3.one * scale;
            if (scale > .2f)
                scale -= scaleFactor;
        }
    }

    /// <summary>
    /// Updates the position of knobs when setting up trajectory line, takes a transform argument
    /// as a reference for the knobs
    /// </summary>
    /// <param name="knobTransform"></param>
    public void UpdateKnobHandlers(Transform knobTransform)
    {
        //SHOOT POINT ANGLE ADJUSTMENTS
        Vector3 target = shootPoint.position - knobParent.transform.GetChild(0).position;
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
        shootPoint.rotation = Quaternion.Slerp(shootPoint.rotation, rot, Time.deltaTime * 20);

        //FORCE CALCULATION FOR THE KNOBS
        distance = Vector2.Distance(transform.position, knobParent.transform.GetChild(0).position);
        direction = (transform.position - knobParent.transform.GetChild(0).position).normalized;
        force = direction * distance * pushForce;

        for (int i = 0; i < _knobRenderer.positionCount; i++)
        {
            _knobRenderer.SetPosition(i, knobParent.transform.GetChild(i).position);
            for (int j = 0; j < knobParent.transform.childCount; j++)
            {
                if (!knobTransform.Equals(knobParent.transform.GetChild(j)))
                    knobParent.transform.GetChild(j).transform.localPosition = -knobTransform.localPosition;
            }
        }
        UpdateDots(transform.position, force);
    }
    void UpdateDots(Vector2 position, Vector2 force)
    {
        timeStamp = dotsSpacing;
        for (int i = 0; i < dotsNumber; i++)
        {
            dotsPos.x = (position.x + force.x * timeStamp);
            dotsPos.y = (position.y + force.y * timeStamp) - ((Physics2D.gravity.magnitude * (timeStamp * timeStamp)) / 2);
            dotsList[i].position = dotsPos;
            timeStamp += dotsSpacing;
        }
    }
    /// <summary>
    /// Simple toggle for the trajectory controls, takes a boolean to check whether
    /// to enable or disable the controls
    /// </summary>
    /// <param name="value"></param>
    void ToggleControls(bool value)
    {
        knobParent.transform.parent.gameObject.SetActive(value);
    }

    #endregion
}
