using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock_Flick_Traj : MonoBehaviour
{
    float releaseTime;
    public GameManager gm;
    public GameObject circleTrajPrefab;
    GameObject rock;
    //Rock_Flick rockFlick;
    public bool isPressed;
    Rigidbody2D rb;
    Rigidbody2D launcherRb;
    Collider2D launcherCol;
    public GameObject launcher;

    Transform trans;
    public Vector2 velocity;
    Vector2 startPoint;
    Vector2 endPoint;
    public Vector2 force;

    Vector3 lastMouseCoordinate = Vector3.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trans = gameObject.transform;

        launcherCol = launcher.GetComponent<Collider2D>();
        launcherRb = launcher.GetComponent<Rigidbody2D>();
        GetComponent<SpringJoint2D>().connectedBody = launcherRb;
        launcherCol.enabled = false;

    }
    private void OnEnable()
    {

    }
    void Update()
    {
        if (isPressed)
        {
            rb.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // First we find out how much it has moved, by comparing it with the stored coordinate.
            Vector3 mouseDelta = Input.mousePosition - lastMouseCoordinate;

            if (mouseDelta != Vector3.zero)
            {
                Debug.Log("Mouse moving");
                Instantiate(circleTrajPrefab, gameObject.transform);
            }
            // Then we store our mousePosition so that we can check it again next frame.
            lastMouseCoordinate = Input.mousePosition;
        }
    }

    void OnMouseDown()
    {
        //rock = gm.rockList[gm.rockCurrent].rock;
        //rockFlick = rock.GetComponent<Rock_Flick>();

        //gameObject.SetActive(true);
        isPressed = true;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        //launcherCol.enabled = false;
    }

    void OnMouseDrag()
    {
        startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        endPoint = GetComponent<SpringJoint2D>().connectedBody.transform.position;
        force = GetComponent<SpringJoint2D>().GetReactionForce(Time.deltaTime);
    }

    private void OnMouseUp()
    {
        isPressed = false;
        gameObject.SetActive(false);
    }

    
}
