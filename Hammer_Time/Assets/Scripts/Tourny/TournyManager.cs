using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

public class TournyManager : MonoBehaviour
{
	public StandingDisplay[] standDisplay;
	public BracketDisplay[] brackDisplay;
	public VSDisplay[] vsDisplay;
	public Team[] teams;
	public DrawFormat[] drawFormat;
	public Team[] playoffTeams;
	public List<Team_List> teamList;

	public GameObject standings;
	public GameObject playoffs;
	public GameObject semiWinner;
	public GameObject finalWinner;
	public GameObject vs;
	public Button simButton;
	public Button contButton;
	public Text heading;
	public Scrollbar scrollBar;
	GameSettingsPersist gsp;

	public int draw;
	public int playoffRound;
	public int playerTeam;
	// Start is called before the first frame update
	void Start()
    {
		gsp = GameObject.Find("GameSettingsPersist").GetComponent<GameSettingsPersist>();

		teamList = new List<Team_List>();

		Shuffle(teams);

		playerTeam = Random.Range(0, 6);
		teams[playerTeam].name = gsp.teamName;

		for (int i = 0; i < teams.Length; i++)
		{
			teamList.Add(new Team_List(teams[i]));
			teams[i].strength = Random.Range(0, 10);
        }

		SetDraw();
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

	void SetDraw()
    {
		if (draw < drawFormat.Length)
		{
			teams[drawFormat[draw].game1.x].nextOpp = teams[drawFormat[draw].game1.y].name;
			teams[drawFormat[draw].game1.y].nextOpp = teams[drawFormat[draw].game1.x].name;


			teams[drawFormat[draw].game2.x].nextOpp = teams[drawFormat[draw].game2.y].name;
			teams[drawFormat[draw].game2.y].nextOpp = teams[drawFormat[draw].game2.x].name;


			teams[drawFormat[draw].game3.x].nextOpp = teams[drawFormat[draw].game3.y].name;
			teams[drawFormat[draw].game3.y].nextOpp = teams[drawFormat[draw].game3.x].name;
		}

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
						vsDisplay[1].name.text = "BYE TO FINALS";
						vsDisplay[1].rank.text = "-";
						break;
					case 2:
						vsDisplay[1].name.text = playoffTeams[2].name;
						vsDisplay[1].rank.text = playoffTeams[2].rank.ToString();
						break;
					case 3:
						vsDisplay[1].name.text = playoffTeams[1].name;
						vsDisplay[1].rank.text = playoffTeams[1].rank.ToString();
						break;
					default:
						vs.SetActive(false);
						break;
                }

				scrollBar.value = 0;
				StartCoroutine(RefreshPlayoffPanel());

				standings.SetActive(false);
				playoffs.SetActive(true);
				playoffRound++;

				simButton.gameObject.SetActive(true);
				contButton.gameObject.SetActive(false);
				break;

			case 2:
				heading.text = "Finals";
				semiWinner.SetActive(true);
				scrollBar.value = 0.5f;
				brackDisplay[3].name.text = playoffTeams[3].name;
				brackDisplay[3].rank.text = playoffTeams[3].rank.ToString();

				if (playoffTeams[0].name == teams[playerTeam].name)
				{
					vsDisplay[1].name.text = playoffTeams[3].name;
					vsDisplay[1].rank.text = playoffTeams[3].rank.ToString();
				}
				else if (playoffTeams[3].name == teams[playerTeam].name)
				{
					vsDisplay[1].name.text = playoffTeams[0].name;
					vsDisplay[1].rank.text = playoffTeams[0].rank.ToString();
				}
				else
					vs.SetActive(false);

				StartCoroutine(RefreshPlayoffPanel());

				simButton.gameObject.SetActive(true);
				contButton.gameObject.SetActive(false);
				break;

			case 3:
				heading.text = "Winner";
				scrollBar.value = 1;
				finalWinner.SetActive(true);
				brackDisplay[4].name.text = playoffTeams[4].name;
				brackDisplay[4].rank.text = playoffTeams[4].rank.ToString();

				StartCoroutine(RefreshPlayoffPanel());

				contButton.gameObject.SetActive(false);
				simButton.gameObject.SetActive(false);
				break;
		}
	}

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

	IEnumerator SimDraw()
    {
		//SetDraw();
		Team game1X = teams[drawFormat[draw].game1.x];
		Team game1Y = teams[drawFormat[draw].game1.y];

		Team game2X = teams[drawFormat[draw].game2.x];
		Team game2Y = teams[drawFormat[draw].game2.y];

		Team game3X = teams[drawFormat[draw].game3.x];
		Team game3Y = teams[drawFormat[draw].game3.y];

		if (Random.Range(0, game1X.strength) > Random.Range(0, game1Y.strength))
		{
			game1Y.loss++;
			game1X.wins++;
		}
		else
		{
			game1X.loss++;
			game1Y.wins++;
		}

		if (Random.Range(0, game2X.strength) > Random.Range(0, game2Y.strength))
		{
			game2Y.loss++;
			game2X.wins++;
		}
		else
		{
			game2X.loss++;
			game2Y.wins++;
		}

		if (Random.Range(0, game3X.strength) > Random.Range(0, game3Y.strength))
		{
			game3Y.loss++;
			game3X.wins++;
		}
		else
		{
			game3X.loss++;
			game3Y.wins++;
		}

		if (draw < drawFormat.Length - 1)
		{
			Debug.Log("Draw number " + draw);
			yield return new WaitForSeconds(0.1f);
			heading.text = "After Draw " + (draw + 1);
			SetDraw();
			draw++;
		}
		else if (draw == drawFormat.Length - 1)
		{
			//Debug.Log("Final End");
			heading.text = "End of Draw";
			SetDraw();
			simButton.gameObject.SetActive(false);
			contButton.gameObject.SetActive(true);

		}
		else
			heading.text = "End of Round Robin";

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

		yield break;
		//SetPlayoffs();
	}

	public void Menu()
    {
		SceneManager.LoadScene("SplashMenu");
    }
}
