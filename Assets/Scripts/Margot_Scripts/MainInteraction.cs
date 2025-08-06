using EPOOutline;
using Unity.VisualScripting;
using UnityEngine;

public class MainInteraction : MonoBehaviour
{
    public RoomState roomState;
    ObjectInteract objectInteractor;
    Animation anim;

    public bool interacted = false;
    bool played = false;


    void Start()
    {
        objectInteractor = GetComponent<ObjectInteract>();
        anim = GetComponent<Animation>();
    }

    void Update()
    {
        //if (GameManager.Instance.loopManager.roomState != roomState)
        //{
        //    objectInteractor.enabled = false;
        //}
        //else objectInteractor.enabled = true;

        if (objectInteractor.enabled)
        {
            if (!played)
            {
                if (anim != null) anim.Play();
                played = true;
            }
        }

        // 현재 방 상호작용 물체 차례가 아니라면
        if (GameManager.Instance.loopManager.currentLoop > 0)
        {
            if (roomState != GameManager.Instance.loopManager.roomState)
            {
                GetComponent<Outlinable>().enabled = false;
                objectInteractor.enabled = false;
            }
            else
            {
                GetComponent<Outlinable>().enabled = true;
                objectInteractor.enabled = true;
            }

        }

        if (interacted)
        {
            GameManager.Instance.loopManager.hologramBehaviours[(int)roomState].roomClear = true;
            objectInteractor.enabled = false;
            GetComponent<Outlinable>().enabled = false;
            enabled = false;
        }
    }
}
