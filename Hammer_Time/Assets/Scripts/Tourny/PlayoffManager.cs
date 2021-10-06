using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TigerForge;

public class PlayoffManager : MonoBehaviour
{
	public TournyManager tm;
	public Team[] playoffTeams;

	public BracketDisplay[] brackDisplay;
	public GameObject[] row;
	public GameObject playoffs;
	public Button simButton;
	public Button contButton;
	public Button playButton;
	public Text heading;
	public Scrollbar scrollBar;
	public Text careerEarningsText;

	GameSettingsPersist gsp;

	EasyFileSave myFile;
	int pTeams;
	public int playerTeam;
	public int oppTeam;
	public int playoffRound;

	public float careerEarnings;
	public Vector2Int careerRecord;
	private void Start()
	{
		gsp = FindObjectOfType<GameSettingsPersist>();

		myFile = new EasyFileSave("my_player_data");

		StartCoroutine(LoadCareer());
		Debug.Log("Career Earnings before playoffs - $ " + careerEarnings.ToString());
		playoffs.SetActive(true);

		playerTeam = tm.playerTeam;
		playoffRound = gsp.playoffRound;
		if (gsp.playoffRound > 0)
			LoadPlayoffs(playoffRound);
		else
			SetSeeding(tm.teams.Length);
	}

	public void SetSeeding(int numberOfTeams)
    {

		pTeams = 4;
		playoffTeams = new Team[9];
		heading.text = "Page Playoff";

		for (int i = 0; i < pTeams; i++)
		{
			playoffTeams[i] = tm.teamList[i].team;
			brackDisplay[i].name.text = playoffTeams[i].name;
			brackDisplay[i].rank.text = playoffTeams[i].rank.ToString();
		}
		tm.playoffRound++;
		playoffRound++;
		SetPlayoffs(playoffRound);
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

				for (int i = 0; i < 4; i++)
				{
					if (tm.teams[playerTeam] == playoffTeams[i])
					{
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
		Debug.Log("Set Playoffs - Round " + playoffRound);
		switch (playoffRound)
        {
			case 1:

                switch (tm.teams[tm.playerTeam].rank)
                {
                    case 1:
                        playButton.gameObject.SetActive(true);
                        tm.vsDisplay[1].name.text = playoffTeams[1].name;
                        tm.vsDisplay[1].rank.text = playoffTeams[1].rank.ToString();
                        break;
                    case 2:
                        playButton.gameObject.SetActive(true);
                        tm.vsDisplay[1].name.text = playoffTeams[0].name;
                        tm.vsDisplay[1].rank.text = playoffTeams[0].rank.ToString();
                        break;
                    case 3:
                        playButton.gameObject.SetActive(true);
                        tm.vsDisplay[1].name.text = playoffTeams[3].name;
                        tm.vsDisplay[1].rank.text = playoffTeams[3].rank.ToString();
                        break;
					case 4:
						playButton.gameObject.SetActive(true);
						tm.vsDisplay[1].name.text = playoffTeams[2].name;
						tm.vsDisplay[1].rank.text = playoffTeams[2].rank.ToString();
						break;
					default:
                        playButton.gameObject.SetActive(false);
                        tm.vs.SetActive(false);
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
					row[i].SetActive(true);
				}

                //brackDisplay[4].name.text = playoffTeams[4].name;
                //brackDisplay[4].rank.text = playoffTeams[4].rank.ToString();


				if (playoffTeams[4].name == tm.teams[tm.playerTeam].name)
                {
                    playButton.gameObject.SetActive(false);

                    tm.vsDisplay[0].name.text = playoffTeams[4].name;
                    tm.vsDisplay[0].rank.text = playoffTeams[4].rank.ToString();
                    tm.vsDisplay[1].name.text = "BYE TO FINALS";
                    tm.vsDisplay[1].rank.text = "-";
                }
                else if (playoffTeams[5].name == tm.teams[tm.playerTeam].name)
                {
                    playButton.gameObject.SetActive(true);
                    tm.vsDisplay[0].name.text = playoffTeams[5].name;
                    tm.vsDisplay[0].rank.text = playoffTeams[5].rank.ToString();
                    tm.vsDisplay[1].name.text = playoffTeams[6].name;
                    tm.vsDisplay[1].rank.text = playoffTeams[6].rank.ToString();
                }
				else if (playoffTeams[6].name == tm.teams[tm.playerTeam].name)
                {
					playButton.gameObject.SetActive(true);
					tm.vsDisplay[0].name.text = playoffTeams[6].name;
					tm.vsDisplay[0].rank.text = playoffTeams[6].rank.ToString();
					tm.vsDisplay[1].name.text = playoffTeams[5].name;
					tm.vsDisplay[1].rank.text = playoffTeams[5].rank.ToString();
				}
                else
                {
                    tm.vs.SetActive(false);
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
					row[i].SetActive(true);
				}

				if (playoffTeams[4].name == tm.teams[tm.playerTeam].name)
				{
					playButton.gameObject.SetActive(false);

					tm.vsDisplay[0].name.text = playoffTeams[4].name;
					tm.vsDisplay[0].rank.text = playoffTeams[4].rank.ToString();
					tm.vsDisplay[1].name.text = playoffTeams[7].name;
					tm.vsDisplay[1].rank.text = playoffTeams[7].rank.ToString();
				}
				else if (playoffTeams[7].name == tm.teams[tm.playerTeam].name)
				{
					playButton.gameObject.SetActive(true);
					tm.vsDisplay[0].name.text = playoffTeams[7].name;
					tm.vsDisplay[0].rank.text = playoffTeams[7].rank.ToString();
					tm.vsDisplay[1].name.text = playoffTeams[4].name;
					tm.vsDisplay[1].rank.text = playoffTeams[4].rank.ToString();
				}
				else
				{
					tm.vs.SetActive(false);
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
					//brackDisplay[i].name.transform.parent.gameObject.SetActive(true);
					row[i].SetActive(true);
				}

                playoffs.SetActive(true);
                StartCoroutine(RefreshPlayoffPanel());

				if (tm.teams[tm.playerTeam].name == playoffTeams[8].name)
				{
					heading.text = "You Win!";

					careerEarnings += 70000f;
				}
				else if (tm.teams[tm.playerTeam].name == playoffTeams[4].name | tm.teams[tm.playerTeam].name == playoffTeams[7].name)
				{
					heading.text = "Runner-up";
					careerEarnings += 35000f;
				}
				else if (tm.teams[tm.playerTeam].name == playoffTeams[5].name | tm.teams[tm.playerTeam].name == playoffTeams[6].name)
				{
					heading.text = "3rd Place";
					careerEarnings += 20000f;
				}
				else if (tm.teams[tm.playerTeam].name == playoffTeams[2].name | tm.teams[tm.playerTeam].name == playoffTeams[3].name)
				{
					heading.text = "4th Place";
					careerEarnings += 10000f;
				}
                else
                {
                    for (int i = 4; i < tm.teamList.Count; i++)
					{
						float p = 1.4f;
						float totalTeams = tm.teamList.Count - 4;
						float prizePayout = ((Mathf.Pow(p, totalTeams - (i + 1))) / (Mathf.Pow(p, totalTeams) - 1f)) * 15000f * (p - 1);

						Debug.Log("Position " + (i + 1) + " Payout is $" + prizePayout);
						if (tm.teams[tm.playerTeam].name == tm.teamList[i].team.name)
						{
							if (i > 3)
								heading.text = (i + 1) + "th Place";

							//float prizePayout = (totalTeams - i) / (totalTeams);
							//float prizePayout = ((1 - p) / Mathf.Pow(1 - p, totalTeams) * Mathf.Pow(p, (i - 1))) * 10000f;
							//float prizePayout = ((Mathf.Pow(p, totalTeams - (i + 1))) / (Mathf.Pow(p, totalTeams) - 1f)) * 10000f * (p - 1);

							Debug.Log("Prize Payout multiplier is " + prizePayout);
							prizePayout = Mathf.RoundToInt(prizePayout);
							careerEarnings += prizePayout;
						}
					}
                }
                Debug.Log("Career Earnings after calculation - " + careerEarnings.ToString());
				careerEarningsText.text = "$ " + careerEarnings.ToString();
				
				careerRecord = new Vector2Int(careerRecord.x + tm.teams[playerTeam].wins, careerRecord.y + tm.teams[playerTeam].loss);

				StartCoroutine(SaveCareer());
				//heading.text = "So Close!";
				tm.vs.SetActive(false);
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
					row[i].SetActive(true);
				}
				StartCoroutine(RefreshPlayoffPanel());
				tm.playoffRound++;
				simButton.gameObject.SetActive(false);
				contButton.gameObject.SetActive(true);
				SetPlayoffs(tm.playoffRound);
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
					row[i].SetActive(true);
				}
				StartCoroutine(RefreshPlayoffPanel());
				tm.playoffRound++;
				simButton.gameObject.SetActive(false);
				contButton.gameObject.SetActive(true);
				SetPlayoffs(tm.playoffRound);
				break;

			case 3:
				game1X = playoffTeams[4];
				game1Y = playoffTeams[7];

				if (Random.Range(0, game1X.strength) > Random.Range(0, game1Y.strength))
				{
					playoffTeams[8] = game1X;
				}
				else
				{
					playoffTeams[8] = game1Y;
				}

				for (int i = 0; i < 9; i++)
				{
					brackDisplay[i].rank.text = playoffTeams[i].rank.ToString();
					brackDisplay[i].name.text = playoffTeams[i].name;
					brackDisplay[i].name.transform.parent.gameObject.SetActive(true);
					row[i].SetActive(true);
				}
				StartCoroutine(RefreshPlayoffPanel());
				tm.playoffRound++;
				simButton.gameObject.SetActive(false);
				contButton.gameObject.SetActive(true);
				SetPlayoffs(tm.playoffRound);
				break;

			default:
				SetPlayoffs(tm.playoffRound);
				break;

		}
		//SetPlayoffs(tm.playoffRound);
		yield break;
		//SetPlayoffs();
	}

	IEnumerator LoadCareer()
    {
		if (myFile.Load())
		{
			careerEarnings = myFile.GetFloat("Career Earnings", 0f);
			Vector2 tempRecord = myFile.GetUnityVector2("Career Record");
			careerRecord = new Vector2Int((int)tempRecord.x, (int)tempRecord.y);
			Debug.Log("Loading Career Earnings - $ " + careerEarnings);
			myFile.Dispose();
		}

		yield return new WaitForEndOfFrame();

		careerEarningsText.text = "$ " + careerEarnings.ToString();
	}

	IEnumerator SaveCareer()
    {
		//myFile = new EasyFileSave("my_player_data");
		myFile.Add("Career Record", careerRecord);
		Vector2 tempRecord = new Vector2(careerRecord.x, careerRecord.y);
		myFile.Add("Career Earnings", tempRecord);
		yield return myFile.Save();
	}
}