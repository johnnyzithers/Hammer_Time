using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public enum GameState { START, DRAWTOBUTTON, REDTURN, YELLOWTURN, CHECKSCORE, SCORE, RESET, END, DEBUG }


public class GameManager : MonoBehaviour
{
    public AudioManager am;

    public int endCurrent;
    public int endTotal;
    public bool redHammer;
    public int rocksPerTeam;
    public int rockTotal;
    public int rockCurrent;

    public RockManager rm;
    public GameObject redShooter;
    public GameObject yellowShooter;
    public GameObject shooterAnimRed;
    public GameObject shooterAnimYellow;

    public SweeperManager sm;

    GameObject shooterGO;
    public Transform launcher;
    public Transform yellowRocksInactive;
    public Transform redRocksInactive;
    public Collider2D boardCollider;
    public Collider2D launchCollider;

    public GameObject yellowSpin;
    public GameObject redSpin;

    int redRocks_left;
    int yellowRocks_left;
    public int redScore;
    public int yellowScore;

    public GameHUD gHUD;
    public RockBar rockBar;

    public bool aiTeamRed;
    public bool aiTeamYellow;

    public GameObject debug;
    public TutorialManager tm;
    public AIManager aim;
    public TutorialHUD tHUD;
    public bool tutorial;

    public GameState state;

    Rock_Info redRock;
    Rock_Info yellowRock;

    public Button redButton;
    public Button yellowButton;
    public Button sweepButton;
    public GameObject db;
    public GameObject dbrandom;

    public CameraManager cm;
    public GameObject vcam_go;
    public CinemachineVirtualCamera vcam;
    Transform tFollowTarget;

    public ShootingKnob knob;
    public List<Rock_List> rockList;
    public List<House_List> houseList;
    public List<FreeGuard_List> fgList;

    public Transform[] fgPosList;
    void Start()
    {
        state = GameState.START;

        //sweepButton.gameObject.SetActive(false);

        am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        StartCoroutine(SetupGame());
    }

    IEnumerator SetupGame()
    {
        //gHUD.SetHUD(redRock);
        Debug.Log("Game Start");

        endCurrent = 1;
        rockCurrent = 0;
        redHammer = FindObjectOfType<GameSettingsPersist>().redHammer;
        endTotal = FindObjectOfType<GameSettingsPersist>().ends;
        rocksPerTeam = FindObjectOfType<GameSettingsPersist>().rocks;

        Debug.Log("redHammer is " + redHammer);
        am.Play("Theme");

        redRocks_left = rocksPerTeam;
        yellowRocks_left = rocksPerTeam;
        rockTotal = rocksPerTeam * 2;

        rockList = new List<Rock_List>();
        houseList = new List<House_List>();
        fgList = new List<FreeGuard_List>();

        //yield return new WaitForSeconds(2f);
        yield return new WaitForEndOfFrame();

        gHUD.SetHammer(redHammer);
        if (!tutorial)
        {
            if (redHammer)
            {
                SetHammerRed();
            }
            else
            {
                SetHammerYellow();
            }
        }
        else if (tutorial)
        {
            StartCoroutine(Tutorial());
        }
        
    }

    public void SetHammerRed()
    {
        db.SetActive(false);

        StartCoroutine(SetupRocks());
        OnYellowTurn();
    }

    public void SetHammerYellow()
    {
        db.SetActive(false);

        StartCoroutine(SetupRocks());
        OnRedTurn();
    }

    IEnumerator SetupRocks()
    {
        int hammer;
        int notHammer;

        if (redHammer)
        {
            hammer = 0;
            notHammer = 1;
        }
        else
        {
            hammer = 1;
            notHammer = 0;
        }

        for (int i = 1; i <= rockTotal; i++)
        {
            if (i % 2 == notHammer)
            {
                GameObject yellowRock_go = Instantiate(yellowShooter, yellowRocksInactive);

                float yRocks = rocksPerTeam * 0.5f;
                int k = (i / 2) + notHammer;

                if (k <= yRocks)
                {
                    yellowRock_go.transform.position = new Vector2(yellowRock_go.transform.position.x, yellowRock_go.transform.position.y - ((k - 1) * 0.4f));
                }
                else if (k > yRocks)
                {
                    float j = k - yRocks;
                    yellowRock_go.transform.position = new Vector2(yellowRock_go.transform.position.x - 0.4f, yellowRock_go.transform.position.y - ((j - 1) * 0.4f));
                }

                Rock_Info yellowRock_info = yellowRock_go.GetComponent<Rock_Info>();
                yellowRock_info.rockNumber = k;
                yellowRock_go.name = yellowRock_info.teamName + " " + yellowRock_info.rockNumber;
                yellowRock_go.GetComponent<Rock_Flick>().enabled = false;
                yellowRock_go.GetComponent<Rock_Release>().enabled = false;
                yellowRock_go.GetComponent<Rock_Force>().enabled = false;
                yellowRock_go.GetComponent<CircleCollider2D>().enabled = false;
                rockList.Add(new Rock_List(yellowRock_go, yellowRock_info));
                yield return new WaitForSeconds(0.025f);
            }
            if (i % 2 == hammer)
            {
                GameObject redRock_go = Instantiate(redShooter, redRocksInactive);
                float yRocks = rocksPerTeam / 2f;
                int k = (i / 2) + hammer;
                if (k <= yRocks)
                {
                    redRock_go.transform.position = new Vector2(redRock_go.transform.position.x, redRock_go.transform.position.y - ((k - 1) * 0.4f));
                }
                else if (k > yRocks)
                {
                    float j = k - yRocks;
                    redRock_go.transform.position = new Vector2(redRock_go.transform.position.x + 0.4f, redRock_go.transform.position.y - ((j - 1) * 0.4f));
                }


                Rock_Info redRock_info = redRock_go.GetComponent<Rock_Info>();
                redRock_info.rockNumber = k;
                redRock_go.name = redRock_info.teamName + " " + redRock_info.rockNumber;
                redRock_go.GetComponent<CircleCollider2D>().enabled = false;
                redRock_go.GetComponent<Rock_Flick>().enabled = false;
                redRock_go.GetComponent<Rock_Release>().enabled = false;
                redRock_go.GetComponent<Rock_Force>().enabled = false;
                rockList.Add(new Rock_List(redRock_go, redRock_info));
                yield return new WaitForSeconds(0.025f);
            }
            //rockList.Sort();
        }

        rockBar.ResetBar(redHammer);
        //scoreboard.SetActive(false);

    }

    IEnumerator ResetGame()
    {
        houseList.Clear();

        if (rockList.Count != 0)
        {
            foreach (Rock_List rock in rockList)
            {
                Destroy(rock.rock);
            }
        }

        yield return new WaitForFixedUpdate();

        rockList.Clear();

        endCurrent++;
        redRocks_left = rocksPerTeam;
        yellowRocks_left = rocksPerTeam;
        rockCurrent = 0;

        gHUD.SetHammer(redHammer);

        if (redHammer)
        {
            StartCoroutine(SetupRocks());
            OnYellowTurn();
        }
        else
        {
            StartCoroutine(SetupRocks());
            OnRedTurn();
        }
    }

    public void OnRedTurn()
    {
        state = GameState.REDTURN;

        cm.ShotSetup();

        if (GameObject.FindGameObjectsWithTag("Player").Length >= 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
        }

        if (rockCurrent < 6)
        {
            foreach (Rock_List rock in rockList)
            {
                if (rockList.IndexOf(rock) < 6)
                {
                    fgList.Add(new FreeGuard_List(rockList.IndexOf(rock), rock.rockInfo.freeGuard, rock.rock.transform));
                }
            }
        }

        Debug.Log("Red Turn");
        shooterGO = Instantiate(shooterAnimRed);
        sm.SetupSweepers();

        GameObject redRock_1 = rockList[rockCurrent].rock;
        redRock_1.GetComponent<Rock_Flick>().enabled = true;
        redRock_1.GetComponent<Rock_Release>().enabled = true;
        redRock_1.GetComponent<Rock_Force>().enabled = true;
        redRock_1.GetComponent<Rock_Colliders>().enabled = true;
        boardCollider.enabled = false;
        launchCollider.enabled = false;

        StartCoroutine(RedTurn());
    }

    IEnumerator RedTurn()
    {

        redRocks_left--;
        rm.inturn = false;

        GameObject redRock_1 = rockList[rockCurrent].rock;

        redRock = redRock_1.GetComponent<Rock_Info>();
        Debug.Log(redRock_1.name);

        Debug.Log("Current Rock is " + rockCurrent);
        rockBar.ActiveRock(true);

        yield return new WaitUntil(() => redRock.shotTaken == true);

        am.Play("RockScrape");
        cm.RockFollow(redRock_1.transform);
        boardCollider.enabled = true;

        rockBar.ActiveRock(true);

        yield return new WaitUntil(() => redRock.released == true);

        redRock_1.GetComponent<Rock_Flick>().enabled = false;
        sm.Release(redRock_1, aiTeamRed);
        rm.GetComponent<Sweep>().EnterSweepZone();

        yield return new WaitUntil(() => redRock.rest == true);

        am.Stop("RockScrape");
        rm.GetComponent<Sweep>().ExitSweepZone();

        Debug.Log("redRock at Rest");
        vcam.enabled = false;

        StartCoroutine(AllStopped());

        if (!redRock.outOfPlay)
        {
            if (redRock.inPlay && !redRock.inHouse)
            {
                if (redRock_1.transform.position.y >= 1.5f)
                {
                    redRock.freeGuard = true;
                }
            }

            Debug.Log("Out Of Play is " + redRock.outOfPlay);
            Debug.Log("Rock Current is " + rockCurrent);
            rockBar.IdleRock(rockCurrent);
        }
        else
        {
            Debug.Log("Out Of Play is " + redRock.outOfPlay);
            Debug.Log("Rock Current is " + rockCurrent);
            rockBar.DeadRock(rockCurrent);

        }

        //foreach (Rock_List rock in rockList)
        //{
        //    bool outOfPlay;
        //    int rockIndex;
        //    rockIndex = rockList.IndexOf(rock);
        //    outOfPlay = rock.rockInfo.outOfPlay;
        //    rockBar.ShotUpdate(rockIndex, outOfPlay);
        //}

        StartCoroutine(CheckScore());
    }

    public void OnYellowTurn()
    {
        state = GameState.REDTURN;

        if (GameObject.FindGameObjectsWithTag("Player").Length >= 1)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
        }

        if (rockCurrent < 6)
        {
            foreach (Rock_List rock in rockList)
            {
                if (rockList.IndexOf(rock) < 6)
                {
                    fgList.Add(new FreeGuard_List(rockList.IndexOf(rock), rock.rockInfo.freeGuard, rock.rock.transform));
                }
            }
        }

        shooterGO = Instantiate(shooterAnimYellow);

        GameObject yellowRock_1 = rockList[rockCurrent].rock;
        yellowRock_1.GetComponent<Rock_Flick>().enabled = true;
        yellowRock_1.GetComponent<Rock_Release>().enabled = true;
        yellowRock_1.GetComponent<Rock_Force>().enabled = true;
        yellowRock_1.GetComponent<Rock_Colliders>().enabled = true;
        boardCollider.enabled = false;
        launchCollider.enabled = false;

        sm.SetupSweepers();

        Debug.Log("Yellow Turn");


        //vcam = vcam_go.GetComponent<CinemachineVirtualCamera>();

        //tFollowTarget = launcher.transform;
        //vcam.LookAt = tFollowTarget;
        //vcam.Follow = tFollowTarget;
        //vcam.enabled = true;

        

        StartCoroutine(YellowTurn());
    }

    IEnumerator YellowTurn()
    {
        yellowRocks_left--;

        cm.ShotSetup();


        GameObject yellowRock_1 = rockList[rockCurrent].rock;
        //if (tutorial && !tm.rocksPlaced)
        //{
        //    yellowRock_1.GetComponent<Rock_Release>().enabled = false;
        //    yellowRock_1.GetComponent<Rock_Force>().enabled = false;
        //    yellowRock_1.GetComponent<Rock_Colliders>().enabled = false;
        //}
        //else
        //{
        //    yellowRock_1.GetComponent<Rock_Release>().enabled = true;
        //    yellowRock_1.GetComponent<Rock_Force>().enabled = true;
        //    yellowRock_1.GetComponent<Rock_Colliders>().enabled = true;
        //}

        rm.inturn = false;
        //Debug.Log("rmInturn is " + rm.inturn);

        yellowRock = yellowRock_1.GetComponent<Rock_Info>();
        Debug.Log(yellowRock_1.name);

        rockBar.ActiveRock(false);

        if (aiTeamYellow)
        {
            gHUD.mainDisplay.enabled = true;
            gHUD.mainDisplay.text = yellowRock.teamName + " Turn";
            yield return new WaitForSeconds(1f);
            aim.OnShot(rockCurrent);

        }

        yield return new WaitUntil(() => yellowRock.shotTaken == true);

        cm.RockFollow(yellowRock_1.transform);
        //am.Play("RockScrape")
        boardCollider.enabled = true;
        rockBar.ActiveRock(false);

        yield return new WaitUntil(() => yellowRock.released == true);

        sm.Release(yellowRock_1, aiTeamYellow);
        //sweeper.AttachToRock(yellowRock_1);
        //rm.GetComponent<Sweep>().EnterSweepZone();

        yield return new WaitUntil(() => yellowRock.rest == true);
        am.Stop("RockScrape");
        
        rm.GetComponent<Sweep>().ExitSweepZone();

        vcam.enabled = false;

        yield return new WaitForEndOfFrame();

        if (rockCurrent < 6)
        {
            if (yellowRock.inPlay && !yellowRock.inHouse)
            {
                if (yellowRock_1.transform.position.y >= 1.5f)
                {
                    yellowRock.freeGuard = true;
                }
            }
        }

        StartCoroutine(CheckScore());

    }

    IEnumerator AllStopped()
    {
        foreach (Rock_List rock in rockList)
        {
            Rigidbody2D rockRB = rock.rock.GetComponent<Rigidbody2D>();
            if (Mathf.Abs(rockRB.velocity.y) > 0.01f && Mathf.Abs(rockRB.velocity.x) > 0.01f)
            {
                Debug.Log(rock.rockInfo.teamName + " " + rock.rockInfo.rockNumber + " is still moving :(");
                yield return new WaitUntil(() => Mathf.Abs(rockRB.velocity.y) < 0.01f && Mathf.Abs(rockRB.velocity.x) < 0.01f);
            }
        }
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator CheckScore()
    {
        state = GameState.CHECKSCORE;


        Debug.Log("Check Score");

        yield return StartCoroutine(AllStopped());


        Debug.Log("All Stopped");
        yield return new WaitForFixedUpdate();

        rockBar.ShotUpdate(rockCurrent, rockList[rockCurrent].rockInfo.outOfPlay);

        Debug.Log("Current Rock is " + rockCurrent);
        houseList.Clear();

        Destroy(shooterGO);

        foreach (Rock_List rock in rockList)
        {
            if (rock.rockInfo.inHouse == true)
            {
                houseList.Add(new House_List(rock.rock, rock.rockInfo));
            } 
        }

        yield return new WaitForFixedUpdate();

        houseList.Sort();
        // if the we have shot all the rocks, go to the final scoring
        if (rockList.Count == rockCurrent + 1)
        {
            Debug.Log("Rock List " + rockList.Count + " equals " + "Current Rock " + (rockCurrent + 1));
            StartCoroutine(Scoring());
        }
        // or else we will just check the score
        else
        {
            if (houseList.Count != 0)
            {
                foreach (House_List rock in houseList)
                {
                    Debug.Log(rock.rockInfo.name + " - " + rock.rockInfo.distance);
                }
            }

            yield return new WaitForFixedUpdate();

            int houseRock = houseList.Count;

            // if the list isnt empty
            if (houseList.Count != 0)
            {
                int houseScore = 0;
                string winningTeamName = houseList[0].rockInfo.teamName;
                bool stopCounting = false;

                // lets loop the list
                for (int i = 0; i < houseList.Count; i++)
                {
                    if (!stopCounting)
                    {
                        // lets only count until the team changes
                        if (houseList[i].rockInfo.teamName == winningTeamName)
                        {
                            houseScore++;
                        }
                        else if(houseList[i].rockInfo.teamName != winningTeamName)
                        {
                            stopCounting = true;
                        }
                    }
                }
                bool noRocks = false;

                //send the info to the ui
                gHUD.CheckScore(noRocks, winningTeamName, houseScore);

            }
            // if the house is empty move along to next turn
            else if (houseList.Count == 0)
            {
                bool noRocks = true;
                gHUD.CheckScore(noRocks, " ", 0);
            }

            yield return StartCoroutine(WaitForClick());
            gHUD.MainDisplayOff();
            gHUD.ScoreboardOff();

            Debug.Log("Current Rock is " + rockCurrent);
            NextTurn();
        }
    }

    public void NextTurn()
    {
        Debug.Log("Next Turn");

        rockCurrent++;
        Debug.Log("Current Rock is " + rockCurrent);

        if (rockCurrent % 2 == 1)
        {
            if (redHammer)
            {
                OnRedTurn();
            }
            else
            {
                OnYellowTurn();
            }
        }
        else if (rockCurrent % 2 == 0)
        {
            if (redHammer)
            {
                OnYellowTurn();
            }
            else
            {
                OnRedTurn();
            }
        }
    }

    IEnumerator Scoring()
    {
        Debug.Log("Current rock is " + rockCurrent);

        state = GameState.SCORE;
        Debug.Log("Scoring");

        if (redHammer)
        {
            //Debug.Log("red hammer");
            redRock = rockList[1].rockInfo;
            yellowRock = rockList[0].rockInfo;
        }
        else
        {
            //Debug.Log("not red hammer");
            redRock = rockList[0].rockInfo;
            yellowRock = rockList[1].rockInfo;
        }

        if (houseList.Count != 0)
        {
            Debug.Log("Rocks in house");
            houseList.Clear();

            foreach (Rock_List rock in rockList)
            {
                if (rock.rockInfo.inHouse)
                {
                    houseList.Add(new House_List(rock.rock, rock.rockInfo));
                }
            }

            yield return new WaitForFixedUpdate();

            houseList.Sort();

            yield return new WaitForFixedUpdate();
            int houseRock = houseList.Count;

            int houseScore = 0;
            string winningTeamName = houseList[0].rockInfo.teamName;
            bool stopCounting = false;

            // lets loop the list
            for (int i = 0; i < houseList.Count; i++)
            {
                if (!stopCounting)
                {
                    // lets only count until the team changes
                    if (houseList[i].rockInfo.teamName == winningTeamName)
                    {
                        houseScore++;
                    }
                    else if (houseList[i].rockInfo.teamName != winningTeamName)
                    {
                        stopCounting = true;
                    }
                }
            }

            if (redHammer)
            {
                gHUD.ScoringUI(redRock.teamName, winningTeamName, houseScore);
            }
            else
            {
                gHUD.ScoringUI(yellowRock.teamName, winningTeamName, houseScore);
            }

            if (winningTeamName == redRock.teamName)
            {
                redScore = redScore + houseScore;
                gHUD.Scoreboard(endCurrent, redScore, 0);
                redHammer = false;
                gHUD.SetHammer(redHammer);
            }
            else if (winningTeamName == yellowRock.teamName)
            {
                yellowScore = yellowScore + houseScore;
                gHUD.Scoreboard(endCurrent, 0, yellowScore);
                redHammer = true;
                gHUD.SetHammer(redHammer);
            }

            yield return new WaitForSeconds(1.5f);
            
            
        }
        else if (houseList.Count == 0)
        {
            if (redHammer)
            {
                redHammer = true;
                gHUD.Scoreboard(endCurrent, 0, 0);
                gHUD.ScoringUI(redRock.teamName, " ", 0);
                gHUD.SetHammer(redHammer);
            }
            else
            {
                redHammer = false;
                gHUD.Scoreboard(endCurrent, 0, 0);
                gHUD.ScoringUI(yellowRock.teamName, " ", 0);
                gHUD.SetHammer(redHammer);
            }
        }

        rockBar.EndUpdate(yellowScore, redScore);

        yield return StartCoroutine(WaitForClick());


        if (endCurrent < endTotal)
        {
            gHUD.ScoreboardOff();
            gHUD.MainDisplayOff();
            state = GameState.RESET;
            StartCoroutine(ResetGame());
        }
        else if (endCurrent >= endTotal)
        {
            state = GameState.END;
            StartCoroutine(EndOfGame());
        }
    }

    IEnumerator WaitForClick()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        Debug.Log("Clickeddd");
    }

    IEnumerator EndOfGame()
    {
        gHUD.EndOfGame(redScore, redRock.teamName, yellowScore, yellowRock.teamName);

        yield return new WaitForSeconds(2f);
        if (redScore == yellowScore)
        {
            Debug.Log("Game is tied");
            yield return StartCoroutine(WaitForClick());
            gHUD.ScoreboardOff();
            gHUD.MainDisplayOff();
            StartCoroutine(ResetGame());
        }
        else
        {
            Debug.Log("Game is over");
            yield return StartCoroutine(WaitForClick());
            SceneManager.LoadScene("SplashMenu");
        }
    }

    public void OnDebug()
    {
        db.SetActive(false);

        GetComponent<Debug_Placement>().enabled = true;
        GetComponent<Debug_Shooting>().enabled = true;
        GetComponent<Debug_Random>().enabled = true;

        dbrandom.SetActive(true);
    }

    public void OnDebugReset()
    {
        StartCoroutine(Scoring());
    }

    IEnumerator Tutorial()
    {
        redHammer = true;
        endTotal = 10;
        endCurrent = 10;
        aiTeamYellow = true;
        gHUD.Scoreboard(2, 3, 0);
        gHUD.Scoreboard(4, 0, 1);
        gHUD.Scoreboard(5, 2, 0);
        gHUD.Scoreboard(6, 0, 3);
        gHUD.Scoreboard(7, 0, 1);
        gHUD.Scoreboard(8, 1, 0);
        gHUD.Scoreboard(9, 0, 1);

        rockBar.EndUpdate(6, 6);
        redScore = 6;
        yellowScore = 6;

        yield return StartCoroutine(SetupRocks());

        tm.enabled = true;

        yield return new WaitUntil(() => tm.rocksPlaced == true);

        rockCurrent = 11;

        rockBar.ShotUpdate(rockCurrent, true);

        yield return StartCoroutine(CheckScore());
    }

}
