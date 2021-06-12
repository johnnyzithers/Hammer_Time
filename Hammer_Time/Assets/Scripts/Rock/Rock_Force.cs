using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock_Force : MonoBehaviour
{
    private Rigidbody2D body;

    public float turnValue = 60f;
    public Vector2 curl;
    public float scaleFactor;

    float velX = 0f;
    float velY = 0f;
    bool turnStart;
    bool forceStart;
    //bool debugVertex;
    public bool flipAxis = false;
    //public bool moving;
    int dirMult = 1;

    GameObject trajLineGO;
    TrajectoryLine trajLine;
    AudioManager am;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        //trajLineGO = GameObject.Find("TrajectoryLine");
        //trajLine = trajLineGO.GetComponent<TrajectoryLine>();

        am = FindObjectOfType<AudioManager>();
    }


    public void Release()
    {
        if (flipAxis)
        {
            dirMult = -1;
        }

        GetComponent<SpriteRenderer>().enabled = true;
        turnStart = true;
        forceStart = true;
        //debugVertex = true;
        return;
    }

    void FixedUpdate()
    {
        velX = body.angularVelocity;

        Vector2 vel = new Vector2(velX * scaleFactor, velY);
        
        if (turnStart == true)
        {
            body.AddTorque(dirMult * turnValue * Mathf.Deg2Rad, ForceMode2D.Impulse);
            Debug.Log("Rotate");
            turnStart = false;

            //Debug.Log("vertex 1 is " + body.position.x + ", " + body.position.y + Time.deltaTime);
        }

        if (forceStart == true)
        {
            //Debug.Log("Curl Force");
            body.AddForce(curl * vel, ForceMode2D.Force);
            //Debug.Log("curl is " + curl.x);
            if (Mathf.Abs(body.velocity.y) < 0.01f && Mathf.Abs(body.velocity.x) < 0.01f)
            {
                //Debug.Log("Velocity below 0.01");
                am.Stop("RockScrape");
                GetComponent<Rock_Info>().stopped = true;
                GetComponent<Rock_Info>().rest = true;
                body.drag = 0.55f;
            }

            //if (debugVertex)
            //{
            //    if (body.velocity.x <= 0.01f)
            //    {
            //        Debug.Log("vertex 1 is " + body.position.x + ", " + body.position.y + Time.deltaTime);
            //        debugVertex = false;
            //    }
            //}
            
        }

    }
}
