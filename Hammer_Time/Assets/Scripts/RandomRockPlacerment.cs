using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRockPlacerment : MonoBehaviour
{
    public GameManager gm;
    public RockManager rm;

    public bool placed;
    int rockCurrent;

    public Vector2[] placePos;
    public Vector2[] rockPos;
    

    // Start is called before the first frame update
    void Start()
    {
        placed = false;
    }

    // Update is called once per frame
    public void OnRockPlace(int rockCrnt)
    {
        placed = false;
        rockCurrent = rockCrnt;
        rockPos = new Vector2[rockCurrent];

        StartCoroutine(RandomRockPlace());
    }

    IEnumerator RandomRockPlace()
    {
        int houseCount = 0;
        int houseRed = 0;
        bool[] guardCount = new bool[9];
        for (int i = 0; i < 9; i++)
            guardCount[i] = false;
        for (int i = 0; i < rockCurrent; i++)
        {
            int placeSelector;
            int shotSelector;
            if (i < 4 && Random.Range(0f, 1f) < 0.5f)
                shotSelector = Random.Range(1, 4);
            else
                shotSelector = Random.Range(0, 4);

            switch (shotSelector)
            {
                case 0:
                    placeSelector = 9;
                    if (houseCount > 4)
                    {
                        placeSelector = 10;
                        rockPos[i] = placePos[placeSelector];
                    }
                    else if (houseRed > 2)
                    {
                        if (gm.redHammer && i % 2 != 0)
                        {
                            rockPos[i] = placePos[placeSelector] + (Random.insideUnitCircle * 1.25f);
                        }
                        else if (!gm.redHammer && i % 2 != 1)
                            rockPos[i] = placePos[placeSelector] + (Random.insideUnitCircle * 1.25f);
                        else
                        {
                            placeSelector = 10;
                            rockPos[i] = placePos[placeSelector];
                        }
                    }

                    else if (houseCount - houseRed > 2)
                    {
                        if (gm.redHammer && i % 2 != 1)
                        {
                            houseRed++;
                            rockPos[i] = placePos[placeSelector] + (Random.insideUnitCircle * 1.25f);
                        }
                        else if (!gm.redHammer && i % 2 != 0)
                        {
                            houseRed++;
                            rockPos[i] = placePos[placeSelector] + (Random.insideUnitCircle * 1.25f);
                        }
                        else
                        {
                            placeSelector = 10;
                            rockPos[i] = placePos[placeSelector];
                        }
                    }
                    else
                    {
                        houseCount++;
                        rockPos[i] = placePos[placeSelector] + (Random.insideUnitCircle * 1.25f);
                    }
                    Debug.Log("case 0 rockPos is - " + rockPos[i].x + ", " + rockPos[i].y);
                    break;
                case 1:
                    placeSelector = 10;
                    rockPos[i] = placePos[placeSelector];
                    break;
                case 2:
                    placeSelector = 10;
                    rockPos[i] = placePos[placeSelector];
                    break;
                case 3:
                    if (i % 2 != 0)
                    {
                        placeSelector = Random.Range(0, 6);
                        if (guardCount[placeSelector])
                        {
                            placeSelector = Random.Range(0, 6);
                            if (guardCount[placeSelector])
                            {
                                placeSelector = 10;
                                rockPos[i] = placePos[placeSelector];
                            }
                            else
                            {
                                guardCount[placeSelector] = true;
                                rockPos[i] = placePos[placeSelector];
                            }
                        }
                        else
                        {
                            guardCount[placeSelector] = true;
                            rockPos[i] = placePos[placeSelector];
                        }

                    }
                    else
                    {
                        placeSelector = Random.Range(6, 9);
                        if (guardCount[placeSelector])
                        {
                            placeSelector = Random.Range(6, 9);
                            if (guardCount[placeSelector])
                            {
                                placeSelector = 10;
                                rockPos[i] = placePos[placeSelector];
                            }
                            else
                            {
                                guardCount[placeSelector] = true;
                                rockPos[i] = placePos[placeSelector];
                            }
                        }
                        else
                        {
                            guardCount[placeSelector] = true;
                            rockPos[i] = placePos[placeSelector];
                        }
                    }
                    rockPos[i] += Random.insideUnitCircle * 0.35f;
                    break;
            }
        }
        for (int i = 0; i < rockCurrent; i++)
        {
            gm.rockList[i].rockInfo.placed = true;
        }

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < rockCurrent; i++)
        {
            gm.rockList[i].rock.GetComponent<CircleCollider2D>().radius = 0.14f;
            gm.rockList[i].rock.GetComponent<SpriteRenderer>().enabled = false;
            gm.rockList[i].rock.GetComponent<SpringJoint2D>().enabled = false;
            gm.rockList[i].rock.GetComponent<Rock_Flick>().enabled = false;
            gm.rockList[i].rock.transform.parent = null;
            //rm.rb.DeadRock(i);
            yield return new WaitForEndOfFrame();
            Debug.Log("Rock Position " + i + " " + rockPos[i].x + ", " + rockPos[i].y);
            gm.rockList[i].rock.GetComponent<Rigidbody2D>().position = rockPos[i];

            gm.rockList[i].rock.GetComponent<CircleCollider2D>().enabled = true;
            gm.rockList[i].rock.GetComponent<Rock_Release>().enabled = true;
            gm.rockList[i].rock.GetComponent<Rock_Force>().enabled = true;
            gm.rockList[i].rock.GetComponent<Rock_Colliders>().enabled = true;
            yield return new WaitForEndOfFrame();
            if (rockPos[i].y > 8f)
            {
                gm.rockList[i].rockInfo.inPlay = false;
                gm.rockList[i].rockInfo.outOfPlay = true;
                gm.rockList[i].rock.SetActive(false);
            }
            else
            {
                gm.rockList[i].rock.GetComponent<SpriteRenderer>().enabled = true;
                gm.rockList[i].rockInfo.inPlay = true;
                gm.rockList[i].rockInfo.outOfPlay = false;
            }
            gm.rockList[i].rockInfo.moving = false;
            gm.rockList[i].rockInfo.shotTaken = true;
            gm.rockList[i].rockInfo.released = true;
            gm.rockList[i].rockInfo.stopped = true;
            gm.rockList[i].rockInfo.rest = true;
            Debug.Log("i is equal to " + i);

            //rm.rb.ShotUpdate(rockCurrent, gm.rockList[i].rockInfo.outOfPlay);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        //rocksPlaced = true;
        gm.rockCurrent = rockCurrent - 1;
        gm.rockTotal = 16;
        yield return new WaitForEndOfFrame();
        placed = true;
    }
}
