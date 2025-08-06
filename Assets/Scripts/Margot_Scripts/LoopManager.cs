using EPOOutline;
using Margot;
using System.Collections;
using UnityEngine;

public enum RoomState { Bedroom, Livingroom, Bathroom }
public class LoopManager : MonoBehaviour
{
    bool playAudio = false;
    public int currentLoop = 0;
    public bool loopStarted = false;
    public bool endingReady = false;
    public bool waitForCalling = false;

    public Animation closingEyesAnim;

    public bool[] isLoopClear;
    public int loopMax = 3;
    public RoomState roomState = RoomState.Bedroom;

    public CameraController camController;
    public GameObject player;
    public HologramBehaviour[] hologramBehaviours; // 각 방마다 존재하는 플레이여 형체의 홀로그램
    public Material nextDateMat;
    public GameObject calendarObj;
    public GameObject phoneObj;

    public GameObject loopSecondBGMusicObj;

    [Header("Bedroom")]
    public GameObject[] bedroom_negativeObjs;
    public GameObject[] bedroom_positiveObjs;
    public GameObject bedroom_objParticle;
    [Header("Livingroom")]
    public GameObject[] livingroom_negativeObjs;
    public GameObject[] livingroom_positiveObjs;
    public GameObject livingroom_objParticle;
    [Header("Bathroom")]
    public GameObject[] bathroom_negativeObjs;
    public GameObject[] bathroom_positiveObjs;
    public GameObject bathroom_objParticle;


    public bool loopEnd = false; // 튜토리얼 루프가 끝나거나, 부정적 행동으로 루프가 반복될 때

    void Start()
    {
        isLoopClear = new bool[loopMax];
    }

    void Update()
    {
       
        if (loopEnd)
        {
            if (currentLoop == 0)
            {
                RestartLoop(false);
            }
            else
            {
                RestartLoop(true);
            }
        }


        if (endingReady && !waitForCalling)
        {
            //StartCoroutine(WaitForCall(false));
            StartCoroutine(WaitForSecond(5f));
            waitForCalling = true;
        }

    }
    IEnumerator WaitForSecond(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        phoneObj.GetComponent<RingingPhone>().RingThePhone();
    }

    IEnumerator WaitForCall(bool skippLetter)
    {
        float elapsedTime = 0f;
        float timeout = 10f;

        while (elapsedTime < timeout && !skippLetter)
        {
            if (GameManager.Instance.interactionManager.readLetter) // 조건이 참이 되면 코루틴 종료
            {
                StartCoroutine(WaitForCall(true));
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 전화벨 울리기
        Debug.Log("ringing ringing");

        phoneObj.GetComponent<RingingPhone>().RingThePhone();

        yield break;
    } // not used

    public void RestartLoop(bool repeat)
    {
        roomState = RoomState.Bedroom;                    // 다시 침실에서 시작
        
        StartCoroutine(EndOfLoop(repeat));

       
    }

    IEnumerator EndOfLoop(bool repeat)
    {
        closingEyesAnim.Play();
        if (!playAudio)
        {
            GameManager.Instance.audioManager.PlayCloseEyes();
            GameManager.Instance.audioManager.PlayNegativeInteraction();
            playAudio = true;
        }
        yield return new WaitForSeconds(2f);

        if (!repeat)
        {
            currentLoop++;
            loopSecondBGMusicObj.SetActive(true);
            calendarObj.GetComponent<MeshRenderer>().material = nextDateMat;
        }
        loopStarted = true;
        loopEnd = false;
        player.GetComponent<PlayerMove>().LoopEnd();
        foreach(HologramBehaviour item in hologramBehaviours)
        {
            item.gameObject.SetActive(false);
            item.gameObject.transform.position = item.startPos.position;
            item.roomClear = false;
            item.played = false;
        }
        //hologramBehaviours[(int)roomState].gameObject.SetActive(false);
        //hologramBehaviours[(int)roomState].gameObject.transform.position = hologramBehaviours[(int)roomState].startPos.position;

        foreach (GameObject obj in GameManager.Instance.interactionManager.doors)
        {
            obj.transform.GetChild(0).transform.localRotation = Quaternion.Euler(Vector3.zero);
            obj.GetComponent<Door>().isOpen = false;
        }

        camController.SetActiveRoom("Bedroom");
        ResetRoom();
        for (int i = 0; i < loopMax; i++) isLoopClear[i] = false;
        GameManager.Instance.interactionManager.LoopEnd();
        playAudio = false;
        yield break;
    }

    public void ResetRoom()
    {
        bedroom_objParticle.SetActive(true);
        livingroom_objParticle.SetActive(false);
        bathroom_objParticle.SetActive(false);


        foreach (GameObject obj in bedroom_negativeObjs)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in bedroom_positiveObjs)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in GameManager.Instance.interactionManager.bedroom_interactableObjs)
        {
            obj.GetComponent<ObjectInteract>().enabled = true;
            obj.GetComponent<Outlinable>().enabled = true;
        }

        foreach (GameObject obj in livingroom_negativeObjs)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in livingroom_positiveObjs)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in GameManager.Instance.interactionManager.livingroom_interactableObjs)
        {
            obj.GetComponent<ObjectInteract>().enabled = true;
            obj.GetComponent<Outlinable>().enabled = true;
        }

        foreach (GameObject obj in bathroom_negativeObjs)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in bathroom_positiveObjs)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in GameManager.Instance.interactionManager.bathroom_interactableObjs)
        {
            obj.GetComponent<ObjectInteract>().enabled = true;
            obj.GetComponent<Outlinable>().enabled = true;
        }
    }
    public void ClearRoom(RoomState targetRoom)
    {
        if (GameManager.Instance.interactionManager.isGrabbing)
        {
            GameManager.Instance.interactionManager.grabbedObj.SetActive(false);
            GameManager.Instance.interactionManager.isGrabbing = false;
        }

        switch (targetRoom)
        {
            case RoomState.Bedroom:
                isLoopClear[0] = true;
                bedroom_objParticle.SetActive(false);
                livingroom_objParticle.SetActive(true);
                foreach (GameObject obj in bedroom_negativeObjs)
                {
                    obj.SetActive(false);
                }
                foreach(GameObject obj in bedroom_positiveObjs)
                {
                    obj.SetActive(true);
                }
                foreach(GameObject obj in GameManager.Instance.interactionManager.bedroom_interactableObjs)
                {
                    obj.GetComponent<ObjectInteract>().enabled = false;
                    obj.GetComponent<Outlinable>().enabled = false;
                }
                break;
            case RoomState.Livingroom:
                isLoopClear[1] = true;
                bathroom_objParticle.SetActive(true);
                livingroom_objParticle.SetActive(false);
                foreach (GameObject obj in livingroom_negativeObjs)
                {
                    obj.SetActive(false);
                }
                foreach (GameObject obj in livingroom_positiveObjs)
                {
                    obj.SetActive(true);
                }
                foreach (GameObject obj in GameManager.Instance.interactionManager.livingroom_interactableObjs)
                {
                    obj.GetComponent<ObjectInteract>().enabled = false;
                    obj.GetComponent<Outlinable>().enabled = false;
                }
                break;
            case RoomState.Bathroom:
                bathroom_objParticle.SetActive(false);
                isLoopClear[2] = true;
                endingReady = true;
                foreach (GameObject obj in bathroom_negativeObjs)
                {
                    obj.SetActive(false);
                }
                foreach (GameObject obj in bathroom_positiveObjs)
                {
                    obj.SetActive(true);
                }
                foreach (GameObject obj in GameManager.Instance.interactionManager.bathroom_interactableObjs)
                {
                    obj.GetComponent<ObjectInteract>().enabled = false;
                    obj.GetComponent<Outlinable>().enabled = false;
                }
                break;
        }
    }

}
