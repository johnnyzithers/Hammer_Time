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

    GameObject rock;
    public float springDistance;
    public Vector3 springDirection;
    public float angle;

    public GameObject curlPointGO;
    public Vector3 curlPoint;
    public GameObject targetPointGO;
    public Vector3 targetPoint;
    public GameObject hogLinePointGO;
    public Vector3 hogLinePoint;
    bool trajCollision;


    void Start()
    {
        lr = GetComponent<LineRenderer>();
        edgeCol = GetComponent<EdgeCollider2D>();

        trajCollision = false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.name);

        if (collider == rock.GetComponent<Collider2D>())
        {
            Debug.Log(collider.name);
            trajCollision = true;
        }
        //else trajCollision = true;
    }

    private void Update()
    {
        if (gm.rockList.Count != 0)
        {
            rock = gm.rockList[gm.rockCurrent].rock;
        }
    }

    public void DrawTrajectory()
    {
        hogLinePoint = new Vector3(hogLinePointGO.transform.position.x, -15.75f, 0f);
        curlPoint = curlPointGO.transform.position;
        targetPoint = targetPointGO.transform.position;

        //List<Vector3> pos = new List<Vector3>();
        //List<Vector2> pos2D = new List<Vector2>();

        lr.positionCount = 100;
        float t = 0f;
        Vector3 B = new Vector3(0, -25, 0);

        lr.SetPosition(0, launcher.transform.position);

        for (int i = 1; i < lr.positionCount; i++)
        {
            B = ((1 - t) * (1 - t) * hogLinePoint) + (2 * (1 - t) * t * curlPoint) + (t * t * targetPoint);
            //pos.Add(B);
            //Vector2 B2 = new Vector2(B.x, B.y);
            lr.SetPosition(i, B);
            //edgeCol.points[i] = new Vector2(B.x, B.y);
            //pos2D.Add(B2);

            //if (trajCollision)
            //{
            //    break;
            //}

            t += (1 / (float)lr.positionCount);
        }

        //lr.SetPositions(lr.GetPositions());
        //DrawQuadraticBezierCurve(hogLinePoint, curlPoint, targetPoint);
        //edgeCol.SetPoints(pos2D);
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