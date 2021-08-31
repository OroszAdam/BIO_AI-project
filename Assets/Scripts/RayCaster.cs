using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    Renderer rend;
    Mesh mesh;
    Collider m_Collider;
    Vector3 carMiddleFront;

    float playerCenterToFront;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        mesh = GetComponentInChildren<MeshFilter>().mesh;
        m_Collider = GetComponentInChildren<Collider>();

        playerCenterToFront = rend.bounds.extents.x;
    }
    // Update is called once per frame
        void FixedUpdate()
    {
        RaycastHit frontHit;
        RaycastHit frontSteerhit;
        carMiddleFront = transform.Find("CarFrontMiddle").position;

        //Front facing ray
        Physics.Raycast(carMiddleFront, transform.TransformDirection(Vector3.forward), out frontHit, Mathf.Infinity);
        Debug.DrawRay(carMiddleFront, transform.TransformDirection(Vector3.forward) * frontHit.distance, Color.red);
        Debug.Log("distance: " + frontHit.distance);

        //Little bit side-facing ray
        Physics.Raycast(carMiddleFront, transform.TransformDirection(Vector3.forward + new Vector3(10 * Mathf.PI / 180, 0, 0)), out frontSteerhit, Mathf.Infinity);
        Debug.DrawRay(carMiddleFront, transform.TransformDirection(Vector3.forward + new Vector3(10 * Mathf.PI / 180, 0, 0)) * frontSteerhit.distance, Color.red);
        Debug.Log("distance: " + frontSteerhit.distance);


        //// Does the ray intersect any objects excluding the player layer
        //if (Physics.Raycast(carMiddleFront, transform.TransformDirection(Vector3.forward + new Vector3(20 * Mathf.PI / 180, 0, 0)), out frontHit, Mathf.Infinity))
        //{
        //    Debug.DrawRay(carMiddleFront, transform.TransformDirection(Vector3.forward + new Vector3(20 * Mathf.PI / 180, 0, 0)) * hit.distance, Color.red);
        //    Debug.Log("Did Hit, distance: " + hit.distance);
        //}
        //else
        //{
        //    Debug.DrawRay(carMiddleFront, transform.TransformDirection(Vector3.forward + new Vector3(20 * Mathf.PI / 180, 0, 0)) * 1000, Color.blue);
        //    Debug.Log("Did not Hit");
        //}
        
    }

}
