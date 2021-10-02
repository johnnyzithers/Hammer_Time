using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TigerForge;
using System;
using Random = UnityEngine.Random;

public class TournyManager : MonoBehaviour
{
	public PlayoffManager pm;
	public StandingDisplay[] standDisplay;
	public BracketDisplay[] brackDisplay;
	public VSDisplay[] vsDisplay;
	public TournyTeamList tTeamList;
	public Team[] teams;
	public DrawFormat[] drawFormat;
	public Team[] playoffTeams;
	public List<Team_List> teamList;
	public DrawFormatList dfList;

	public GameObject[] standDisplayTest;
	public GameObject standings;
	//public Transform panel;
	public Transform standTextParent;
	GameObject[] row;
	public GameObject standTextRow;
	public GameObject playoffs;
	public GameObject semiWinner;
	public GameObject finalWinner;
	public GameObject vs;
	public Button simButton;
	public Button contButton;
	public Button playButton;
	public Text heading;
	public Scrollbar scrollBar;
	public Scrollbar standScrollBar;

	GameSettingsPersist gsp;
	EasyFileSave myFile;
	public int numberOfTeams;
	public int draw;
	public int playoffRound;
	public int playerTeam;
	public int oppTeam;

	int careerWins;
	float careerPoints;
	string teamName;

	// Start is called before the first frame update
	void Start()
	{
		myFile = new EasyFileSave("my_tourny_data");
		gsp = GameObject.Find("GameSettingsPersist").GetComponent<GameSettingsPersist>();

		teamList = new List<Team_List>();

		standDisplay = new StandingDisplay[teams.Length];

		StartCoroutine(SetupStandings());

        Debug.Log("Draw at top of start - " + gsp.draw);
		
		//PrintRows(teams);
	}

	IEnumerator RefreshPanel()
	{
		for (int i = 0; i < standDisplay.Length; i++)
        {
			standDisplay[i].name.gameObject.transform.parent.GetComponent<ContentSizeFitter>().enabled = false;
			standDisplay[i].nextOpp.gameObject.transform.parent.GetComponent<ContentSizeFitter>().enabled = false;

			yield return new WaitForEndOfFrame();
			standDisplay[i].name.gameObject.transform.parent.GetComponent<ContentSizeFitter>().enabled = true;
			standDisplay[i].nextOpp.gameObject.transform.parent.GetComponent<ContentSizeFitter>().enabled = true;
		}

		for (int i = 0; i < vsDisplay.Length; i++)
		{
			vsDisplay[i].name.gameObject.GetComponent<ContentSizeFitter>().enabled = false;

			yield return new WaitForEndOfFrame();
			vsDisplay[i].name.gameObject.GetComponent<ContentSizeFitter>().enabled = true;
		}
    }

	IEnumerator RefreshPlayoffPanel()
	{
		for (int i = 0; i < brackDisplay.Length; i++)
		{
			brackDisplay[i].name.gameObject.transform.parent.GetComponent<ContentSizeFitter>().enabled = false;

			yield return new WaitForEndOfFrame();
			brackDisplay[i].name.gameObject.transform.parent.GetComponent<ContentSizeFitter>().enabled = true;
		}

		for (int i = 0; i < vsDisplay.Length; i++)
		{
			vsDisplay[i].name.gameObject.GetComponent<ContentSizeFitter>().enabled = false;

			yield return new WaitForEndOfFrame();
			vsDisplay[i].name.gameObject.GetComponent<ContentSizeFitter>().enabled = true;
		}
	}

	IEnumerator SetupStandings()
    {
		row = new GameObject[teams.Length];
		Debug.Log("Team Length is " + teams.Length);
		dfList.DrawSelector(teams.Length);

		yield return new WaitForEndOfFrame();

		drawFormat = dfList.currentFormat;

		for (int i = 0; i < teams.Length; i++)
		{
			row[i] = Instantiate(standTextRow, standTextParent);
			row[i].name = "Row " + (i + 1);
			row[i].GetComponent<RectTransform>().position = new Vector2(0f, i * -125f);
			//Text[] tList = row.transform.GetComponentsInChildren<Text>();

			RowVariables rv = row[i].GetComponent<RowVariables>();
			yield return new WaitForEndOfFrame();

			standDisplay[i] = rv.standDisplay;
		}

		if (gsp.draw > 0)
		{
			playoffRound = gsp.playoffRound;
			teamList = gsp.teamList;
			teams = gsp.teams;
			draw = gsp.draw;

			if (playoffRound > 0)
			{
				SetupPlayoffs();
			}
			else
			{
				for (int i = 0; i < teams.Length; i++)
				{
					if (teams[i].name == gsp.playerTeam.nextOpp)
						oppTeam = i;
					if (teams[i].name == gsp.playerTeam.name)
						playerTeam = i;
				}

				Debug.Log("OppTeam is " + oppTeam);

				if (gsp.playerTeam.name == gsp.redTeamName)
				{
					if (gsp.redScore > gsp.yellowScore)
					{
						teams[oppTeam].loss++;
						teams[playerTeam].wins++;
					}
					else
					{
						teams[oppTeam].wins++;
						teams[playerTeam].loss++;
					}
				}
				else
				{
					if (gsp.redScore < gsp.yellowScore)
					{
						teams[oppTeam].loss++;
						teams[playerTeam].wins++;
					}
					else
					{
						teams[oppTeam].wins++;
						teams[playerTeam].loss++;
					}
				}
				Debug.Log(teams[oppTeam].name + " " + teams[oppTeam].wins + " Wins");
				StartCoroutine(SimRestDraw());
			}

		}
		else
		{
			Shuffle(teams);

			for (int i = 0; i < teams.Length; i++)
			{
				teamList.Add(new Team_List(teams[i]));
				teams[i].strength = Random.Range(0, 10);
			}

			playerTeam = Random.Range(0, teams.Length);
			teamList[playerTeam].team.name = gsp.teamName;

			yield return new WaitForEndOfFrame();
			SetDraw();
		}

		//yield return new WaitUntil( () => standDisplay.Length == teams.Length);
		yield return new WaitForEndOfFrame();
	}

	void SetupPlayoffs()
    {
		playoffTeams = gsp.playoffTeams;

		for (int i = 0; i < teams.Length; i++)
		{
			if (teams[i].name == gsp.playerTeam.name)
				playerTeam = i;
			if (teams[i].name == gsp.playerTeam.nextOpp)
				oppTeam = i;
		}

		Debug.Log("OppTeam is " + oppTeam);

		if (playoffRound == 2)
		{
			if (gsp.playerTeam.name == gsp.redTeamName)
			{
				if (gsp.redScore > gsp.yellowScore)
				{
					playoffTeams[3] = teams[playerTeam];
				}
				else
				{
					playoffTeams[3] = teams[oppTeam];
				}
			}
			else
			{
				if (gsp.redScore < gsp.yellowScore)
				{
					playoffTeams[3] = teams[playerTeam];
				}
				else
				{
					playoffTeams[3] = teams[oppTeam];
				}
			}
		}
		else if (playoffRound == 3)
		{
			if (gsp.playerTeam.name == gsp.redTeamName)
			{
				if (gsp.redScore > gsp.yellowScore)
				{
					playoffTeams[4] = teams[playerTeam];
				}
				else
				{
					playoffTeams[4] = teams[oppTeam];
				}
			}
			else
			{
				if (gsp.redScore < gsp.yellowScore)
				{
					playoffTeams[4] = teams[playerTeam];
				}
				else
				{
					playoffTeams[4] = teams[oppTeam];
				}
			}
		}

		pm.SetPlayoffs(playoffRound);
	}

	void Shuffle(Team[] a)
	{
		// Loops through array
		for (int i = a.Length - 1; i > 0; i--)
		{
			// Randomize a number between 0 and i (so that the range decreases each time)
			int rnd = Random.Range(0, i);

			// Save the value of the current i, otherwise it'll overright when we swap the values
			Team temp = a[i];

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

	void PrintRows()
	{
		int tempRank;
		teamList.Sort();

		for (int i = 0; i < teamList.Count; i++)
        {
			standDisplay[i].name.text = teamList[i].team.name;
            standDisplay[i].wins.text = teamList[i].team.wins.ToString();
            standDisplay[i].loss.text = teamList[i].team.loss.ToString();
            standDisplay[i].nextOpp.text = teamList[i].team.nextOpp;
            teamList[i].team.rank = i + 1;
        }

		vsDisplay[0].name.text = teams[playerTeam].name;
		vsDisplay[0].rank.text = teams[playerTeam].rank.ToString();

		for (int i = 0; i < teamList.Count; i++)
        {
			if (teams[playerTeam].nextOpp == teamList[i].team.name)
			{
				tempRank = i + 1;
				vsDisplay[1].name.text = teamList[i].team.name;
				vsDisplay[1].rank.text = tempRank.ToString();
			}
        }
		StartCoroutine(RefreshPanel());
    }

    #region Set
    void SetDraw()
    {
		if (draw < drawFormat.Length)
		{
			for (int i = 0; i < drawFormat[draw].game.Length; i++)
			{
				teams[drawFormat[draw].game[i].x].nextOpp = teams[drawFormat[draw].game[i].y].name;
				teams[drawFormat[draw].game[i].y].nextOpp = teams[drawFormat[draw].game[i].x].name;
			}
		}
		else if (draw == drawFormat.Length)
        {
			for (int i = 0; i < teamList.Count; i++)
            {
				teamList[i].team.nextOpp = "-----";
            }
        }

		for (int i = 0; i < teams.Length; i++)
		{
			if (teams[i].name == teams[playerTeam].nextOpp)
				oppTeam = i;
		}

		//yield return new WaitUntil(() => standDisplay.Length >= row.Length);

		//yield return new WaitUntil(() => standDisplay.Length );
		PrintRows();
    }

	public void SetPlayoffs()
	{
		switch (playoffRound)
        {
			case 0:
				heading.text = "Semifinals";
				for (int i = 0; i < 3; i++)
				{
					playoffTeams[i] = teamList[i].team;
					brackDisplay[i].name.text = playoffTeams[i].name;
					brackDisplay[i].rank.text = playoffTeams[i].rank.ToString();
				}
				switch (teams[playerTeam].rank)
                {
					case 1:
						playButton.gameObject.SetActive(false);
						teams[playerTeam].nextOpp = "-----";
						vsDisplay[1].name.text = "BYE TO FINALS";
						vsDisplay[1].rank.text = "-";
						break;
					case 2:
						playButton.gameObject.SetActive(true);
						teams[playerTeam].nextOpp = playoffTeams[2].name;
						vsDisplay[1].name.text = playoffTeams[2].name;
						vsDisplay[1].rank.text = playoffTeams[2].rank.ToString();
						break;
					case 3:
						playButton.gameObject.SetActive(true);
						teams[playerTeam].nextOpp = playoffTeams[1].name;
						vsDisplay[1].name.text = playoffTeams[1].name;
						vsDisplay[1].rank.text = playoffTeams[1].rank.ToString();
						break;
					default:
						playButton.gameObject.SetActive(false);
						vs.SetActive(false);
						playButton.gameObject.SetActive(false);
						break;
                }

				StartCoroutine(RefreshPlayoffPanel());

				standings.SetActive(false);
				playoffs.SetActive(true);
				playoffRound++;

				simButton.gameObject.SetActive(true);
				contButton.gameObject.SetActive(false);
				scrollBar.value = 0;
				break;

			case 2:
				heading.text = "Finals"; 
				
				for (int i = 0; i < 4; i++)
				{
					brackDisplay[i].name.text = playoffTeams[i].name;
					brackDisplay[i].rank.text = playoffTeams[i].rank.ToString();
				}

				semiWinner.SetActive(true);
				brackDisplay[3].name.text = playoffTeams[3].name;
				brackDisplay[3].rank.text = playoffTeams[3].rank.ToString();

				if (playoffTeams[0].name == teams[playerTeam].name)
				{
					playButton.gameObject.SetActive(true);

					vsDisplay[0].name.text = playoffTeams[0].name;
					vsDisplay[0].rank.text = playoffTeams[0].rank.ToString();
					teams[playerTeam].nextOpp = playoffTeams[3].name;
					vsDisplay[1].name.text = playoffTeams[3].name;
					vsDisplay[1].rank.text = playoffTeams[3].rank.ToString();
				}
				else if (playoffTeams[3].name == teams[playerTeam].name)
				{
					playButton.gameObject.SetActive(true);
					vsDisplay[0].name.text = playoffTeams[3].name;
					vsDisplay[0].rank.text = playoffTeams[3].rank.ToString();
					teams[playerTeam].nextOpp = playoffTeams[0].name;
					vsDisplay[1].name.text = playoffTeams[0].name;
					vsDisplay[1].rank.text = playoffTeams[0].rank.ToString();
				}
				else
				{
					vs.SetActive(false);
					playButton.gameObject.SetActive(false);
				}

				standings.SetActive(false);
				playoffs.SetActive(true);
				StartCoroutine(RefreshPlayoffPanel());

				simButton.gameObject.SetActive(true);
				contButton.gameObject.SetActive(false);
				scrollBar.value = 0.5f;
				break;

			case 3:
				heading.text = "Winner";

				for (int i = 0; i < 5; i++)
				{
					brackDisplay[i].name.text = playoffTeams[i].name;
					brackDisplay[i].rank.text = playoffTeams[i].rank.ToString();
				}

				semiWinner.SetActive(true);
				finalWinner.SetActive(true);
				brackDisplay[4].name.text = playoffTeams[4].name;
				brackDisplay[4].rank.text = playoffTeams[4].rank.ToString();

				standings.SetActive(false);
				playoffs.SetActive(true);
				StartCoroutine(RefreshPlayoffPanel());

				if (teams[playerTeam].name == playoffTeams[4].name)
					heading.text = "You Win!";
				else
					heading.text = "So Close!";

				vs.SetActive(false);
				playButton.gameObject.SetActive(false);
				contButton.gameObject.SetActive(false);
				simButton.gameObject.SetActive(false);
				scrollBar.value = 1;

				for (int i = 0; i < playoffTeams.Length; i++)
                {
					if (playoffTeams[i].name == teams[playerTeam].name)
                    {

                    }
				}
				break;
		}
	}
    #endregion

    #region Sim
    IEnumerator SimDraw()
    {
		Team[] games = new Team[teams.Length];

		//SetDraw();
		for (int i = 0; i < teams.Length; i++)
        {
			if (i % 2 == 0)
				games[i] = teams[drawFormat[draw].game[i / 2].x];
			else
				games[i] = teams[drawFormat[draw].game[i / 2].y];
        }

		//games[0] = teams[drawFormat[draw].game[0].x];

		//games[1] = teams[drawFormat[draw].game[0].y];

		//games[2] = teams[drawFormat[draw].game[1].x];
		//games[3] = teams[drawFormat[draw].game[1].y];

		//games[4] = teams[drawFormat[draw].game[2].x];
		//games[5] = teams[drawFormat[draw].game[2].y];

		for (int i = 0; i < games.Length; i++)
		{
			if (i % 2 == 0)
			{
				if (Random.Range(0, games[i].strength) > Random.Range(0, games[i + 1].strength))
				{
						games[i + 1].loss++;
						games[i].wins++;
				}
				else
				{
						games[i].loss++;
						games[i + 1].wins++;
				}
			}
		}
		
		draw++;
		yield return StartCoroutine(DrawScoring());
	}

	IEnumerator SimRestDraw()
	{
		int tempDraw = draw - 1;
		Debug.Log("Temp Draw " + tempDraw);
		Team[] games = new Team[teams.Length];
		//SetDraw();
		for (int i = 0; i < teams.Length; i++)
		{
			if (i % 2 == 0)
				games[i] = teams[drawFormat[draw].game[i / 2].x];
			else
				games[i] = teams[drawFormat[draw].game[(i - 1) / 2].y];
		}
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < games.Length; i++)
        {
			if (i % 2 == 0)
			{
				//Debug.Log("Settling Game - " + games[i].name);
				if (games[i].name == teams[playerTeam].name || games[i].name == teams[oppTeam].name)
                {
					Debug.Log("Player Game skip sim - " + i + " - " + games[i].name);
				}
				else if (Random.Range(0, games[i].strength) > Random.Range(0, games[i + 1].strength))
				{
					//if (i + 1 != playerTeam & i + 1 != oppTeam)
						games[i + 1].loss++;
					//if (i != playerTeam & i != oppTeam)
						games[i].wins++;
				}
				else
				{
					//if (i != playerTeam & i != oppTeam)
						games[i].loss++;
					//if (i + 1 != playerTeam & i + 1 != oppTeam)
						games[i + 1].wins++;
				}
			}
        }
		yield return StartCoroutine(DrawScoring());
	}

	IEnumerator SimPlayoff()
	{
		switch (playoffRound)
        {
			case 1:
				Team semiX = playoffTeams[1];
				Team semiY = playoffTeams[2];

				if (Random.Range(0, semiX.strength) > Random.Range(0, semiY.strength))
				{
					playoffTeams[3] = semiX;
				}
				else
				{
					playoffTeams[3] = semiY;
				}

				semiWinner.SetActive(true);
				brackDisplay[3].rank.text = playoffTeams[3].rank.ToString();
				brackDisplay[3].name.text = playoffTeams[3].name;
				StartCoroutine(RefreshPlayoffPanel());
				playoffRound++;
				simButton.gameObject.SetActive(false);
				contButton.gameObject.SetActive(true);
				break;

			case 2:
				if (Random.Range(0, playoffTeams[0].strength) > Random.Range(0, playoffTeams[3].strength))
                {
					playoffTeams[4] = playoffTeams[0];
                }
				else
                {
					playoffTeams[4] = playoffTeams[3];
				}
				playoffRound++;
				SetPlayoffs();
				simButton.gameObject.SetActive(false);
				contButton.gameObject.SetActive(true);
				break;

			default:
				break;

		}
		SetPlayoffs();
		yield break;
		//SetPlayoffs();
	}

	IEnumerator DrawScoring()
    {

		if (draw < drawFormat.Length)
		{
			Debug.Log("Draw number " + draw);
			yield return new WaitForSeconds(0.1f);
			heading.text = "Draw " + (draw + 1);
			SetDraw();
		}
		else if (draw == drawFormat.Length)
		{
			//Debug.Log("Final End");
			heading.text = "End of Draws";
			SetDraw();
			playButton.gameObject.SetActive(false);
			simButton.gameObject.SetActive(false);
			contButton.gameObject.SetActive(true);

		}
		else
			heading.text = "End of Round Robin";

		

		
	}
	#endregion

	
	public void OnSim()
	{
		if (playoffRound > 0)
		{
			StartCoroutine(SimPlayoff());
		}
		else if (draw < drawFormat.Length)
		{
			StartCoroutine(SimDraw());
		}
	}

	public void PlayDraw()
    {
		gsp.TournySetup();
		SceneManager.LoadScene("End_Menu_Tourny_1");
    }
	public void Menu()
    {
		SceneManager.LoadScene("SplashMenu");
    }
}
