using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweeperManager : MonoBehaviour
{

    public Sweeper sweeperL;
    public Sweeper sweeperR;

    public Sweeper sweeperRedL;
    public Sweeper sweeperRedR;
    public Sweeper sweeperYellowL;
    public Sweeper sweeperYellowR;

    public CharacterStats swprLStats;
    public CharacterStats swprRStats;

    public Sweep sweep;

    public SweeperSelector sweepSel;
    
    public GameObject sweepButton;
    public GameObject hardButton;
    public GameObject whoaButton;
    public RockManager rm;
    public AudioManager am;
    public bool inturn;

    private void Awake()
    {
        am = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        if (am == null)
        {
            Debug.Log("Audio Manager not loaded");
        }
    }

    private void Update()
    {
        if (sweeperR.gameObject.activeSelf & sweeperL.gameObject.activeSelf)
        {
            if (sweeperR.gameObject.activeSelf && sweeperR.gameObject.GetComponent<CharacterStats>().sweepHealth <= 0f)
            {
                if (rm.gm.aiTeamRed)
                    SweepWhoa(rm.gm.aiTeamRed);
                else if (rm.gm.aiTeamYellow)
                    SweepWhoa(rm.gm.aiTeamYellow);
                else
                    SweepWhoa(false);
            }
            if (sweeperL.gameObject.activeSelf && sweeperL.gameObject.GetComponent<CharacterStats>().sweepHealth <= 0f)
            {
                if (rm.gm.aiTeamRed)
                    SweepWhoa(rm.gm.aiTeamRed);
                else if (rm.gm.aiTeamYellow)
                    SweepWhoa(rm.gm.aiTeamYellow);
                else
                    SweepWhoa(false);
            }
        }
        
        
    }

    public void SetupSweepers(bool redTurn)
    {
        sweeperRedL.gameObject.SetActive(false);
        sweeperRedR.gameObject.SetActive(false);
        sweeperYellowL.gameObject.SetActive(false);
        sweeperYellowR.gameObject.SetActive(false);

        if (redTurn)
        {
            sweeperL = sweeperRedL;
            sweeperR = sweeperRedR;
        }
        else
        {
            sweeperL = sweeperYellowL;
            sweeperR = sweeperYellowR;
        }

        sweeperL.gameObject.SetActive(true);
        sweeperR.gameObject.SetActive(true);
        swprLStats = sweeperL.gameObject.GetComponent<CharacterStats>();
        swprRStats = sweeperR.gameObject.GetComponent<CharacterStats>();

        sweepSel.gameObject.SetActive(false);
        sweeperL.sweep = false;
        sweeperL.hard = false;
        sweeperL.whoa = true;

        sweeperR.sweep = false;
        sweeperR.hard = false;
        sweeperR.whoa = true;
    }

    public void ResetSweepers()
    {
        sweeperRedL.gameObject.SetActive(false);
        sweeperRedR.gameObject.SetActive(false);
        sweeperYellowL.gameObject.SetActive(false);
        sweeperYellowR.gameObject.SetActive(false);

        am.Stop("Sweep");
        am.Stop("Hard");
        sweepSel.gameObject.SetActive(false);
        sweeperL.sweep = false;
        sweeperL.hard = false;
        sweeperL.whoa = true;

        sweeperR.sweep = false;
        sweeperR.hard = false;
        sweeperR.whoa = true;
    }

    public void Release(GameObject rock, bool aiTurn)
    {
        sweepSel.gameObject.SetActive(true);
        sweepSel.AttachToRock(rock);
        inturn = rm.inturn;
        //sweep.OnWhoa();
        if (!aiTurn)
        {
            sweepButton.SetActive(true);
            hardButton.SetActive(false);
            whoaButton.SetActive(false);
        }
        else
        {
            sweepButton.SetActive(false);
            hardButton.SetActive(false);
            whoaButton.SetActive(false);
        }

        if (inturn)
        {
            sweeperL.yOffset = 1.1f;

            sweeperR.yOffset = 0.6f;
        }
        else
        {
            sweeperL.yOffset = 0.6f;

            sweeperR.yOffset = 1.1f;
        }
    }

    public void SweepWeight(bool aiTurn)
    {
        am.Play("Sweep");
        sweeperL.Sweep();
        sweeperR.Sweep();
        sweep.OnSweep();

        if (!aiTurn)
        {
            sweepButton.SetActive(false);
            hardButton.SetActive(true);
            whoaButton.SetActive(true);
        }
    }

    public void SweepHard(bool aiTurn)
    {
        am.Stop("Sweep");
        am.Play("Hard");
        sweeperL.Hard();
        sweeperR.Hard();
        sweep.OnHard();

        if (!aiTurn)
        {
            hardButton.SetActive(false);
            sweepButton.SetActive(false);
            whoaButton.SetActive(true);
        }
    }

    public void SweepWhoa(bool aiTurn)
    {
        am.Stop("Sweep");
        am.Stop("Hard");
        sweeperL.Whoa();
        sweeperR.Whoa();
        sweep.OnWhoa();

        if (!aiTurn)
        {
            whoaButton.SetActive(false);
            sweepButton.SetActive(true);
            hardButton.SetActive(false);
        }
    }

    public void SweepLeft(bool aiTurn)
    {
        am.Play("Sweep");
        sweep.OnLeft();
        sweeperL.yOffset = 0.6f;
        sweeperL.Sweep();

        sweeperR.yOffset = 1.1f;
        sweeperR.Whoa();

        if (!aiTurn)
        {
            sweepButton.SetActive(true);
            hardButton.SetActive(false);
            whoaButton.SetActive(true);
        }
    }

    public void SweepRight(bool aiTurn)
    {
        am.Play("Sweep");
        sweep.OnRight();
        sweeperL.yOffset = 1.1f;
        sweeperL.Whoa();

        sweeperR.yOffset = 0.6f;
        sweeperR.Sweep();

        if (!aiTurn)
        {
            sweepButton.SetActive(true);
            hardButton.SetActive(false);
            whoaButton.SetActive(true);
        }
    }
}
