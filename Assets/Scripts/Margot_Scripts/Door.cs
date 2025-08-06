using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;

    public void CloseDoorEnd()
    {
        if (GameManager.Instance.loopManager.endingReady)
        {
            GetComponent<BoxCollider>().isTrigger = false;
        }
    }
}
