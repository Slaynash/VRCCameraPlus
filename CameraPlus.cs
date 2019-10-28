using System;
using System.Collections;
using UnityEngine;
using VRCModLoader;
using VRCTools;

namespace CameraPlus
{
    [VRCModInfo("CameraPlus", "2.0.0", "Slaynash, Herp Derpinstine")]
    public class CameraPlus : VRCMod
    {
        public void OnApplicationStart() { ModManager.StartCoroutine(InitEnhancedCamera()); }

        private IEnumerator InitEnhancedCamera()
        {
            yield return VRCUiManagerUtils.WaitForUiManagerInit();
            
            // Grab Controller
            UserCameraController userCameraController = Resources.FindObjectsOfTypeAll<UserCameraController>()[0];

            // Create Sprites
            Sprite zoomin_sprite = CreateSprite(ImageData.zoomin_image);
            Sprite zoomout_sprite = CreateSprite(ImageData.zoomout_image);
            Sprite cameraindicator_on_sprite = CreateSprite(ImageData.cameraindicator_on_image);
            Sprite cameraindicator_off_sprite = CreateSprite(ImageData.cameraindicator_off_image);

            // Zoom-In
            GameObject zoomInButton = GameObject.Instantiate(userCameraController.viewFinder.transform.Find("PhotoControls/Right_Filters").gameObject, userCameraController.viewFinder.transform);
            VRCSDK2.VRC_CustomTrigger.Create("Zoom-In", zoomInButton, () =>
            {
                Camera cam = userCameraController.photoCamera.GetComponent<Camera>();
                if ((cam.fieldOfView - 10) > 0) cam.fieldOfView -= 10;
                cam = userCameraController.videoCamera.GetComponent<Camera>();
                if ((cam.fieldOfView - 10) > 0) cam.fieldOfView -= 10;
                userCameraController.speaker.PlayOneShot(userCameraController.buttonSound);
            });
            SetButtonSprite(zoomInButton, zoomin_sprite);
            SetButtonIconScale(zoomInButton);
            SetButtonOffset(zoomInButton);

            // Zoom-Out
            GameObject zoomOutButton = GameObject.Instantiate(userCameraController.viewFinder.transform.Find("PhotoControls/Right_Extender").gameObject, userCameraController.viewFinder.transform);
            VRCSDK2.VRC_CustomTrigger.Create("Zoom-Out", zoomOutButton, () =>
            {
                Camera cam = userCameraController.photoCamera.GetComponent<Camera>();
                if ((cam.fieldOfView + 10) < 180) cam.fieldOfView += 10;
                cam = userCameraController.videoCamera.GetComponent<Camera>();
                if ((cam.fieldOfView + 10) < 180) cam.fieldOfView += 10;
                userCameraController.speaker.PlayOneShot(userCameraController.buttonSound);
            });
            SetButtonSprite(zoomOutButton, zoomout_sprite);
            SetButtonIconScale(zoomOutButton);
            SetButtonOffset(zoomOutButton);

            // Toggle Camera Indicator
            GameObject cameraHelper = userCameraController.photoCamera.transform.Find("camera_lens_mesh").gameObject;
            GameObject toggleCameraIndicatorButton = GameObject.Instantiate(userCameraController.viewFinder.transform.Find("PhotoControls/Right_Timer").gameObject, userCameraController.viewFinder.transform);
            VRCSDK2.VRC_CustomTrigger.Create("Camera Indicator", toggleCameraIndicatorButton, () =>
            {
                cameraHelper.SetActive(!cameraHelper.activeSelf);
                if (cameraHelper.activeSelf)
                    SetButtonSprite(toggleCameraIndicatorButton, cameraindicator_on_sprite);
                else
                    SetButtonSprite(toggleCameraIndicatorButton, cameraindicator_off_sprite);
                userCameraController.speaker.PlayOneShot(userCameraController.buttonSound);
            });
            SetButtonSprite(toggleCameraIndicatorButton, cameraindicator_on_sprite);
            SetButtonIconScale(toggleCameraIndicatorButton);
            SetButtonOffset(toggleCameraIndicatorButton);

            // Resize Camera Body
            Transform camera_body = userCameraController.viewFinder.transform.Find("camera_mesh/body");
            camera_body.localPosition = camera_body.localPosition + new Vector3(-0.025f, 0, 0);
            camera_body.localScale = camera_body.localScale + new Vector3(0.8f, 0, 0);
        }

        private static Sprite CreateSprite(string base64texture)
        {
            Texture2D texture = new Texture2D(2, 2);
            VRCTools.utils.Texture2DUtils.LoadImage(texture, Convert.FromBase64String(base64texture));
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        private static void SetButtonIconScale(GameObject button)
        {
            foreach (Transform trans in button.transform)
            {
                if ((trans != null) && trans.gameObject.name.StartsWith("icon"))
                    trans.localScale /= 2.4f;
            }
        }

        private static void SetButtonSprite(GameObject button, Sprite sp)
        {
            foreach (Transform trans in button.transform)
            {
                if ((trans != null) && trans.gameObject.name.StartsWith("icon"))
                    trans.GetComponent<SpriteRenderer>().sprite = sp;
            }
        }

        private static void SetButtonOffset(GameObject button) { button.transform.localPosition = button.transform.localPosition + new Vector3(-0.05f, 0, 0); }
    }
}
