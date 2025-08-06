using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HologramBehaviour : MonoBehaviour
{
    LoopManager loopManager;
    Animator anim;

    public RoomState type;
    public Transform startPos;
    public Transform? target;
    public bool roomClear = false;
    public bool played = false;

    #region Livingroom
    bool moveToTarget = false;
    public float rotationSpeed = 5f;  // 회전 속도
    public float moveSpeed = 2f;      // 이동 속도
    #endregion

    void Start()
    {
        loopManager = GameManager.Instance.loopManager;
        anim = GetComponent<Animator>();    
    }

    void Update()
    {
        switch (type)
        {
            case RoomState.Bedroom:
                if (roomClear && !played)
                {
                    GameManager.Instance.loopManager.roomState = RoomState.Livingroom;
                    anim.SetTrigger("disappear");
                    played = true;
                }
                break;
            case RoomState.Livingroom:
                if (moveToTarget)
                {
                    // 1. 목표 방향 계산 (y축 제외)
                    Vector3 direction = target.position - transform.position;
                    direction.y = 0f;

                    if (direction.magnitude > 0.1f)
                    {
                        // 2. 목표 회전
                        Quaternion targetRot = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(
                            transform.rotation,
                            targetRot,
                            rotationSpeed * Time.deltaTime
                        );

                        // 3. 전진 이동
                        transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    }
                    else
                    {
                        // 목표 도착 시
                        moveToTarget = false;

                        // 목표 transform의 Y 회전값으로 설정
                        Vector3 targetEuler = target.rotation.eulerAngles;
                        transform.rotation = Quaternion.Euler(0f, targetEuler.y, 0f);

                        // 앉기 애니메이션 실행
                        anim.SetTrigger("sitting");
                    }
                   
                }
                if (roomClear && !played)
                {
                    GameManager.Instance.loopManager.roomState = RoomState.Bathroom;
                    anim.SetTrigger("disappear");
                    played = true;
                }
                break;
            case RoomState.Bathroom:
                if (roomClear && !played)
                {
                    anim.SetTrigger("disappear");
                    played = true;
                }
                break;
        }

        
    }


    public void LocateToStartPosition()
    {
        StartCoroutine(LocateNextFrame());
    }

    IEnumerator LocateNextFrame()
    {
        anim.applyRootMotion = false;
        yield return null; // 다음 프레임까지 대기
        transform.position = startPos.position;
        transform.rotation = startPos.rotation;
        anim.applyRootMotion = true;
    }

    
    public void OffThisHologram()
    {
        if (type != RoomState.Bathroom) GameManager.Instance.loopManager.hologramBehaviours[(int)type + 1].gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ResetAnimationState()
    {
        LocateToStartPosition();
        anim.Rebind();         // 애니메이터 초기화
        anim.Update(0f);       // 즉시 상태 갱신 (프레임 반영)
    }


    public void WalkToSofa()
    {
        moveToTarget = true;
        anim.SetTrigger("walking");
    }
}
