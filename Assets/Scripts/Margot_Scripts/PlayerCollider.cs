using EPOOutline.Demo;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    public Margot.CameraController cameraController;



    void OnTriggerEnter(Collider other)
    {
        if (!cameraController.firstPerson)
        {
            if (other.gameObject.tag == "Livingroom" || other.gameObject.tag == "Bathroom" || other.gameObject.tag == "Bedroom")
                cameraController.SetActiveRoom(other.gameObject.tag);
        }


        if (other.gameObject.CompareTag("Phone") && GameManager.Instance.loopManager.endingReady)
        {
            GameManager.Instance.loopManager.phoneObj.GetComponent<RingingPhone>().StopRinging();
        }

        if (other.gameObject.CompareTag("Bottle"))
        {
            GameManager.Instance.audioManager.PlayKickBottle();
        }
    }
}
