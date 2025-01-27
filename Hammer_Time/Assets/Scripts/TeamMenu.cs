using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TigerForge;

public class TeamMenu : MonoBehaviour
{
    CareerManager cm;
    public PowerUpManager pm;
    public TournySelector tSel;

    public GameObject teamMenu;
    public GameObject agentMenu;
    public GameObject setTeamButton;

    public Player[] activePlayers;
    public Player[] freeAgents;

    public Color buttonDisabledColor;
    public Color buttonEnabledColor;

    public Player[] playerPool;

    public PlayerDisplay[] teamDisplay;

    public PlayerDisplay replaceMemberDisplay;
    public PlayerDisplay[] freeAgentDisplay;

    public GameObject dialogueGO;
    public DialogueTrigger coachGreen;

    public Slider drawSlider;
    public Slider guardSlider;
    public Slider takeOutSlider;
    public Slider strengthSlider;
    public Slider enduranceSlider;
    public Slider healthSlider;

    public Slider oppDrawSlider;
    public Slider oppGuardSlider;
    public Slider oppTakeOutSlider;
    public Slider oppStrengthSlider;
    public Slider oppEnduranceSlider;
    public Slider oppHealthSlider;

    public float xp;
    public float cash;
    public Text xpText;
    public Text cashText;

    int oppStatBase;

    EasyFileSave myFile;

    bool callCount;

    // Start is called before the first frame update
    void Start()
    {
        cm = FindObjectOfType<CareerManager>();
        //Shuffle(playerPool);
        callCount = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (cm)
        {
            if (cm.week < 5)
                oppStatBase = 5;
            else if (cm.week < 10)
                oppStatBase = 7;
            else
                oppStatBase = 10;

            drawSlider.value = cm.cStats.drawAccuracy + cm.modStats.drawAccuracy;
            guardSlider.value = cm.cStats.guardAccuracy + cm.modStats.guardAccuracy;
            takeOutSlider.value = cm.cStats.takeOutAccuracy + cm.modStats.takeOutAccuracy;
            enduranceSlider.value = cm.cStats.sweepEndurance + cm.modStats.sweepEndurance;
            strengthSlider.value = cm.cStats.sweepStrength + cm.modStats.sweepStrength;
            healthSlider.value = cm.cStats.sweepCohesion + cm.modStats.sweepCohesion;

            oppDrawSlider.value = oppStatBase + cm.oppStats.drawAccuracy;
            oppGuardSlider.value = oppStatBase + cm.oppStats.guardAccuracy;
            oppTakeOutSlider.value = oppStatBase + cm.oppStats.takeOutAccuracy;
            oppEnduranceSlider.value = oppStatBase + cm.oppStats.sweepEndurance;
            oppStrengthSlider.value = oppStatBase + cm.oppStats.sweepStrength;
            oppHealthSlider.value = oppStatBase + cm.oppStats.sweepCohesion;

            xp = cm.xp;
            cash = cm.earnings;
            xpText.text = xp.ToString();
            cashText.text = "$" + cash.ToString("n0");
        }
    }

    public void TeamMenuOpen()
    {
        if (!callCount)
        {
            callCount = true;
            cm = FindObjectOfType<CareerManager>();
            StartCoroutine(SetUpTeam());
        }
    }

    IEnumerator SetUpTeam()
    {
        myFile = new EasyFileSave("my_player_data");

        cm.coachDialogue = new bool[tSel.coachGreen.dialogue.Length];
        cm.qualDialogue = new bool[tSel.coachGreen.qualDialogue.Length];
        cm.reviewDialogue = new bool[tSel.coachGreen.reviewDialogue.Length];
        cm.introDialogue = new bool[tSel.coachGreen.introDialogue.Length];
        cm.storyDialogue = new bool[tSel.coachGreen.storyDialogue.Length];
        cm.helpDialogue = new bool[tSel.coachGreen.helpDialogue.Length];
        cm.strategyDialogue = new bool[tSel.coachGreen.strategyDialogue.Length];

        for (int i = 0; i < cm.coachDialogue.Length; i++)
            cm.coachDialogue[i] = false;
        for (int i = 0; i < cm.qualDialogue.Length; i++)
            cm.qualDialogue[i] = false;
        for (int i = 0; i < cm.reviewDialogue.Length; i++)
            cm.reviewDialogue[i] = false;
        for (int i = 0; i < cm.introDialogue.Length; i++)
            cm.introDialogue[i] = false;
        for (int i = 0; i < cm.storyDialogue.Length; i++)
            cm.storyDialogue[i] = false;
        for (int i = 0; i < cm.helpDialogue.Length; i++)
            cm.helpDialogue[i] = false;
        for (int i = 0; i < cm.strategyDialogue.Length; i++)
            cm.strategyDialogue[i] = false;

        Debug.Log("TeamMenu Earnings are " + cm.earnings);
        if (cm.week > 1)
        {
            //cm.LoadCareer();
            if (myFile.Load())
            {
                int[] activePlayerIdList = myFile.GetArray<int>("Active Players ID List");

                for (int i = 0; i < activePlayers.Length; i++)
                {
                    Debug.Log("Active Players ID is " + i + " - " + activePlayers[i].id);
                    for (int j = 0; j < playerPool.Length; j++)
                    {
                        if (activePlayerIdList[i] == playerPool[j].id)
                        {
                            activePlayers[i] = playerPool[j];
                            activePlayers[i].active = true;
                            playerPool[j].active = true;
                        }
                    }
                }

                if (cm.week == 2)
                {
                    Debug.Log("TEAM MENU - Player Rank is " + cm.playerTeam.rank);
                }
            }
            cm.teamPaid = false;

            Shuffle(playerPool);
        }
        else
        {
            for (int i = 0; i < activePlayers.Length; i++)
            {
                activePlayers[i] = playerPool[i];
            }

            Shuffle(playerPool);

            dialogueGO.SetActive(true);
            coachGreen.TriggerDialogue("Intro", 8);
            cm.introDialogue[8] = true;
        }

        SelectFreeAgents();
        cm.activePlayers = activePlayers;

        ViewTeam();

        yield return new WaitForEndOfFrame();
    }

    public void SelectFreeAgents()
    {
        //Shuffle(playerPool);

        for (int i = 0; i < freeAgents.Length; i++)
        {
            bool stop = false;
            for (int j = 0; j < playerPool.Length; j++)
            {
                if (!playerPool[j].active && !playerPool[j].view && !stop)
                {
                    playerPool[j].view = true;
                    freeAgents[i] = playerPool[j];
                    stop = true;
                }
            }
        }
    }

    public void ViewTeam()
    {
        for (int i = 0; i < freeAgentDisplay.Length; i++)
        {
            if (!freeAgentDisplay[i].charName.transform.parent.GetComponent<Button>().interactable)
            {
                freeAgentDisplay[i].charName.rectTransform.anchoredPosition += new Vector2(15f, 15f);
                freeAgentDisplay[i].cost.rectTransform.anchoredPosition += new Vector2(15f, 15f);
                //freeAgentDisplay[i].photo.rectTransform.anchoredPosition += new Vector2(35f, 35f);
                freeAgentDisplay[i].photo.transform.parent.gameObject.GetComponent<Image>().rectTransform.anchoredPosition += new Vector2(35f, 35f);
                GameObject tempPanel = freeAgentDisplay[i].charName.transform.parent.GetChild(2).gameObject;
                tempPanel.GetComponent<Image>().rectTransform.anchoredPosition += new Vector2(35f, 35f);
                freeAgentDisplay[i].charName.transform.parent.GetComponent<Image>().color = buttonEnabledColor;
                freeAgentDisplay[i].charName.transform.parent.GetComponent<Button>().interactable = true;
            }
        }

        teamMenu.SetActive(true);
        setTeamButton.SetActive(true);
        agentMenu.SetActive(false);
        //pm.nextWeekButton.gameObject.SetActive(false);
        PreviewPoints();

        for (int i = 0; i < playerPool.Length; i++)
        {
            playerPool[i].view = false;

            if (playerPool[i].id == activePlayers[0].id | playerPool[i].id == activePlayers[1].id | playerPool[i].id == activePlayers[2].id)
                playerPool[i].active = true;
            else
                playerPool[i].active = false;
        }

        for (int i = 0; i < teamDisplay.Length; i++)
        {
            teamDisplay[i].charName.text = activePlayers[i].name;
            teamDisplay[i].cost.text = "$" + activePlayers[i].cost.ToString("N0");
            teamDisplay[i].photo.sprite = activePlayers[i].image;
            teamDisplay[i].description.text = activePlayers[i].description;
        }

    }

    public void ViewFreeAgents(int teamMember)
    {
        agentMenu.SetActive(true);
        teamMenu.SetActive(false);

        replaceMemberDisplay.charName.text = activePlayers[teamMember].name;
        replaceMemberDisplay.cost.text = "$" + activePlayers[teamMember].cost.ToString("N0");
        replaceMemberDisplay.photo.sprite = activePlayers[teamMember].image;
        replaceMemberDisplay.description.text = activePlayers[teamMember].description;
        if (teamMember == 0)
            replaceMemberDisplay.charName.transform.parent.GetChild(5).GetComponent<Text>().text = "Lead";
        else if (teamMember == 1)
            replaceMemberDisplay.charName.transform.parent.GetChild(5).GetComponent<Text>().text = "Second";
        else
            replaceMemberDisplay.charName.transform.parent.GetChild(5).GetComponent<Text>().text = "Third";

        float moneyToSpend = cm.earnings;
        for (int i = 0; i < activePlayers.Length; i++)
        {
            if (i != teamMember)
            {
                moneyToSpend -= (activePlayers[i].cost);
            }
        }
        Debug.Log("Money to Spend is " + moneyToSpend);
        for (int i = 0; i < freeAgentDisplay.Length; i++)
        {
            freeAgentDisplay[i].charName.text = freeAgents[i].name;
            freeAgentDisplay[i].cost.text = "$" + freeAgents[i].cost.ToString("N0");
            freeAgentDisplay[i].photo.sprite = freeAgents[i].image;

            if (freeAgents[i].cost > moneyToSpend)
            {
                freeAgentDisplay[i].charName.rectTransform.anchoredPosition -= new Vector2(15f, 15f);
                freeAgentDisplay[i].cost.rectTransform.anchoredPosition -= new Vector2(15f, 15f);
                //freeAgentDisplay[i].photo.rectTransform.anchoredPosition -= new Vector2(15f, 15f);
                freeAgentDisplay[i].photo.transform.parent.gameObject.GetComponent<Image>().rectTransform.anchoredPosition -= new Vector2(35f, 35f);
                GameObject tempPanel = freeAgentDisplay[i].charName.transform.parent.GetChild(2).gameObject;
                tempPanel.GetComponent<Image>().rectTransform.anchoredPosition -= new Vector2(35f, 35f);
                freeAgentDisplay[i].charName.transform.parent.GetComponent<Image>().color = buttonDisabledColor;
                freeAgentDisplay[i].charName.transform.parent.GetComponent<Button>().interactable = false;
            }
        }

    }

    void TeamSelect()
    {
        for (int i = 0; i < activePlayers.Length; i++)
        {
            bool stop = false;
            for (int j = 0; j < playerPool.Length; j++)
            {
                if (playerPool[j].active && !stop)
                {
                    activePlayers[i] = playerPool[j];
                    stop = true;
                }
            }
        }
    }

    public void ChangeTeamMember(int freeAgent)
    {
        int playerToReplace = 99;
        
        for (int i = 0; i < activePlayers.Length; i++)
        {
            if (activePlayers[i].name == replaceMemberDisplay.charName.text)
            {
                playerToReplace = i;
            }
        }

        Player tempPlayer = activePlayers[playerToReplace];

        for (int i = 0; i < playerPool.Length; i++)
        {
            if (freeAgents[freeAgent].id == playerPool[i].id)
            {
                playerPool[i].active = true;
                activePlayers[playerToReplace] = playerPool[i];
            }
        }
        freeAgents[freeAgent] = tempPlayer;

        ViewTeam();
    }

    public void SetTeam()
    {
        float teamCost = 0f;

        for (int i = 0; i < activePlayers.Length; i++)
        {
            teamCost += activePlayers[i].cost;
        }

        Debug.Log("Team Cost is " + teamCost);

        cm.earnings -= teamCost;
        cm.teamPaid = true;
        //teamMenu.SetActive(false);
        //agentMenu.SetActive(false);
        ////pm.cardParent.SetActive(true);
        //setTeamButton.SetActive(false);
        //pm.nextWeekButton.SetActive(true);
        cm.SaveCareer();
        
        for (int i = 0; i < teamDisplay.Length; i++)
        {
            teamDisplay[i].charName.rectTransform.anchoredPosition -= new Vector2(15f, 15f);
            teamDisplay[i].cost.rectTransform.anchoredPosition -= new Vector2(15f, 15f);
            teamDisplay[i].description.rectTransform.anchoredPosition -= new Vector2(15f, 15f);
            //freeAgentDisplay[i].photo.rectTransform.anchoredPosition -= new Vector2(15f, 15f);
            teamDisplay[i].photo.transform.parent.gameObject.GetComponent<Image>().rectTransform.anchoredPosition -= new Vector2(35f, 35f);
            GameObject tempPanel = freeAgentDisplay[i].charName.transform.parent.GetChild(2).gameObject;
            tempPanel.GetComponent<Image>().rectTransform.anchoredPosition -= new Vector2(35f, 35f);
            teamDisplay[i].charName.transform.parent.GetComponent<Image>().color = buttonDisabledColor;
            teamDisplay[i].charName.transform.parent.GetComponent<Button>().interactable = false;
        }

        //pm.SetUp();
    }

    void Shuffle(Player[] a)
    {
        Debug.Log("Shuffling");
        // Loops through array
        for (int i = a.Length - 1; i > 0; i--)
        {
            // Randomize a number between 0 and i (so that the range decreases each time)
            int rnd = Random.Range(0, i);

            // Save the value of the current i, otherwise it'll overright when we swap the values
            Player temp = a[i];

            // Swap the new and old values
            a[i] = a[rnd];
            a[rnd] = temp;
        }
    }

    public void PreviewPoints()
    {
        cm = FindObjectOfType<CareerManager>();

        cm.modStats.drawAccuracy = activePlayers[0].draw + activePlayers[1].draw + activePlayers[2].draw;
        cm.modStats.guardAccuracy = activePlayers[0].guard + activePlayers[1].guard + activePlayers[2].guard;
        cm.modStats.takeOutAccuracy = activePlayers[0].takeOut + activePlayers[1].takeOut + activePlayers[2].takeOut;
        cm.modStats.sweepEndurance = activePlayers[0].sweepEnduro + activePlayers[1].sweepEnduro + activePlayers[2].sweepEnduro;
        cm.modStats.sweepStrength = activePlayers[0].sweepStrength + activePlayers[1].sweepStrength + activePlayers[2].sweepStrength;
        cm.modStats.sweepCohesion = activePlayers[0].sweepCohesion + activePlayers[1].sweepCohesion + activePlayers[2].sweepCohesion;
        cm.oppStats.drawAccuracy = activePlayers[0].oppDraw + activePlayers[1].oppDraw + activePlayers[2].oppDraw;
        cm.oppStats.guardAccuracy = activePlayers[0].oppGuard + activePlayers[1].oppGuard + activePlayers[2].oppGuard;
        cm.oppStats.takeOutAccuracy = activePlayers[0].oppTakeOut + activePlayers[1].oppTakeOut + activePlayers[2].oppTakeOut;
        cm.oppStats.sweepEndurance = activePlayers[0].oppEnduro + activePlayers[1].oppEnduro + activePlayers[2].oppEnduro;
        cm.oppStats.sweepStrength = activePlayers[0].oppStrength + activePlayers[1].oppStrength + activePlayers[2].oppStrength;
        cm.oppStats.sweepCohesion = activePlayers[0].oppCohesion + activePlayers[1].oppCohesion + activePlayers[2].oppCohesion;

    }
}
