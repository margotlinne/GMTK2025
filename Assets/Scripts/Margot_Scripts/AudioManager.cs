using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Transform audioSourceParent;
    private int poolNum = 10;
    public List<GameObject> audioPool = new List<GameObject>();

    [Header("Audio Clips")]
    public AudioClip openingFridge;
    public AudioClip closingFridge;
    public AudioClip openingDoor;
    public AudioClip closingDoor;
    public AudioClip sittingDown;
    public AudioClip standingUp;
    public AudioClip closingEyes;
    public AudioClip pikcingUpBottle;
    public AudioClip kickingBottle;
    public AudioClip drinkingBottle;
    public AudioClip takingPills;
    public AudioClip grabbingPaper;

    public AudioClip negativeInteraction;
    public AudioClip positiveInteraction;

    void Start()
    {
        InitiatePool();
    }

    #region AudioSource Pool
    void InitiatePool()
    {
        for (int i = 0; i < poolNum; i++)
        {
            GameObject audioSource = Instantiate(new GameObject("PooledAudio"), audioSourceParent.transform);
            audioSource.AddComponent<AudioSource>().playOnAwake = false;
            audioSource.SetActive(false);
            audioSource.GetComponent<AudioSource>().volume = 0.5f;
            audioPool.Add(audioSource);
        }
    }

    GameObject TakeFromPool()
    {
        for (int i = 0; i < audioPool.Count; i++)
        {
            if (!audioPool[i].activeSelf)
                return audioPool[i];

            if (i == audioPool.Count - 1)
            {
                Debug.LogWarning("No available AudioSource in pool! Creating a new one.");
                GameObject newAudio = Instantiate(new GameObject("PooledAudio"), audioSourceParent.transform);
                newAudio.AddComponent<AudioSource>().playOnAwake = false;
                newAudio.SetActive(false);
                audioPool.Add(newAudio);
                return newAudio;
            }
        }
        return null;
    }

    void ReturnToPool(GameObject item)
    {
        AudioSource source = item.GetComponent<AudioSource>();
        source.Stop();
        source.clip = null;
        source.loop = false;
        item.SetActive(false);
    }
    #endregion

    #region One-shot Sound
    public void PlaySound(AudioClip clip, float pitch = 1f, float volume = 0.5f, bool loop = false)
    {
        GameObject obj = TakeFromPool();
        obj.SetActive(true);
        AudioSource source = obj.GetComponent<AudioSource>();

        source.loop = loop;
        source.pitch = pitch;
        source.volume = volume;
        source.clip = clip;

        source.Play();

        if (!loop)
            StartCoroutine(ReturnAfterPlay(obj, clip.length / Mathf.Abs(pitch)));
    }


    private IEnumerator ReturnAfterPlay(GameObject obj, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        ReturnToPool(obj);
    }

    public void PlayFridgeOpen() => PlaySound(openingFridge);
    public void PlayFridgeClose() => PlaySound(closingFridge);
    public void PlayDoorOpen() => PlaySound(openingDoor);
    public void PlayDoorClose() => PlaySound(closingDoor);
    public void PlaySitDown() => PlaySound(sittingDown);
    public void PlayStandUp() => PlaySound(standingUp);

 

    public void PlayCloseEyes() => PlaySound(closingEyes, 0.5f);
    public void PlayPickUpBottle() => PlaySound(pikcingUpBottle);
    public void PlayKickBottle() => PlaySound(kickingBottle, 1f, 0.1f);
    public void PlayDrinkBottle() => PlaySound(drinkingBottle);
    public void PlayTakePills() => PlaySound(takingPills);

    public void PlayGrabPaper() => PlaySound(grabbingPaper);

    public void PlayPositiveInteraction() => PlaySound(positiveInteraction);
    public void PlayNegativeInteraction() => PlaySound(negativeInteraction);
    #endregion




}
