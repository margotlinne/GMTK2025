using System.Collections;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class RingingPhone : MonoBehaviour
{
    AudioSource phoneRingingAudio;
    public AudioClip childSpeakingClip;

    void Start()
    {
        phoneRingingAudio = GetComponent<AudioSource>(); 
    }


    public void RingThePhone()
    {

        GameManager.Instance.loopManager.loopSecondBGMusicObj.GetComponent<Animation>().Play();
        phoneRingingAudio.Play();
    }




    public void StopRinging()
    {
        phoneRingingAudio.Stop();
        phoneRingingAudio.clip = childSpeakingClip;
        phoneRingingAudio.loop = false;
        phoneRingingAudio.Play();
        GameManager.Instance.cutsceneManager.StartCutScene(phoneRingingAudio);
    }

}
