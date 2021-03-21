/// <summary>
/// Used to switch the bow between left and right hands.
/// </summary>
namespace LongBow
{
    using System;
    using System.Collections;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class BowHandPlacement : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference leftHandActivationAction = default;
        [SerializeField] private InputActionReference rightHandActivationAction = default;

        private Transform bowTransform;
        private GameObject bowObject;
        private GameObject leftHandModel;
        private GameObject rightHandModel;
        private HandGrabber leftHandGrabber;
        private HandGrabber rightHandGrabber;

        private void Awake()
        {
            bowTransform = transform;
            bowObject = gameObject;
        }

        private void Start()
        {
            try
            {
                var _localPlayerItems = FindObjectsOfType<AvatarItemLink>();
                leftHandModel = _localPlayerItems.Where(x => x.AvatarItem == AvatarItemLink.AvatarItems.LeftHand).First().GetModel;
                leftHandGrabber = _localPlayerItems.Where(x => x.AvatarItem == AvatarItemLink.AvatarItems.LeftHand).First().gameObject.GetComponentInChildren<HandGrabber>();
                rightHandModel = _localPlayerItems.Where(x => x.AvatarItem == AvatarItemLink.AvatarItems.RightHand).First().GetModel;
                rightHandGrabber = _localPlayerItems.Where(x => x.AvatarItem == AvatarItemLink.AvatarItems.RightHand).First().gameObject.GetComponentInChildren<HandGrabber>();

                leftHandModel.SetActive(false);
                rightHandModel.SetActive(false);

                leftHandGrabber.heldObject = bowObject;
                bowTransform.parent = leftHandGrabber.grabPoint;
                bowTransform.localPosition = Vector3.zero;
                bowTransform.localRotation = Quaternion.identity;
            }
            catch (Exception ex)
            {
                Debug.LogError("Local player transforms not set properly: " + ex, this);
            }
        }

        private void OnEnable()
        {
            leftHandActivationAction.action.Enable();
            leftHandActivationAction.action.performed += ToggleBowLeftHand;
            rightHandActivationAction.action.Enable();
            rightHandActivationAction.action.performed += ToggleBowRightHand;
        }

        private void OnDisable()
        {
            leftHandActivationAction.action.Disable();
            leftHandActivationAction.action.performed -= ToggleBowLeftHand;
            rightHandActivationAction.action.Disable();
            rightHandActivationAction.action.performed -= ToggleBowRightHand;

            if(leftHandGrabber.heldObject == bowObject)
            {
                leftHandGrabber.heldObject = null;
            }
            if (rightHandGrabber.heldObject == bowObject)
            {
                rightHandGrabber.heldObject = null;
            }

            leftHandModel.SetActive(true);
            rightHandModel.SetActive(true);
        }

        private void ToggleBowLeftHand(InputAction.CallbackContext obj)
        {
            if (rightHandGrabber.heldObject == bowObject)
            {
                if(leftHandGrabber.heldObject != null)
                {
                    // TODO:
                    // object pooling
                    Destroy(leftHandGrabber.heldObject);
                }
                leftHandGrabber.heldObject = bowObject;
                bowTransform.parent = leftHandGrabber.grabPoint;
                bowTransform.localPosition = Vector3.zero;
                bowTransform.localRotation = Quaternion.identity;
                rightHandGrabber.heldObject = null;
            }
        }

        private void ToggleBowRightHand(InputAction.CallbackContext obj)
        {
            if (leftHandGrabber.heldObject == bowObject)
            {
                if (rightHandGrabber.heldObject != null)
                {
                    // TODO:
                    // object pooling
                    Destroy(rightHandGrabber.heldObject);
                }
                rightHandGrabber.heldObject = bowObject;
                bowTransform.parent = rightHandGrabber.grabPoint;
                bowTransform.localPosition = Vector3.zero;
                bowTransform.localRotation = Quaternion.identity;
                leftHandGrabber.heldObject = null;
            }
        }
    }
}
