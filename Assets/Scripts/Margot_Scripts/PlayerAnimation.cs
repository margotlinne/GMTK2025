using System.Collections;
using UnityEngine;

namespace Margot
{
    public class PlayerAnimation : MonoBehaviour
    {
        [HideInInspector] public Animator anim;
        PlayerMove playerMove;
        PlayerInteraction interaction;
        public float moveSpeed = 3f;
        public float rotationSpeed = 3f;
        CapsuleCollider capsuleCollider;

        public bool isSitting = false;
        public bool isDrinking = false;
        public bool isGettingDrink = false;
        public bool isGettingPill = false;
        public bool isLayingDown = true;

        public GameObject snoringSound;
        //DD: code optimization
        private bool prevIsWalking = false;

        void Awake()
        {
            interaction = GetComponent<PlayerInteraction>();    
            anim = GetComponent<Animator>();
            playerMove = GetComponent<PlayerMove>();
            capsuleCollider = GetComponent<CapsuleCollider>();
        }

        void Update()
        {
            PlayWalkingAnimation();

            if (isLayingDown)
            {
                if (!snoringSound.activeSelf)
                {
                    snoringSound.SetActive(true);
                    snoringSound.GetComponent<AudioSource>().Play();
                }
            }
            else
            {
                if (snoringSound.activeSelf)
                {
                    snoringSound.GetComponent<AudioSource>().Stop();
                    snoringSound.SetActive(false);
                }
            }
        }

        public void PlayWalkingAnimation()
        {
            bool isMoving = playerMove.enabled && playerMove.GetMoveInput().sqrMagnitude > 0.01f;
            if (prevIsWalking != isMoving)
            {
                GetComponent<AudioSource>().mute = !isMoving;    
                anim.SetBool("isWalking", isMoving);
                prevIsWalking = isMoving;
            }
        }

        public void PlaySittingAnimation()
        {
            if (!isSitting)
            {
                GameManager.Instance.interactionManager.interactTarget.interacted = true;
                playerMove.dontMove = true;
                capsuleCollider.isTrigger = true;
                StartCoroutine(MoveAndRotateCoroutine(GameManager.Instance.interactionManager.interactTarget.transform.GetChild(0), true));
                playerMove.dontMove = true;
            }

           
        }

        public void PlayStandingUpAnimation()
        {
            anim.SetTrigger("standingUp");
            GameManager.Instance.audioManager.PlayStandUp();
            isSitting = false;
        }

        public void PlayDrinkingAnimation()
        {
            playerMove.dontMove = true;
            isDrinking = true;
            anim.SetTrigger("drinking");
            if (isGettingDrink) GameManager.Instance.audioManager.PlayDrinkBottle();
            if (isGettingPill) GameManager.Instance.audioManager.PlayTakePills();
        }

        public void PlayInteractAnimation()
        {
            playerMove.dontMove = true;
            if (isGettingDrink || isGettingPill)
            {
                anim.SetTrigger("takingOut");
                GameManager.Instance.audioManager.PlayPickUpBottle();
            }
            else anim.SetTrigger("interacting");
            GameManager.Instance.interactionManager.interactTarget.interacted = true;

            GameManager.Instance.interactionManager.drink.gameObject.SetActive(isGettingDrink);
            GameManager.Instance.interactionManager.pill.gameObject.SetActive(isGettingPill);
        }

        public void PlayGettingUpAnimation()
        {
            anim.SetTrigger("gettingUp");
            isLayingDown = false;
            GameManager.Instance.audioManager.PlayStandUp();
        }


        // 애니메이션 끝나면 호출
        public void StandingUpAnimationDone()
        {
            StartCoroutine(MoveAndRotateCoroutine(GameManager.Instance.interactionManager.interactTarget.transform.GetChild(1), false));
            playerMove.targetRotation = transform.rotation; // 애니메이션 끝나는 시점 각도 유지
            playerMove.rotationVelocity = 0f;
        }

        public void DrinkOrPillAnimationDone() 
        {
            isDrinking = false;

            if (isSitting) anim.SetTrigger("sitting");
            else
            {
                // playerMove.dontMove = false;
                anim.SetTrigger("standing");
            }

            GameManager.Instance.interactionManager.grabbedObj.SetActive(false);
            GameManager.Instance.interactionManager.isGrabbing = false;


            GameManager.Instance.loopManager.loopEnd = true;

        }

        public void InteractAnimationDone()
        {
            playerMove.dontMove = false;
            if (isGettingDrink || isGettingPill)
            {
                GameManager.Instance.interactionManager.grabbedObj.SetActive(true);
                GameManager.Instance.interactionManager.isGrabbing = true;
                if (GameManager.Instance.interactionManager.interactTarget.GetComponent<Animation>() != null)
                {
                    GameManager.Instance.interactionManager.interactTarget.GetComponent<Animation>().Play("fridge_closing");
                    GameManager.Instance.audioManager.PlayFridgeClose();
                }
                GameManager.Instance.interactionManager.interactTarget.interacted = false;
            }
        }

        public void GettingUpAnimationDone()
        {
            playerMove.characterController.enabled = true;
            playerMove.dontMove = false;
            GameManager.Instance.loopManager.loopStarted = false;
            GameManager.Instance.loopManager.hologramBehaviours[0].gameObject.SetActive(true);
            playerMove.targetRotation = transform.rotation;
            playerMove.rotationVelocity = 0f;
        }


        private IEnumerator MoveAndRotateCoroutine(Transform target, bool sitting)
        {
            playerMove.characterController.enabled = false;

            // 목표 회전값 (y축만 적용)
            Quaternion targetRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);

            while (sitting)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime * 100f // 각도 단위 회전
                );

                if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
                    break;

                yield return null;
            }

            if (sitting)
            {
                anim.SetTrigger("startSitting");
                GameManager.Instance.audioManager.PlaySitDown();
                while (true)
                {
                    // 위치 이동
                    transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, target.position) < 0.5f)
                        break;

                    yield return null;
                }
                isSitting = true;
            }
            else
            {
                while (true)
                {
                    // 위치 이동
                    transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, target.position) < 0.5f)
                        break;

                    yield return null;
                }
                GameManager.Instance.interactionManager.interactTarget.interacted = false;
                capsuleCollider.isTrigger = false;
                playerMove.dontMove = false;
                playerMove.characterController.enabled = true;
            }

            yield break;
        }


        public void ResetAnimationToStart()
        {
            anim.Rebind();         // 애니메이터 초기화
            anim.Update(0f);       // 즉시 상태 갱신 (프레임 반영)
            isLayingDown = true;
            isDrinking = false;
            isGettingPill = false;
            isGettingDrink = false;
            isSitting = false;
        }
    }


   
}
