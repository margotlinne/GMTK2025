using Margot;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class CutSceneManager : MonoBehaviour
{
    public bool cutsceneStarted = false;
    public bool readyToEnd = false;
    public GameObject player;
    public GameObject frontDoorShine;
    public Transform cutsceneStartPos;
    public Transform cutsceneEndPos;
    public Camera lenseCam;
    public GameObject whiteInEffect;
    public GameObject manCrying;


    void Start()
    {

        frontDoorShine.SetActive(false);
    }

    public void StartCutScene(AudioSource audioSource)
    {
        // player.GetComponent<PlayerInput>().enabled = false;
        player.GetComponent<PlayerMove>().enabled = false;
        // player.GetComponent<PlayerAnimation>().anim.speed = 0f;
        StartCoroutine(WaitForEndOfCall(audioSource));
        manCrying.GetComponent<AudioSource>().Play();
    }




    IEnumerator WaitForEndOfCall(AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        manCrying.GetComponent<Animation>().Play();

        int layer = LayerMask.NameToLayer("Player");
        lenseCam.cullingMask &= ~(1 << layer);

        GameManager.Instance.loopManager.camController.enabled = false;

        cutsceneStarted = true;

        // 카메라 문 쪽으로 이동
        lenseCam.transform.position = cutsceneStartPos.position;

        // 타겟으로 이동 + 자연스러운 바라보기
        while (Vector3.Distance(lenseCam.transform.position, cutsceneEndPos.position) > 0.1f)
        {
            Vector3 dir = (cutsceneEndPos.position - lenseCam.transform.position).normalized;

            // 이동
            lenseCam.transform.position = Vector3.MoveTowards(lenseCam.transform.position, cutsceneEndPos.position, 2f * Time.deltaTime);

            // 이동 중 회전 (Smooth)
            Quaternion lookRot = Quaternion.LookRotation(dir);
            lenseCam.transform.rotation = Quaternion.Slerp(lenseCam.transform.rotation, lookRot, Time.deltaTime * 3f);

            yield return null;
        }

        // 도착 후 최종 회전 보간
        Quaternion startRot = lenseCam.transform.rotation;
        Quaternion endRot = cutsceneEndPos.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            float smoothT = Mathf.SmoothStep(0f, 1f, t); // 부드러운 감속 보간
            lenseCam.transform.rotation = Quaternion.Slerp(startRot, endRot, smoothT);
            yield return null;
        }

        lenseCam.transform.rotation = endRot;

        readyToEnd = true;
        yield break;
    }


    // 문 밖으로 나가며 화이트 페이드인, ui 텍스트 효과
    public void LeaveTheHouse()
    {

        frontDoorShine.SetActive(true);

        StartCoroutine(MoveOut());
    }

    IEnumerator MoveOut()
    {

        // manCrying.Stop();
        float moveDuration = 2f; // 이동 시간
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            lenseCam.transform.position += lenseCam.transform.forward * 3f * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null; // 프레임마다 반복
        }

        yield return new WaitForSeconds(2f);
        whiteInEffect.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        Debug.Log("게임 끝났다아아아아");
        SceneManager.LoadScene(2);

        yield break;
    }
}
