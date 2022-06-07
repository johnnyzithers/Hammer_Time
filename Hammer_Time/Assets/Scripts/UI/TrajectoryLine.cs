using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    private LineRenderer lr;
    private EdgeCollider2D edgeCol;
    public GameObject launcher;
    public Traj_Transform trajTransform;
    public GameManager gm;
    public CameraManager cm;
    public TeamManager tm;

    GameObject rock;
    Rock_Info rockInfo;

    public float springDistance;
    public Vector3 springDirection;
    public float angle;

    public GameObject curlPointGO;
    public Vector3 curlPoint;
    public GameObject targetPointGO;
    public Vector3 targetPoint;
    public GameObject hogLinePointGO;
    public Vector3 hogLinePoint;

    public GameObject aimCircle;
    public int dotCount;
    public GameObject dot;

    public GameObject shootKnob;
    Color knobColour;

    public int lookAheadCount;
    List<GameObject> dots;
    List<Vector2> points;

    bool aiTurn;
    Vector2 initVel;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        edgeCol = GetComponent<EdgeCollider2D>();

        dots = new List<GameObject>();
        aimCircle.GetComponent<SpriteRenderer>().enabled = false;

        lr.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.name);

        if (collider == rock.GetComponent<Collider2D>())
        {
            Debug.Log(collider.name);
        }
        //else trajCollision = true;
    }

    private void Update()
    {
        if (gm.rockList.Count != 0 && gm.rockCurrent < gm.rockList.Count)
        {
            rock = gm.rockList[gm.rockCurrent].rock;
            rockInfo = gm.rockList[gm.rockCurrent].rockInfo;
            if (gm.aiTeamRed)
            {
                if (gm.redHammer)
                {
                    if (gm.rockCurrent % 2 == 0)
                        aiTurn = false;
                    else
                        aiTurn = true;
                }
                else
                {
                    if (gm.rockCurrent % 2 == 0)
                        aiTurn = true;
                    else
                        aiTurn = false;
                }
            }
            else if (gm.aiTeamYellow)
            {
                if (gm.redHammer)
                {
                    if (gm.rockCurrent % 2 == 0)
                        aiTurn = true;
                    else
                        aiTurn = false;
                }
                else
                {
                    if (gm.rockCurrent % 2 == 0)
                        aiTurn = false;
                    else
                        aiTurn = true;
                }
            }
        }

        knobColour = shootKnob.GetComponent<SpriteRenderer>().color;

        if (rock != null && rockInfo != null && rockInfo.released && !aiTurn)
        {
            float cohesion;
            if (gm.redHammer)
            {
                cohesion = 0;
                if (gm.rockCurrent % 2 == 0)
                {
                    for (int i = 0; i > tm.teamYellow.Length; i++)
                    {
                        cohesion += tm.teamYellow[i].charStats.sweepCohesion.GetValue();
                    }
                }
                else
                {
                    for (int i = 0; i > tm.teamRed.Length; i++)
                    {
                        cohesion += tm.teamRed[i].charStats.sweepCohesion.GetValue();
                    }
                }
            }
            else
            {
                cohesion = 0;
                if (gm.rockCurrent % 2 == 0)
                {
                    for (int i = 0; i > tm.teamRed.Length; i++)
                    {
                        cohesion += tm.teamRed[i].charStats.sweepCohesion.GetValue();
                    }
                }
                else
                {
                    for (int i = 0; i > tm.teamYellow.Length; i++)
                    {
                        cohesion += tm.teamYellow[i].charStats.sweepCohesion.GetValue();
                    }
                }
                Debug.Log("Cohesion is " + cohesion);
            }


            //lr.enabled = true;
            int counter = 0;
            List<Vector2> tempPoints = new List<Vector2>();

            if (points != null && points.Count > 0)
            {
                foreach (Vector2 point in points)
                {
                    if (point.y > rock.transform.position.y | point.y > rock.transform.position.y + (cohesion / 40f))
                        tempPoints.Add(point);
                    //else
                    //    lr.SetPosition(counter, new Vector3(point.x, point.y, 0f));

                }
            }

            if (tempPoints.Count > lookAheadCount)
                lr.positionCount = lookAheadCount;
            else
                lr.positionCount = tempPoints.Count;
            //else
            //    lr.positionCount = 6;

            for (int i = 0; i < lr.positionCount; i++)
            {
                lr.SetPosition(i, new Vector3(tempPoints[i].x, tempPoints[i].y, 0f));
            }
            foreach (Vector2 point in tempPoints)
            {
                if (counter <= lr.positionCount)
                counter++;
            }
        }

        if (rock && rockInfo && rockInfo.released && rockInfo.rest)
            lr.enabled = false;
        if (rock && rockInfo && rockInfo.hit)
            lr.enabled = false;
        if (aiTurn)
            lr.enabled = false;
        //if (lr.enabled && rockInfo && rockInfo.rest || rockInfo.hit)
        //{
        //    lr.enabled = false;
        //}
    }

    public void DrawTrajectory()
    {
        //aiTurn = false;
        lr.enabled = false;

        if (dots.Count != 0)
        {
            foreach (GameObject dot in dots)
            {
                Destroy(dot);
            }
            dots.Clear();
        }

        hogLinePoint = new Vector3(hogLinePointGO.transform.position.x, -15.75f, 0f);
        curlPoint = curlPointGO.transform.position;
        targetPoint = targetPointGO.transform.position;
        springDistance = trajTransform.springDistance;
        List<Vector3> pos = new List<Vector3>();

        if (springDistance < 1)
        {
            lr.positionCount = 2;
        }
        else if (springDistance >= 1f)
        {
            if (springDistance < 1.25)
            {
                lr.positionCount = Mathf.RoundToInt(Mathf.Lerp(3f, 5f, springDistance));
            }
            else if (springDistance < 1.5)
            {
                lr.positionCount = Mathf.RoundToInt(Mathf.Lerp(5f, 100f, springDistance));
            }
            else
            {
                float dist = Vector2.Distance(new Vector2(0, -25), aimCircle.transform.position);
                lr.positionCount = 250;
            }
        }

        lr.startWidth = Mathf.Lerp(0f, 0.3f, springDistance / 3.25f);
        lr.endWidth = Mathf.Lerp(0f, 0.1f, springDistance / 3.25f);

        float t = 0f;
        Vector3 B;
        lr.SetPosition(0, launcher.transform.position);

        for (int i = 1; i < lr.positionCount; i++)
        {
            B = ((1 - t) * (1 - t) * hogLinePoint) + (2 * (1 - t) * t * curlPoint) + (t * t * targetPoint);
            
            lr.SetPosition(i, B);
            pos.Add(B);
            t += (1 / (float)lr.positionCount);
        }

        int counter = lr.positionCount / dotCount;
        GameSettingsPersist gsp = FindObjectOfType<GameSettingsPersist>();
        float dotStat = (gsp.cStats.drawAccuracy + gsp.cStats.takeOutAccuracy + gsp.cStats.guardAccuracy) / 6.5f;

        Debug.Log("Dotstat is " + dotStat);

        for (int i = 1; i < dotCount; i++)
        {
            Vector2 dotPos = pos[i * counter];

            //if (dotPos.y > 0f)
            //{
            //    dotPos = new Vector2(dotPos.x, 0f);
            //}
            GameObject dotPlace = Instantiate(dot, dotPos, Quaternion.identity);
            dotPlace.transform.parent = transform;
            dotPlace.GetComponent<SpriteRenderer>().color = knobColour;

            if (dotPos.y > (0f + dotStat))
            {
                dotPlace.transform.localScale = new Vector3(0f, 0f, 0f);
            }
            dots.Add(dotPlace);
            
        }

        points = new List<Vector2>();

        for (int i = 0; i < pos.Count; i++)
        {
            points.Add(pos[i]);
            //points[i] = new Vector2(pos[i].x, pos[i].y);
        }


        aimCircle.GetComponent<SpriteRenderer>().enabled = true;

        aimCircle.transform.position = lr.GetPosition(lr.positionCount - 1);
        aimCircle.GetComponent<SpriteRenderer>().color = knobColour;

        //lr.SetPositions(lr.GetPositions());
        //DrawQuadraticBezierCurve(hogLinePoint, curlPoint, targetPoint);
        //edgeCol.SetPoints(pos2D);

    }

    public void Release()
    {
        aimCircle.GetComponent<SpriteRenderer>().enabled = false;
        //lr.enabled = true;
        lr.startWidth = 0.075f;
        lr.endWidth = 0.075f;
        
        if (dots.Count != 0)
        {
            foreach (GameObject dot in dots)
            {
                Destroy(dot);
            }
            dots.Clear();
        }

    }


    //void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2)
    //{
    //    lr.positionCount = 200;
    //    float t = 0f;
    //    Vector3 B = new Vector3(0, -25, 0);

    //    lr.SetPosition(0, launcher.transform.position);

    //    for (int i = 1; i<lr.positionCount; i++)
    //    {
    //        B = ((1 - t) * (1 - t) * point0) + (2 * (1 - t) * t * point1) + (t * t * point2);
    //        lr.SetPosition(i, B);
    //        t += (1 / (float)lr.positionCount);
    //    }
    //}
}