using System.Collections;
using UnityEngine;

namespace Margot
{
    public class CalendarAnimation : MonoBehaviour
    {
        public AudioSource calendarRippingAudio;
        public Animation[] papers;
        public Animation tvAnimation;

        void Start()
        {
            StartCoroutine(PlayCalendarAnimation());
        }

        IEnumerator PlayCalendarAnimation()
        {
            for (int i = 0; i < papers.Length - 2; i ++)
            {
                Animation anim = papers[i];

                papers[i + 1].gameObject.SetActive(true);


                if (anim != null && anim.clip != null)
                {
                    anim.Play();
                    calendarRippingAudio.Play();

                    // 애니메이션이 끝날 때까지 대기
                    yield return new WaitForSeconds(anim.clip.length);
                }

            }
            tvAnimation["tv_main"].speed = 0.3f;    // 느리게
            yield break;
        }

    }

}
