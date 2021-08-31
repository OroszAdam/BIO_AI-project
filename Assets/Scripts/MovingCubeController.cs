using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovingCubeController : MonoBehaviour
{
    private Quaternion initialRotation;
    private Quaternion finalRotation;
    private LTSpline spline;
    private LTDescr movement;
    private LTDescr rotation;
    private float ratioPassed;

    [Header("ID for the event parameter")]
    public int Id;
    [Header("Movement properties")]
    public bool StartPingPongOnLoad = false;
    public float MovementSpeed = 10f;
    public float DelayTime = 1f;
    public List<Transform> Points;

    private void Start()
    {
        // select every transform.position from the List<Transform> Points
        spline = new LTSpline(Points.Select(transform => transform.position).ToArray());

        GameEvents.Instance.onMovingBlockTriggerEnter += OnMovingBlockTriggerEnter;
        GameEvents.Instance.onMovingBlockTriggerExit += OnMovingBlockTriggerExit;

        initialRotation = transform.rotation;
        finalRotation = Points.Last().rotation;

        if (StartPingPongOnLoad)
        {
            movement = LeanTween.moveSpline(gameObject, spline.pts, MovementSpeed).setEase(LeanTweenType.easeInOutQuad).setLoopPingPong();
            rotation = LeanTween.rotate(gameObject, finalRotation.eulerAngles, MovementSpeed).setLoopPingPong();
        }

    }

    private void OnMovingBlockTriggerEnter(int id)
    {
        if (id == Id)
        {
            LeanTween.cancel(gameObject);

            movement = LeanTween.moveSpline(gameObject, spline.pts, MovementSpeed).setEase(LeanTweenType.easeInOutQuad);
            rotation = LeanTween.rotate(gameObject, finalRotation.eulerAngles, MovementSpeed);
        }
    }

    private void OnMovingBlockTriggerExit(int id)
    {
        if (id == Id)
        {
            // If the movement is completed, we need to create a new one in a reversed direction
            if (movement.ratioPassed == 1f)
            {
                movement = LeanTween.moveSpline(gameObject, spline.pts, MovementSpeed / 2).setEase(LeanTweenType.easeInOutQuad).setDirection(-1);
                rotation = LeanTween.rotate(gameObject, initialRotation.eulerAngles, MovementSpeed / 2);
            }
            else
            {
                movement.setDirection(-1).setTime(MovementSpeed / 2);
                rotation.setDirection(-1).setTime(MovementSpeed / 2);
            }
        }
    }

    private void Update()
    {
        if (movement != null)
            ratioPassed = movement.ratioPassed;
    }

    private void OnDestroy()
    {
        GameEvents.Instance.onMovingBlockTriggerEnter -= OnMovingBlockTriggerEnter;
        GameEvents.Instance.onMovingBlockTriggerExit -= OnMovingBlockTriggerExit;
    }

    void OnDrawGizmos()
    {
        if (spline != null)
        {
            Gizmos.color = Color.red;
            spline.gizmoDraw(); // debug aid to be able to see the path in the scene inspector
        }
    }
}
