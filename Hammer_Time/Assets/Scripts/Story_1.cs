using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story_1 : MonoBehaviour
{
    public GameManager gm;
    public CameraManager cm;
    public SweeperManager sm;
    public DialogueManager dm;
    public StoryManager storyM;
    public GameHUD gHUD;

    public DialogueTrigger skipDialogue;
    public DialogueTrigger annDialogue;
    public AI_Target aiTarg;
    public AI_Shooter aiShoot;
    public GameObject dialogueGO;

    public float timeScale;

    public GameObject targetStory;
    public GameObject targetPullback;
    public GameObject targetPlayer;
    public GameObject targetAi;

    public Vector2[] rockPos;
    public Vector2[] targetStoryPos;
    public Vector2[] targetAiPos;
    public Vector2[] targetPullbackPos;

    Rock_Flick rockFlick;
    Rigidbody2D rockRB;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StoryFlow());
    }
    private void Update()
    {
        storyM.rockPos = rockPos;
        storyM.targetPullbackPos = targetPullbackPos;
        storyM.targetAiPos = targetAiPos;
        storyM.targetStoryPos = targetStoryPos;
    }
    IEnumerator StoryFlow()
    {
        //for (int i = 0; i < rockPos.Length; i++)
        //{
        //    storyM.rockPos[i] = rockPos[i];
        //}
        //for (int i = 0; i < targetAiPos.Length; i++)
        //{
        //    storyM.targetAiPos[i] = targetAiPos[i];
        //}
        //for (int i = 0; i < targetStoryPos.Length; i++)
        //{
        //    storyM.targetStoryPos[i] = targetStoryPos[i];
        //}
        //for (int i = 0; i < targetPullbackPos.Length; i++)
        //{
        //    storyM.targetPullbackPos[i] = targetPullbackPos[i];
        //}

        yield return new WaitUntil(() => gm.rockCurrent == 7);
        
        storyM.SyncToGm();
        yield return StartCoroutine(Shot8());

        yield return new WaitUntil(() => gm.rockCurrent == 8);
        storyM.SyncToGm();
        yield return StartCoroutine(Shot9());

        yield return new WaitUntil(() => gm.rockCurrent == 9);
        storyM.SyncToGm();
        yield return StartCoroutine(Shot10());

        yield return new WaitUntil(() => gm.rockCurrent == 10);
        storyM.SyncToGm();
        yield return StartCoroutine(Shot11());

        yield return new WaitUntil(() => gm.rockCurrent == 11);
        storyM.SyncToGm();
        yield return StartCoroutine(Shot12());

        yield return new WaitUntil(() => gm.rockCurrent == 12);
        storyM.SyncToGm();
        yield return StartCoroutine(Shot13());

        yield return new WaitUntil(() => gm.rockCurrent == 13);
        storyM.SyncToGm();
        yield return StartCoroutine(Shot14());

        yield return new WaitUntil(() => gm.rockCurrent == 14);
        storyM.SyncToGm();
        yield return StartCoroutine(Shot15());

        yield return new WaitUntil(() => gm.rockCurrent == 15);
        storyM.SyncToGm();
        yield return StartCoroutine(Shot16());
        storyM.SyncToGm();
    }

    IEnumerator Shot8()
    {
        rockRB = gm.rockList[gm.rockCurrent].rock.GetComponent<Rigidbody2D>();
        gHUD.mainDisplay.enabled = false;
        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(0);

        yield return new WaitUntil(() => dialogueGO.activeSelf == false);

        targetAi.transform.position = targetAiPos[0];
        gm.rm.inturn = true;
        aiTarg.OnTarget("Manual Draw", gm.rockCurrent, 0);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= -7f);
        sm.SweepWeight(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 0f);
        if (rockRB.velocity.y >= 3)
            sm.SweepWhoa(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 1.5f);
        Debug.Log("y = 1.5 velocity - " + rockRB.velocity.x + ", " + rockRB.velocity.y);
        if (rockRB.velocity.y >= 2.4f)
            sm.SweepLeft(true);
        else
            sm.SweepWhoa(true);
        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 3f);
        Debug.Log("y = 3 velocity - " + rockRB.velocity.x + ", " + rockRB.velocity.y);
        sm.SweepWhoa(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rockInfo.rest == true);

        gm.gHUD.mainDisplay.enabled = false;
        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(1);
    }

    IEnumerator Shot9()
    {
        yield return new WaitUntil(() => gm.rockCurrent == 8);
        rockFlick = gm.rockList[8].rock.GetComponent<Rock_Flick>();
        rockRB = gm.rockList[gm.rockCurrent].rock.GetComponent<Rigidbody2D>();

        cm.RockZoom(gm.rockList[5].rock.transform);
        dialogueGO.SetActive(true);
        skipDialogue.gameObject.SetActive(true);
        skipDialogue.TriggerDialogue(0);
        targetStory.SetActive(true);
        targetStory.transform.position = gm.rockList[5].rock.transform.position;
        targetPullback.transform.position = targetPullbackPos[0];
        targetPullback.SetActive(true);

        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
        cm.ShotSetup();
        dialogueGO.SetActive(true);
        skipDialogue.gameObject.SetActive(true);
        skipDialogue.TriggerDialogue(1);
        dm.contButton.SetActive(false);

        yield return new WaitUntil(() => targetPullback.gameObject.GetComponent<TutorialTrajectory>().distance <= 0.15f);

        dialogueGO.SetActive(true);
        skipDialogue.gameObject.SetActive(true);
        skipDialogue.TriggerDialogue(2);
        dm.contButton.SetActive(false);

        rockFlick.story = true;
        yield return new WaitUntil(() => rockFlick.isPressed == true);

        yield return new WaitUntil(() => rockFlick.GetComponent<SpringJoint2D>().dampingRatio == 1f);

        rockFlick.story = false;
        targetAi.transform.position = targetPullbackPos[0];
        aiTarg.OnTarget("Manual Tap Back", gm.rockCurrent, 0);
        yield return new WaitUntil(() => gm.rockList[8].rockInfo.shotTaken == true);

        dm.DisplayNextSentence();
        dm.contButton.SetActive(true);
        dialogueGO.SetActive(false);
        targetStory.SetActive(false);
        targetPullback.SetActive(false);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= -3f);
        sm.SweepWeight(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 0f);
        if (rockRB.velocity.y >= 3.2f)
        {
            sm.SweepWhoa(true);
            sm.SweepRight(true);
        }

        //if (rockRB.velocity.x <= -0.025)
        //    sm.SweepRight(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 3f);
        sm.SweepWhoa(true);

        yield return new WaitUntil(() => gm.rockList[8].rockInfo.rest);

        dialogueGO.SetActive(true);
        skipDialogue.gameObject.SetActive(true);
        skipDialogue.TriggerDialogue(3);
        yield return new WaitUntil(() => dialogueGO.activeSelf == false);

        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(2);
        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
    }

    IEnumerator Shot10()
    {
        yield return new WaitUntil(() => gm.rockCurrent == 9);
        rockRB = gm.rockList[gm.rockCurrent].rock.GetComponent<Rigidbody2D>();
        targetAi.transform.position = targetAiPos[1];
        gm.rm.inturn = true;
        aiTarg.OnTarget("Manual Draw", gm.rockCurrent, 0);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= -7f);
        Debug.Log("y = -7 velocity - " + rockRB.velocity.x + ", " + rockRB.velocity.y);
        if (rockRB.velocity.y >= 5f)
            sm.SweepRight(true);
        else
            sm.SweepWeight(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= -1f);
        Debug.Log("y = -1 velocity - " + rockRB.velocity.x + ", " + rockRB.velocity.y);
        if (rockRB.velocity.y >= 2.7f)
            sm.SweepRight(true);
        else
            sm.SweepWhoa(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 0f);
        sm.SweepWhoa(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rockInfo.rest);
        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(2);
        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
    }

    IEnumerator Shot11()
    {
        yield return new WaitUntil(() => gm.rockCurrent == 10);
        rockFlick = gm.rockList[gm.rockCurrent].rock.GetComponent<Rock_Flick>();
        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(2);


        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
        cm.RockZoom(gm.rockList[8].rock.transform);
        dialogueGO.SetActive(true);
        skipDialogue.gameObject.SetActive(true);
        skipDialogue.TriggerDialogue(2);
        targetStory.SetActive(true);
        targetStory.transform.position = gm.rockList[8].rock.transform.position;
        targetPullback.transform.position = new Vector2(targetPullbackPos[1].x + gm.rockList[8].rock.transform.position.x, targetPullbackPos[1].y);
        targetPullback.SetActive(true);

        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
        cm.ShotSetup();

        yield return new WaitUntil(() => targetPullback.gameObject.GetComponent<TutorialTrajectory>().distance <= 0.15f);

        dialogueGO.SetActive(true);
        skipDialogue.gameObject.SetActive(true);
        skipDialogue.TriggerDialogue(1);
        dm.contButton.SetActive(false);

        rockFlick.story = true;
        yield return new WaitUntil(() => rockFlick.isPressed == true);

        yield return new WaitUntil(() => rockFlick.GetComponent<SpringJoint2D>().dampingRatio == 1f);

        rockFlick.story = false;
        targetPlayer.transform.position = targetPullback.transform.position;
        aiTarg.OnTarget("Manual Take Out", gm.rockCurrent, 0);
        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rockInfo.shotTaken == true);

        dm.DisplayNextSentence();
        dm.contButton.SetActive(true);
        dialogueGO.SetActive(false);
        targetStory.SetActive(false);
        targetPullback.SetActive(false);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rockInfo.rest);
        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(2);
        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
    }

    IEnumerator Shot12()
    {
        yield return new WaitUntil(() => gm.rockCurrent == 11);

        targetAi.transform.position = targetAiPos[2];
        gm.rm.inturn = true;
        //aiShoot.OnShot("Top Four Foot", gm.rockCurrent);
        aiTarg.OnTarget("Manual Draw", gm.rockCurrent, 0);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= -7f);
        sm.SweepWeight(true);

        //yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= -3f);
        //sm.SweepHard(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 0f);
        sm.SweepWhoa(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 1f);
        sm.SweepLeft(true);


        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rockInfo.rest);
        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(2);
        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
    }

    IEnumerator Shot13()
    {
        yield return new WaitUntil(() => gm.rockCurrent == 12);
        rockFlick = gm.rockList[gm.rockCurrent].rock.GetComponent<Rock_Flick>();
        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(2);

        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
        cm.RockZoom(gm.rockList[2].rock.transform);
        dialogueGO.SetActive(true);
        skipDialogue.gameObject.SetActive(true);
        skipDialogue.TriggerDialogue(2);
        targetStory.SetActive(true);
        targetStory.transform.position = gm.rockList[2].rock.transform.position;
        targetPullback.transform.position = new Vector2(targetPullbackPos[2].x + gm.rockList[2].rock.transform.position.x, targetPullbackPos[2].y);
        targetPullback.SetActive(true);

        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
        cm.ShotSetup();

        yield return new WaitUntil(() => targetPullback.gameObject.GetComponent<TutorialTrajectory>().distance <= 0.15f);

        dialogueGO.SetActive(true);
        skipDialogue.gameObject.SetActive(true);
        skipDialogue.TriggerDialogue(1);
        dm.contButton.SetActive(false);

        rockFlick.story = true;
        yield return new WaitUntil(() => rockFlick.isPressed == true);

        yield return new WaitUntil(() => rockFlick.GetComponent<SpringJoint2D>().dampingRatio == 1f);
        rockFlick.story = false;
        targetPlayer.transform.position = targetPullback.transform.position;
        aiTarg.OnTarget("Manual Take Out", gm.rockCurrent, 0);
        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rockInfo.shotTaken == true);

        dm.DisplayNextSentence();
        dm.contButton.SetActive(true);
        dialogueGO.SetActive(false);
        targetStory.SetActive(false);
        targetPullback.SetActive(false);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rockInfo.rest);
        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(2);
        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
    }

    IEnumerator Shot14()
    {
        yield return new WaitUntil(() => gm.rockCurrent == 13);
        rockRB = gm.rockList[gm.rockCurrent].rock.GetComponent<Rigidbody2D>();
        targetAi.transform.position = targetAiPos[3];
        gm.rm.inturn = true;
        aiTarg.OnTarget("Manual Draw", gm.rockCurrent, 0);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= -7f);
        sm.SweepWeight(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 0f);
        if (rockRB.velocity.y >= 3)
            sm.SweepWhoa(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 1.5f);
        Debug.Log("y = 1.5 velocity - " + rockRB.velocity.x + ", " + rockRB.velocity.y);
        if (rockRB.velocity.y >= 2.4f)
            sm.SweepLeft(true);
        else
            sm.SweepWeight(true);
        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 4f);
        sm.SweepWhoa(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rockInfo.rest);
        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(2);
        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
    }

    IEnumerator Shot15()
    {
        #region Shot 15
        yield return new WaitUntil(() => gm.rockCurrent == 14);
        rockFlick = gm.rockList[gm.rockCurrent].rock.GetComponent<Rock_Flick>();

        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(2);

        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
        cm.RockZoom(gm.rockList[13].rock.transform);
        dialogueGO.SetActive(true);
        skipDialogue.gameObject.SetActive(true);
        skipDialogue.TriggerDialogue(2);
        targetStory.SetActive(true);
        targetStory.transform.position = gm.rockList[13].rock.transform.position;
        targetPullback.transform.position = targetPullbackPos[3];
        targetPullback.SetActive(true);

        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
        cm.ShotSetup();

        yield return new WaitUntil(() => targetPullback.gameObject.GetComponent<TutorialTrajectory>().distance <= 0.15f);

        dialogueGO.SetActive(true);
        skipDialogue.gameObject.SetActive(true);
        skipDialogue.TriggerDialogue(1);
        dm.contButton.SetActive(false);

        rockFlick.story = true;
        yield return new WaitUntil(() => rockFlick.isPressed == true);

        yield return new WaitUntil(() => rockFlick.GetComponent<SpringJoint2D>().dampingRatio == 1f);

        rockFlick.story = false;
        targetAi.transform.position = targetPullbackPos[3];
        aiTarg.OnTarget("Manual Draw", gm.rockCurrent, 0);
        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rockInfo.shotTaken == true);

        dm.DisplayNextSentence();
        dm.contButton.SetActive(true);
        dialogueGO.SetActive(false);
        targetStory.SetActive(false);
        targetPullback.SetActive(false);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rockInfo.rest);
        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(2);
        yield return new WaitUntil(() => dialogueGO.activeSelf == false);

        #endregion
    }

    IEnumerator Shot16()
    {
        yield return new WaitUntil(() => gm.rockCurrent == 15);
        rockRB = gm.rockList[gm.rockCurrent].rock.GetComponent<Rigidbody2D>();

        if (gm.houseList[0].rockInfo.teamName != gm.rockList[gm.rockCurrent].rockInfo.teamName)
            targetAi.transform.position = new Vector2(gm.houseList[0].rock.transform.position.x, targetAiPos[4].y);
        else if (gm.houseList[1].rockInfo.teamName != gm.rockList[gm.rockCurrent].rockInfo.teamName)
        {
            if (Mathf.Abs(gm.houseList[0].rock.transform.position.x - gm.houseList[1].rock.transform.position.x) >= 0.6f)
                targetAi.transform.position = new Vector2(gm.houseList[1].rock.transform.position.x, targetAiPos[4].y);
            else
                targetAi.transform.position = new Vector2(0f, 6.5f);
        }
        else if (gm.houseList[2].rockInfo.teamName != gm.rockList[gm.rockCurrent].rockInfo.teamName)
        {
            if (Mathf.Abs(gm.houseList[0].rock.transform.position.x - gm.houseList[2].rock.transform.position.x) >= 0.6f)
                targetAi.transform.position = new Vector2(gm.houseList[1].rock.transform.position.x, targetAiPos[4].y);
            else
                targetAi.transform.position = new Vector2(0f, 6.5f);
        }
        else
            targetAi.transform.position = new Vector2(0f, 6.5f);

        gm.rm.inturn = true;
        aiTarg.OnTarget("Manual Draw", gm.rockCurrent, 0);
        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 0f);
        if (rockRB.velocity.y <= 2f)
            sm.SweepHard(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 1.5f);
        sm.SweepLeft(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rock.transform.position.y >= 3.5f);
        sm.SweepWhoa(true);

        yield return new WaitUntil(() => gm.rockList[gm.rockCurrent].rockInfo.rest);
        dialogueGO.SetActive(true);
        annDialogue.gameObject.SetActive(true);
        annDialogue.TriggerDialogue(2);
        yield return new WaitUntil(() => dialogueGO.activeSelf == false);
    }
}
