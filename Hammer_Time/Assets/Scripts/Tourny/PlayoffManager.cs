using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayoffManager : MonoBehaviour
{
	public TournyManager tm;

	public List<PlayoffTeam_List> pOffList;
	public Team[] playoffTeams;

	public BracketDisplay[] brackDisplay;
	GameObject[] row;
	public GameObject playoffs;
	public GameObject vs;
	public Button simButton;
	public Button contButton;
	public Button playButton;
	public Text heading;
	public Scrollbar scrollBar;

	GameSettingsPersist gsp;

	int pTeams;
	public int playerTeam;
	public int oppTeam;

	private void Start()
	{
		SetSeeding(tm.teams.Length);
	}

	public void SetSeeding(int numberOfTeams)
    {

		pOffList = new List<PlayoffTeam_List>();
		pTeams = 4;
		playoffTeams = new Team[9];
		heading.text = "Page Playoff";
		for (int i = 0; i < pTeams; i++)
		{
			pOffList.Add(new PlayoffTeam_List(tm.teamList[i].team));
			brackDisplay[i].name.text = pOffList[i].team.name;
			brackDisplay[i].rank.text = pOffList[i].team.rank.ToString();
		}

		SetPlayoffs(tm.playoffRound);
	}

	IEnumerator RefreshPlayoffPanel()
	{
		for (int i = 0; i < brackDisplay.Length; i++)
		{
			brackDisplay[i].name.gameObject.transform.parent.GetComponent<ContentSizeFitter>().enabled = false;

			yield return new WaitForEndOfFrame();
			brackDisplay[i].name.gameObject.transform.parent.GetComponent<ContentSizeFitter>().enabled = true;
		}

		for (int i = 0; i < tm.vsDisplay.Length; i++)
		{
			tm.vsDisplay[i].name.gameObject.GetComponent<ContentSizeFitter>().enabled = false;

			yield return new WaitForEndOfFrame();
			tm.vsDisplay[i].name.gameObject.GetComponent<ContentSizeFitter>().enabled = true;
		}
	}
	void LoadPlayoffs(int playoffRound)
	{
		playoffTeams = gsp.playoffTeams;

		for (int i = 0; i < tm.teams.Length; i++)
		{
			if (tm.teams[i].name == gsp.playerTeam.name)
				playerTeam = i;
			if (tm.teams[i].name == gsp.playerTeam.nextOpp)
				oppTeam = i;
		}

		Debug.Log("OppTeam is " + oppTeam);
		switch (playoffRound)
        {
			case 1:
				bool game1 = false;
				bool game2 = false;

				int playerPlayoffPos = 10;

				for (int i = 0; i < 4; i++)
				{
					if (tm.teams[playerTeam] == playoffTeams[i])
					{
						playerPlayoffPos = i;

						if (i < 2)
							game1 = true;
						else
							game2 = true;
					}
				}

				if (game1)
                {
					if (gsp.playerTeam.name == gsp.redTeamName)
					{
						if (gsp.redScore > gsp.yellowScore)
                        {
							playoffTeams[4] = tm.teams[tm.playerTeam];
							playoffTeams[5] = tm.teams[tm.oppTeam];
						}
						else
						{
							playoffTeams[5] = tm.teams[tm.playerTeam];
							playoffTeams[4] = tm.teams[tm.oppTeam];
						}
					}
					else
					{
						if (gsp.redScore < gsp.yellowScore)
						{
							playoffTeams[4] = tm.teams[tm.playerTeam];
							playoffTeams[5] = tm.teams[tm.oppTeam];
						}
						else
						{
							playoffTeams[5] = tm.teams[tm.playerTeam];
							playoffTeams[4] = tm.teams[tm.oppTeam];
						}
					}
				}

				if (game2)
				{
					if (gsp.playerTeam.name == gsp.redTeamName)
					{
						if (gsp.redScore > gsp.yellowScore)
						{
							playoffTeams[6] = tm.teams[tm.playerTeam];
						}
						else
						{
							playoffTeams[6] = tm.teams[tm.oppTeam];
						}
					}
					else
					{
						if (gsp.redScore < gsp.yellowScore)
						{
							playoffTeams[6] = tm.teams[tm.playerTeam];
						}
						else
						{
							playoffTeams[6] = tm.teams[tm.oppTeam];
						}
					}
				}

				StartCoroutine(SimPlayoff(playoffRound, game1, game2));
				break;

			case 2:

				if (gsp.playerTeam.name == gsp.redTeamName)
				{
					if (gsp.redScore > gsp.yellowScore)
					{
						playoffTeams[7] = tm.teams[tm.playerTeam];
					}
					else
					{
						playoffTeams[7] = tm.teams[tm.oppTeam];
					}
				}
				else
				{
					if (gsp.redScore < gsp.yellowScore)
					{
						playoffTeams[7] = tm.teams[tm.playerTeam];
					}
					else
					{
						playoffTeams[7] = tm.teams[tm.oppTeam];
					}
				}

				break;

			case 3:

				if (gsp.playerTeam.name == gsp.redTeamName)
				{
					if (gsp.redScore > gsp.yellowScore)
					{
						playoffTeams[8] = tm.teams[tm.playerTeam];
					}
					else
					{
						playoffTeams[8] = tm.teams[tm.oppTeam];
					}
				}
				else
				{
					if (gsp.redScore < gsp.yellowScore)
					{
						playoffTeams[8] = tm.teams[tm.playerTeam];
					}
					else
					{
						playoffTeams[8] = tm.teams[tm.oppTeam];
					}
				}

				break;
		}

		SetPlayoffs(playoffRound);
	}
	public void SetPlayoffs(int playoffRound)
	{
		switch (playoffRound)
        {
			case 1:

                switch (tm.teams[playerTeam].rank)
                {
                    case 1:
                        playButton.gameObject.SetActive(true);
                        tm.vsDisplay[1].name.text = playoffTeams[0].name;
                        tm.vsDisplay[1].rank.text = playoffTeams[0].rank.ToString();
                        break;
                    case 2:
                        playButton.gameObject.SetActive(true);
                        tm.vsDisplay[1].name.text = playoffTeams[1].name;
                        tm.vsDisplay[1].rank.text = playoffTeams[1].rank.ToString();
                        break;
                    case 3:
                        playButton.gameObject.SetActive(true);
                        tm.vsDisplay[1].name.text = playoffTeams[2].name;
                        tm.vsDisplay[1].rank.text = playoffTeams[2].rank.ToString();
                        break;
					case 4:
						playButton.gameObject.SetActive(true);
						tm.vsDisplay[1].name.text = playoffTeams[3].name;
						tm.vsDisplay[1].rank.text = playoffTeams[3].rank.ToString();
						break;
					default:
                        playButton.gameObject.SetActive(false);
                        vs.SetActive(false);
                        playButton.gameObject.SetActive(false);
                        break;
                }

                StartCoroutine(RefreshPlayoffPanel());

                playoffs.SetActive(true);

                simButton.gameObject.SetActive(true);
                contButton.gameObject.SetActive(false);
                scrollBar.value = 0;
                break;

            case 2:
                heading.text = "Semifinals";

                for (int i = 0; i < 7; i++)
                {
                    brackDisplay[i].name.text = playoffTeams[i].name;
                    brackDisplay[i].rank.text = playoffTeams[i].rank.ToString();
					brackDisplay[i].name.transform.parent.gameObject.SetActive(true);
				}

                //brackDisplay[4].name.text = playoffTeams[4].name;
                //brackDisplay[4].rank.text = playoffTeams[4].rank.ToString();


				if (playoffTeams[4].name == tm.teams[playerTeam].name)
                {
                    playButton.gameObject.SetActive(false);

                    tm.vsDisplay[0].name.text = playoffTeams[4].name;
                    tm.vsDisplay[0].rank.text = playoffTeams[4].rank.ToString();
                    tm.vsDisplay[1].name.text = "BYE TO FINALS";
                    tm.vsDisplay[1].rank.text = "-";
                }
                else if (playoffTeams[5].name == tm.teams[playerTeam].name)
                {
                    playButton.gameObject.SetActive(true);
                    tm.vsDisplay[0].name.text = playoffTeams[5].name;
                    tm.vsDisplay[0].rank.text = playoffTeams[5].rank.ToString();
                    tm.vsDisplay[1].name.text = playoffTeams[6].name;
                    tm.vsDisplay[1].rank.text = playoffTeams[6].rank.ToString();
                }
				else if (playoffTeams[6].name == tm.teams[playerTeam].name)
                {
					playButton.gameObject.SetActive(true);
					tm.vsDisplay[0].name.text = playoffTeams[6].name;
					tm.vsDisplay[0].rank.text = playoffTeams[6].rank.ToString();
					tm.vsDisplay[1].name.text = playoffTeams[5].name;
					tm.vsDisplay[1].rank.text = playoffTeams[5].rank.ToString();
				}
                else
                {
                    vs.SetActive(false);
                    playButton.gameObject.SetActive(false);
                }

                playoffs.SetActive(true);
                StartCoroutine(RefreshPlayoffPanel());

                simButton.gameObject.SetActive(true);
                contButton.gameObject.SetActive(false);
                scrollBar.value = 0.5f;
                break;

			case 3:
				heading.text = "Finals";

				for (int i = 0; i < 8; i++)
				{
					brackDisplay[i].name.text = playoffTeams[i].name;
					brackDisplay[i].rank.text = playoffTeams[i].rank.ToString();
					brackDisplay[i].name.transform.parent.gameObject.SetActive(true);
				}

				if (playoffTeams[4].name == tm.teams[playerTeam].name)
				{
					playButton.gameObject.SetActive(false);

					tm.vsDisplay[0].name.text = playoffTeams[4].name;
					tm.vsDisplay[0].rank.text = playoffTeams[4].rank.ToString();
					tm.vsDisplay[1].name.text = playoffTeams[7].name;
					tm.vsDisplay[1].rank.text = playoffTeams[7].rank.ToString();
				}
				else if (playoffTeams[7].name == tm.teams[playerTeam].name)
				{
					playButton.gameObject.SetActive(true);
					tm.vsDisplay[0].name.text = playoffTeams[7].name;
					tm.vsDisplay[0].rank.text = playoffTeams[7].rank.ToString();
					tm.vsDisplay[1].name.text = playoffTeams[4].name;
					tm.vsDisplay[1].rank.text = playoffTeams[4].rank.ToString();
				}
				else
				{
					vs.SetActive(false);
					playButton.gameObject.SetActive(false);
				}

				playoffs.SetActive(true);
				StartCoroutine(RefreshPlayoffPanel());

				simButton.gameObject.SetActive(true);
				contButton.gameObject.SetActive(false);
				scrollBar.value = 1f;
				break;

			case 4:
				for (int i = 0; i < 9; i++)
				{
					brackDisplay[i].name.text = playoffTeams[i].name;
					brackDisplay[i].rank.text = playoffTeams[i].rank.ToString();
					brackDisplay[i].name.transform.parent.gameObject.SetActive(true);
				}

                playoffs.SetActive(true);
                StartCoroutine(RefreshPlayoffPanel());

				if (tm.teams[playerTeam].name == playoffTeams[8].name)
					heading.text = "You Win!";
				else if (tm.teams[playerTeam].name == playoffTeams[4].name | tm.teams[playerTeam].name == playoffTeams[7].name)
					heading.text = "Runner-up";
				else if (tm.teams[playerTeam].name == playoffTeams[5].name | tm.teams[playerTeam].name == playoffTeams[6].name)
					heading.text = "3rd Place";
				else if (tm.teams[playerTeam].name == playoffTeams[2].name | tm.teams[playerTeam].name == playoffTeams[3].name)
					heading.text = "4th Place";
				else
                {
					for (int i = 0; i < tm.teamList.Count; i++)
                    {
						if (tm.teams[playerTeam].name == tm.teamList[i].team.name)
                        {
							heading.text = i + "th Place";
                        }
                    }
                }
					heading.text = "So Close!";

                vs.SetActive(false);
                playButton.gameObject.SetActive(false);
                contButton.gameObject.SetActive(false);
                simButton.gameObject.SetActive(false);
                scrollBar.value = 1;

                break;

        }
	}

	public void OnSim(int playoffRound)
    {
		StartCoroutine(SimPlayoff(playoffRound, false, false));
    }
    IEnumerator SimPlayoff(int playoffRound, bool game1, bool game2)
	{
		Team game1X;
		Team game1Y;
		Team game2X;
		Team game2Y;

		switch (playoffRound)
		{
			case 1:
				if (!game1)
                {
					game1X = playoffTeams[0];
					game1Y = playoffTeams[1];

					if (Random.Range(0, game1X.strength) > Random.Range(0, game1Y.strength))
					{
						playoffTeams[4] = game1X;
						playoffTeams[5] = game1Y;
					}
					else
					{
						playoffTeams[4] = game1Y;
						playoffTeams[5] = game1X;
					}
				}
				
				if (!game2)
				{
					game2X = playoffTeams[2];
					game2Y = playoffTeams[3];

					if (Random.Range(0, game2X.strength) > Random.Range(0, game2Y.strength))
					{
						playoffTeams[6] = game2X;
					}
					else
					{
						playoffTeams[6] = game2Y;
					}
				}
				
				for (int i = 0; i < 7; i++)
                {
					brackDisplay[i].rank.text = playoffTeams[i].rank.ToString();
					brackDisplay[i].name.text = playoffTeams[i].name;
					brackDisplay[i].name.transform.parent.gameObject.SetActive(true);
				}
				StartCoroutine(RefreshPlayoffPanel());

				playoffRound++;
				simButton.gameObject.SetActive(false);
				contButton.gameObject.SetActive(true);
				break;

			case 2:
				game1X = playoffTeams[5];
				game1Y = playoffTeams[6];

				if (Random.Range(0, game1X.strength) > Random.Range(0, game1Y.strength))
				{
					playoffTeams[7] = game1X;
				}
				else
				{
					playoffTeams[7] = game1Y;
				}

				for (int i = 0; i < 8; i++)
				{
					brackDisplay[i].rank.text = playoffTeams[i].rank.ToString();
					brackDisplay[i].name.text = playoffTeams[i].name;
					brackDisplay[i].name.transform.parent.gameObject.SetActive(true);
				}
				StartCoroutine(RefreshPlayoffPanel());

				playoffRound++;
				simButton.gameObject.SetActive(false);
				contButton.gameObject.SetActive(true);
				break;

			case 3:
				game1X = playoffTeams[5];
				game1Y = playoffTeams[6];

				if (Random.Range(0, game1X.strength) > Random.Range(0, game1Y.strength))
				{
					playoffTeams[7] = game1X;
				}
				else
				{
					playoffTeams[7] = game1Y;
				}

				for (int i = 0; i < 9; i++)
				{
					brackDisplay[i].rank.text = playoffTeams[i].rank.ToString();
					brackDisplay[i].name.text = playoffTeams[i].name;
					brackDisplay[i].name.transform.parent.gameObject.SetActive(true);
				}
				StartCoroutine(RefreshPlayoffPanel());

				playoffRound++;
				simButton.gameObject.SetActive(false);
				contButton.gameObject.SetActive(true);
				break;

			default:
				break;

		}
		SetPlayoffs(playoffRound);
		yield break;
		//SetPlayoffs();
	}

}