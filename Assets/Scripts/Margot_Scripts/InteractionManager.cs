using EPOOutline;
using EPOOutline.Demo;
using UnityEngine;

namespace Margot
{
    public class InteractionManager : MonoBehaviour
    {
        public ObjectInteract interactTarget = null;
        public GameObject[] bedroom_interactableObjs;
        public GameObject[] livingroom_interactableObjs;
        public GameObject[] bathroom_interactableObjs;
        public GameObject[] doors;
        public Door frontDoor;
        
        public GameObject letterObj;
        public GameObject phoneObj;

        public GameObject[] mainInteractableObjs;

        bool doorClosed = false;
        public bool isGrabbing = false;
        public bool readLetter = false;
        public bool actviateInteractableDoor = false;

        public GameObject grabbedObj = null;
        public GameObject drink = null;
        public GameObject pill = null;

        public bool endingCutScenePlay = false;

        void Start()
        {
            foreach(GameObject obj in bedroom_interactableObjs)
            {
                if (obj.GetComponent<ObjectInteract>() == null) obj.AddComponent<ObjectInteract>();
            }
            foreach (GameObject obj in livingroom_interactableObjs)
            {
                if (obj.GetComponent<ObjectInteract>() == null) obj.AddComponent<ObjectInteract>();
            }
            foreach (GameObject obj in bathroom_interactableObjs)
            {
                if (obj.GetComponent<ObjectInteract>() == null) obj.AddComponent<ObjectInteract>();
            }

            foreach (GameObject obj in mainInteractableObjs)
            {
                obj.GetComponent<ObjectInteract>().enabled = false;
            }

            foreach (GameObject obj in doors)
            {
                if (obj.GetComponent<ObjectInteract>() != null) obj.GetComponent<ObjectInteract>().enabled = false;
            }

            letterObj.SetActive(false);
        }

        void Update()
        {
            #region Visualise Grabbed Obj
            if (!GameManager.Instance.loopManager.isLoopClear[0])
            {
                foreach (GameObject obj in bedroom_interactableObjs)
                {
                    if (obj.tag == "ForBottle")
                    {
                        if (isGrabbing) obj.GetComponent<ObjectInteract>().enabled = false;
                        else obj.GetComponent<ObjectInteract>().enabled = true;
                    }
                }
            }
            if (!GameManager.Instance.loopManager.isLoopClear[1])
            {
                foreach (GameObject obj in livingroom_interactableObjs)
                {
                    if (obj.tag == "ForBottle")
                    {
                        if (isGrabbing) obj.GetComponent<ObjectInteract>().enabled = false;
                        else obj.GetComponent<ObjectInteract>().enabled = true;
                    }
                }
            }
            if (!GameManager.Instance.loopManager.isLoopClear[2])
            {
                foreach (GameObject obj in bathroom_interactableObjs)
                {
                    if (obj.tag == "ForBottle")
                    {
                        if (isGrabbing) obj.GetComponent<ObjectInteract>().enabled = false;
                        else obj.GetComponent<ObjectInteract>().enabled = true;
                    }
                }
            }
            #endregion

            if (GameManager.Instance.loopManager.currentLoop > 0) // 두 번째 부턴 문 열기 가능
            {
                if (!actviateInteractableDoor)
                {
                    for (int i = 0; i < doors.Length; i++)
                    {
                        ObjectInteract obj = doors[i].GetComponent<ObjectInteract>();
                        obj.enabled = true;
                    }
                    actviateInteractableDoor = true;    
                }

                //if (!mainInteractableObjs[(int)GameManager.Instance.loopManager.roomState].GetComponent<MainInteraction>().interacted)
                //{
                //    mainInteractableObjs[(int)GameManager.Instance.loopManager.roomState].GetComponent<ObjectInteract>().enabled = true;
                //    mainInteractableObjs[(int)GameManager.Instance.loopManager.roomState].GetComponent<Outlinable>().enabled = true;
                //}
                //else
                //{
                //    mainInteractableObjs[(int)GameManager.Instance.loopManager.roomState].GetComponent<ObjectInteract>().enabled = false;
                //    mainInteractableObjs[(int)GameManager.Instance.loopManager.roomState].GetComponent<Outlinable>().enabled = false;

                //}
            }

            if (!letterObj.activeSelf && GameManager.Instance.loopManager.endingReady)
            {
                letterObj.SetActive(true);
            }


            

        }

        public void CloseDoors()
        {
            if (!doorClosed)
            {
                foreach (GameObject obj in doors)
                {
                    if (obj.GetComponent<Door>().isOpen)
                    {
                        // GameManager.Instance.audioManager.PlayDoorClose();
                        obj.GetComponent<Animation>().Play("door_close");
                        // obj.GetComponent<BoxCollider>().isTrigger = false;
                    }

                    obj.GetComponent<ObjectInteract>().enabled = false;
                    obj.GetComponent<Outlinable>().enabled = false;
                    
                }
                // GameManager.Instance.loopManager.waitForCalling = true;
                doorClosed = true;
            }
       
        }

        public void LoopEnd()
        {
            foreach(GameObject obj in mainInteractableObjs)
            {
                obj.GetComponent<MainInteraction>().interacted = false;
                obj.GetComponent<ObjectInteract>().enabled = true;
                obj.GetComponent<MainInteraction>().enabled = true;
            }
        }
    }
}

