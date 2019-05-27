using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRCModLoader;
using VRCSDK2;
using VRCTools;
using VRCTools.utils;

namespace CameraPlus
{
    [VRCModInfo("CameraPlus", "1.0.1", "Slaynash")]
    public class CameraPlus : VRCMod
    {

        private GameObject cameraHelper;

        public void OnApplicationStart()
        {
            ModManager.StartCoroutine(InitEnhancedCamera());
        }

        private IEnumerator InitEnhancedCamera()
        {
            yield return VRCUiManagerUtils.WaitForUiManagerInit();

            UserCameraController userCameraController = Resources.FindObjectsOfTypeAll<UserCameraController>()[0];
            VRCModLogger.Log("[CameraPlus] UserCameraController instance: " + userCameraController);
            cameraHelper = userCameraController.photoCamera.transform.Find("camera_lens_mesh")?.gameObject;



            //Zoom In
            GameObject zoomInButton = GameObject.Instantiate(userCameraController.viewFinder.transform.Find("PhotoControls/Right_Filters").gameObject, userCameraController.viewFinder.transform);
            GameObject.Destroy(zoomInButton.GetComponent<VRC_Trigger>());
            VRCT_Trigger zoomInTrigger = VRCT_Trigger.CreateVRCT_Trigger(zoomInButton, () => {
                Camera cam1 = userCameraController.photoCamera.GetComponent<Camera>();
                if (cam1.fieldOfView - 10 > 0) cam1.fieldOfView -= 10;
                Camera cam2 = userCameraController.videoCamera.GetComponent<Camera>();
                if (cam2.fieldOfView - 10 > 0) cam2.fieldOfView -= 10;
                userCameraController.speaker.PlayOneShot(userCameraController.buttonSound);
            });
            zoomInTrigger.interactText = "Zoom in";
            zoomInButton.transform.localPosition = zoomInButton.transform.localPosition + new Vector3(-0.05f, 0, 0);
            SetButtonTexture(zoomInButton, ImageDatas.zoomin);

            //Zoom Out
            GameObject zoomOutButton = GameObject.Instantiate(userCameraController.viewFinder.transform.Find("PhotoControls/Right_Extender").gameObject, userCameraController.viewFinder.transform);
            GameObject.Destroy(zoomOutButton.GetComponent<VRC_Trigger>());
            VRCT_Trigger zoomOutTrigger = VRCT_Trigger.CreateVRCT_Trigger(zoomOutButton, () => {
                Camera cam1 = userCameraController.photoCamera.GetComponent<Camera>();
                if (cam1.fieldOfView + 10 < 180) cam1.fieldOfView += 10;
                Camera cam2 = userCameraController.videoCamera.GetComponent<Camera>();
                if (cam2.fieldOfView + 10 < 180) cam2.fieldOfView += 10;
                userCameraController.speaker.PlayOneShot(userCameraController.buttonSound);
            });
            zoomOutTrigger.interactText = "Zoom out";
            zoomOutButton.transform.localPosition = zoomOutButton.transform.localPosition + new Vector3(-0.05f, 0, 0);
            SetButtonTexture(zoomOutButton, ImageDatas.zoomout);

            //Toggle camera helper
            GameObject toggleCameraHelperButton = GameObject.Instantiate(userCameraController.viewFinder.transform.Find("PhotoControls/Right_Timer").gameObject, userCameraController.viewFinder.transform);
            GameObject.Destroy(toggleCameraHelperButton.GetComponent<VRC_Trigger>());
            VRCT_Trigger toggleCameraHelperTrigger = VRCT_Trigger.CreateVRCT_Trigger(toggleCameraHelperButton, () => {
                cameraHelper?.SetActive(!cameraHelper.activeSelf);
                userCameraController.speaker.PlayOneShot(userCameraController.buttonSound);
            });
            toggleCameraHelperTrigger.interactText = "Toggle camera indicator";
            toggleCameraHelperButton.transform.localPosition = toggleCameraHelperButton.transform.localPosition + new Vector3(-0.05f, 0, 0);

            //Scale body to make it looks better
            Transform body = userCameraController.viewFinder.transform.Find("camera_mesh/body");
            body.localPosition = body.localPosition + new Vector3(-0.025f, 0, 0);
            body.localScale = body.localScale + new Vector3(0.8f, 0, 0);
            foreach (Transform trans in toggleCameraHelperButton.transform)
            {
                if (trans != null && trans.gameObject.name.StartsWith("icon-"))
                    trans.gameObject.SetActive(false);
            }
        }

        private void SetButtonTexture(GameObject button, string base64texture)
        {
            VRCModLogger.Log("[CameraPlus] Setting button texture");
            Texture2D t = new Texture2D(2, 2);
            Texture2DUtils.LoadImage(t, Convert.FromBase64String(base64texture));
            foreach(Transform trans in button.transform)
            {
                if (trans == null) continue;
                if (trans.gameObject.name.StartsWith("icon"))
                {
                    VRCModLogger.Log("[CameraPlus] Found icon element " + trans.gameObject);
                    trans.GetComponent<SpriteRenderer>().sprite = Sprite.Create(t, new Rect(0.0f, 0.0f, t.width, t.height), new Vector2(0.5f, 0.5f), 100.0f);
                    trans.localScale /= 2.4f;
                    VRCModLogger.Log("[CameraPlus] Icon updated successfully");
                }
            }
            VRCModLogger.Log("[CameraPlus] Done setting button texture");
        }
    }
}
