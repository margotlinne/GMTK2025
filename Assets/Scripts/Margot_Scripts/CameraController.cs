using UnityEngine;
using UnityEngine.UIElements;

namespace Margot
{
    public class CameraController : MonoBehaviour
    {
        public GameObject postProcessingObj;
        public Transform livingroomCamPos;
        public Transform bathroomCamPos;
        public Transform bedroomCamPos;
        public Transform bedroomInteractPos;
        public Transform livingroomInteractPos;
        public Transform bathroomInteractPos;
        public Transform letterInteractPos;
        [HideInInspector] public Transform targetInteractPos;
        public Camera lenseCam;
        public GameObject livingroomBorder;
        public GameObject bathroomBorder;
        public GameObject bedroomBorder;
        public GameObject renderTextureCanvas;
        public GameObject postProcessing;

        public bool inLivingroom = true;
        public bool inBathroom = false;
        public bool inBedroom = false;
        public bool firstPerson = false;


        void Start()
        {
            renderTextureCanvas.SetActive(true);
            postProcessing.SetActive(true);

            inBedroom = true;
            livingroomBorder.SetActive(false);
            bathroomBorder.SetActive(false);
            bedroomBorder.SetActive(true);
        }

        void Update()
        {
            if (GameManager.Instance.loopManager.isLoopClear[GameManager.Instance.loopManager.loopMax - 1] && !firstPerson)
            {
                postProcessing.SetActive(false);
            }
            if (GameManager.Instance.loopManager.endingReady)
            {
                if (inLivingroom)
                {
                    GameManager.Instance.interactionManager.CloseDoors();
                }
            }

        
        }

        void LateUpdate()
        {
            if (!GameManager.Instance.cutsceneManager.readyToEnd)
            {
                if (inLivingroom)
                {
                    lenseCam.transform.position = livingroomCamPos.position;
                    lenseCam.transform.rotation = livingroomCamPos.rotation;
                    livingroomBorder.gameObject.SetActive(true);
                    bathroomBorder.gameObject.SetActive(false);
                    bedroomBorder.gameObject.SetActive(false);
                }
                if (inBathroom)
                {
                    lenseCam.transform.position = bathroomCamPos.position;
                    lenseCam.transform.rotation = bathroomCamPos.rotation;
                    livingroomBorder.gameObject.SetActive(false);
                    bathroomBorder.gameObject.SetActive(true);
                    bedroomBorder.gameObject.SetActive(false);
                }
                if (inBedroom)
                {
                    lenseCam.transform.position = bedroomCamPos.position;
                    lenseCam.transform.rotation = bedroomCamPos.rotation;
                    livingroomBorder.gameObject.SetActive(false);
                    bathroomBorder.gameObject.SetActive(false);
                    bedroomBorder.gameObject.SetActive(true);
                }
                if (firstPerson || GameManager.Instance.cutsceneManager.cutsceneStarted)
                {
                    // 시점 변경 시 플레이어 머리 보여서 렌더링 x
                    int layer = LayerMask.NameToLayer("Player");
                    lenseCam.cullingMask &= ~(1 << layer);

                    lenseCam.transform.position = targetInteractPos.position;
                    lenseCam.transform.rotation = targetInteractPos.rotation;
                }
                else
                {
                    int layer = LayerMask.NameToLayer("Player");
                    lenseCam.cullingMask |= (1 << layer);
                }
            }        

        }

        public void SetActiveRoom(string room)
        {
            switch (room)
            {
                case "Livingroom":
                    inLivingroom = true;
                    inBedroom = false;
                    inBathroom = false;
                    firstPerson = false;
                    break;
                case "Bathroom":
                    inLivingroom = false;
                    inBedroom = false;
                    inBathroom = true;
                    firstPerson = false;
                    break;
                case "Bedroom":
                    inLivingroom = false;
                    inBedroom = true;
                    inBathroom = false;
                    firstPerson = false;
                    break;
                case "FirstPersonView":
                    inLivingroom = false;
                    inBedroom = false;
                    inBathroom = false;
                    firstPerson = true;
                    break;
                default:
                    Debug.LogError("No such a room existed");
                    break;

            }
        }
    }
}

