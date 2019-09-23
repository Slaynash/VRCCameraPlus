using System;
using UnityEngine;
using VRCSDK2;

namespace CameraPlus
{
    class VRCT_Trigger : VRC_Interactable
    {
        private Action onInteract;

        public override void Interact()
        {
            onInteract?.Invoke();
        }
        
        public static VRCT_Trigger CreateVRCT_Trigger(GameObject parent, Action onInteract)
        {
            VRCT_Trigger t = parent.AddComponent<VRCT_Trigger>();

            t.onInteract += onInteract;
            
            return t;
        }

        public override void Awake()
        {
            interactTextPlacement = transform;
            base.Awake();
        }
    }
}