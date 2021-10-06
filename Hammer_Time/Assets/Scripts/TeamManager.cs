using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public GameManager gm;
    public SweeperManager sm;
    public GameSettingsPersist gsp;

    public Color teamRedColour;
    public TeamMember[] teamRed;
    public Color teamYellowColour;
    public TeamMember[] teamYellow;


    public GameObject[] leadGO;
    public GameObject[] secondGO;
    public GameObject[] thirdGO;
    public GameObject[] skipGO;

    // Start is called before the first frame update
    void Start()
    {
        gsp = FindObjectOfType<GameSettingsPersist>();
        if (gsp.tourny)
        {
            teamRedColour = gsp.redTeamColour;
            teamYellowColour = gsp.yellowTeamColour;
        }
        else
        {
            Shuffle(teamRed);
            Shuffle(teamYellow);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.rockList.Count != 0)
        {
            #region Target
            //if (gm.rockCurrent <= 11)
            //{
            //    if (gm.redHammer && gm.rockList[gm.rockCurrent].rockInfo.teamName == gm.rockList[1].rockInfo.teamName)
            //    {
            //        if (!gm.aiTeamRed)
            //            gm.target = true;
            //        else
            //            gm.target = false;
            //    }
            //    else if (gm.redHammer && gm.rockList[gm.rockCurrent].rockInfo.teamName == gm.rockList[0].rockInfo.teamName)
            //    {
            //        if (!gm.aiTeamYellow)
            //            gm.target = true;
            //        else
            //            gm.target = false;
            //    }
            //    else if (!gm.redHammer && gm.rockList[gm.rockCurrent].rockInfo.teamName == gm.rockList[0].rockInfo.teamName)
            //    {
            //        if (!gm.aiTeamRed)
            //            gm.target = true;
            //        else
            //            gm.target = false;
            //    }
            //    else if (!gm.redHammer && gm.rockList[gm.rockCurrent].rockInfo.teamName == gm.rockList[1].rockInfo.teamName)
            //    {
            //        if (!gm.aiTeamYellow)
            //            gm.target = true;
            //        else
            //            gm.target = false;
            //    }
            //    else
            //        gm.target = false;
            //}
            //else
            //    gm.target = false;

            //if (gm.rockCurrent <= 3)
            //{
            //    gm.shooterAnimRed = leadGO[0];
            //    gm.shooterAnimYellow = leadGO[1];

            //}
            //else if (gm.rockCurrent <= 7)
            //{
            //    gm.shooterAnimRed = secondGO[0];
            //    gm.shooterAnimYellow = secondGO[1];
            //}
            //else if (gm.rockCurrent <= 11)
            //{
            //    gm.shooterAnimRed = thirdGO[0];
            //    gm.shooterAnimYellow = thirdGO[1];
            //}
            //else
            //{
            //    gm.shooterAnimRed = skipGO[0];
            //    gm.shooterAnimYellow = skipGO[1];
            //}
            #endregion
        }


    }

    void Shuffle(TeamMember[] a)
    {
        // Loops through array
        for (int i = a.Length - 1; i > 0; i--)
        {
            // Randomize a number between 0 and i (so that the range decreases each time)
            int rnd = Random.Range(0, i);

            // Save the value of the current i, otherwise it'll overright when we swap the values
            TeamMember temp = a[i];

            // Swap the new and old values
            a[i] = a[rnd];
            a[rnd] = temp;
        }

        // Print
        //PrintRows(a);
        //for (int i = 0; i < a.Length; i++)
        //{
        //	Print;
        //}
    }

    public void SetCharacter(int rockCurrent, bool redTurn)
    {
        #region Target
        //if (!gm.gsp.story)
        //{
        //    if (gm.gsp.third)
        //    {
        //        if (rockCurrent <= 11 && rockCurrent >= 8)
        //        {
        //            gm.target = false;
        //        }
        //        else
        //        {
        //            if (redTurn)
        //            {
        //                if (!gm.aiTeamRed)
        //                    gm.target = true;
        //                else
        //                    gm.target = false;
        //            }
        //            else if (!redTurn)
        //            {
        //                if (!gm.aiTeamYellow)
        //                    gm.target = true;
        //                else
        //                    gm.target = false;
        //            }
        //            else
        //                gm.target = false;
        //        }
        //    }
        //    else if (gm.gsp.skip)
        //    {
        //        if (rockCurrent <= 11)
        //        {
        //            if (redTurn)
        //            {
        //                if (!gm.aiTeamRed)
        //                    gm.target = true;
        //                else
        //                    gm.target = false;
        //            }
        //            else if (!redTurn)
        //            {
        //                if (!gm.aiTeamYellow)
        //                    gm.target = true;
        //                else
        //                    gm.target = false;
        //            }
        //            else
        //                gm.target = false;
        //        }
        //        else
        //            gm.target = false;
        //    }
        //    else
        //        gm.target = false;

        //}
        //else 
        //{
        //    if (gm.gsp.third)
        //    {
        //        if (rockCurrent <= 11)
        //        {
        //            if (rockCurrent >= 8)
        //            {
        //                if (redTurn)
        //                {
        //                    if (!gm.aiTeamRed)
        //                        gm.target = true;
        //                    else
        //                        gm.target = false;
        //                }
        //                else if (!redTurn)
        //                {
        //                    if (!gm.aiTeamYellow)
        //                        gm.target = true;
        //                    else
        //                        gm.target = false;
        //                }
        //                else
        //                    gm.target = false;
        //            }
        //            else
        //                gm.target = false;
        //        }
        //        else
        //            gm.target = false;
        //    }
        //    else if (gm.gsp.skip)
        //    {
        //        if (rockCurrent <= 11)
        //        {
        //            if (redTurn)
        //            {
        //                if (!gm.aiTeamRed)
        //                    gm.target = true;
        //                else
        //                    gm.target = false;
        //            }
        //            else if (!redTurn)
        //            {
        //                if (!gm.aiTeamYellow)
        //                    gm.target = true;
        //                else
        //                    gm.target = false;
        //            }
        //            else
        //                gm.target = false;
        //        }
        //        else
        //            gm.target = false;
        //    }
        //    else
        //        gm.target = false;
        //}
        #endregion

        if (redTurn)
        {
            for (int i = 0; i < teamRed.Length; i++)
            {
                teamRed[i].shooter.GetComponent<CharColourChanger>().TeamColour(teamRedColour);

                teamRed[i].sweeperL.GetComponent<CharColourChanger>().TeamColour(teamRedColour);
                teamRed[i].sweeperR.GetComponent<CharColourChanger>().TeamColour(teamRedColour);
            }
        }
        else
        {
            for (int i = 0; i < teamYellow.Length; i++)
            {
                teamYellow[i].shooter.GetComponent<CharColourChanger>().TeamColour(teamYellowColour);

                teamYellow[i].sweeperL.GetComponent<CharColourChanger>().TeamColour(teamYellowColour);
                teamYellow[i].sweeperR.GetComponent<CharColourChanger>().TeamColour(teamYellowColour);
            }
        }
        if (rockCurrent < 4)
        {
            sm.sweeperRedL = teamRed[1].sweeperL;
            sm.sweeperRedR = teamRed[2].sweeperR;
            sm.sweeperYellowL = teamYellow[1].sweeperL;
            sm.sweeperYellowR = teamYellow[2].sweeperR;
            gm.shooterAnimRed = teamRed[0].shooter;
            gm.shooterAnimYellow = teamYellow[0].shooter;
        }
        else if (rockCurrent < 8)
        {
            sm.sweeperRedL = teamRed[0].sweeperL;
            sm.sweeperRedR = teamRed[2].sweeperR;
            sm.sweeperYellowL = teamYellow[0].sweeperL;
            sm.sweeperYellowR = teamYellow[2].sweeperR;
            gm.shooterAnimRed = teamRed[1].shooter;
            gm.shooterAnimYellow = teamYellow[1].shooter;
        }
        else if (rockCurrent < 12)
        {
            sm.sweeperRedL = teamRed[0].sweeperL;
            sm.sweeperRedR = teamRed[1].sweeperR;
            sm.sweeperYellowL = teamYellow[0].sweeperL;
            sm.sweeperYellowR = teamYellow[1].sweeperR;
            gm.shooterAnimRed = teamRed[2].shooter;
            gm.shooterAnimYellow = teamYellow[2].shooter;
        }
        else
        {
            sm.sweeperRedL = teamRed[0].sweeperL;
            sm.sweeperRedR = teamRed[1].sweeperR;
            sm.sweeperYellowL = teamYellow[0].sweeperL;
            sm.sweeperYellowR = teamYellow[1].sweeperR;
            gm.shooterAnimRed = teamRed[3].shooter;
            gm.shooterAnimYellow = teamYellow[3].shooter;
        }
    }
}
