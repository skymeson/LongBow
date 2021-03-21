/// <summary>
/// Used to create arrows for the player to grab.
/// </summary>
namespace LongBow
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class Quiver : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference leftGrabInput = default;
        [SerializeField] private InputActionReference rightGrabInput = default;

        [Header("Object Links")]
        [SerializeField] private HandGrabber leftHandGrabber = default;
        [SerializeField] private HandGrabber rightHandGrabber = default;

        [Header("Prefabs")]
        [SerializeField] private GameObject arrowPrefab = default;

        private bool canGrabLeft = false;
        private bool canGrabRight = false;

        private void OnEnable()
        {
            leftGrabInput.action.Enable();
            rightGrabInput.action.Enable();
            leftGrabInput.action.performed += LeftGrabActionPerformed;
            rightGrabInput.action.performed += RightGrabActionPerformed;
        }

        private void OnDisable()
        {
            leftGrabInput.action.Disable();
            rightGrabInput.action.Disable();
            leftGrabInput.action.performed -= LeftGrabActionPerformed;
            rightGrabInput.action.performed -= RightGrabActionPerformed;
        }

        private void RightGrabActionPerformed(InputAction.CallbackContext obj)
        {
            VrConsole.Log("Right grab action performed.");
            if (!canGrabRight || !IsHoldingBow())
            {
                VrConsole.Log("Can grab: " + canGrabRight);
                VrConsole.Log("Holding bow: " + IsHoldingBow());
                return;
            }
            if (rightHandGrabber.heldObject != null)
            {
                VrConsole.Log("Held item: " + rightHandGrabber.heldObject.name);
                return;
            }
            SpawnArrow(rightHandGrabber);
        }

        private void LeftGrabActionPerformed(InputAction.CallbackContext obj)
        {
            VrConsole.Log("Left grab action performed.");
            if (!canGrabLeft || !IsHoldingBow()) return;
            if (leftHandGrabber.heldObject != null) return;
            SpawnArrow(leftHandGrabber);
        }

        private void SpawnArrow(HandGrabber grabber)
        {
            VrConsole.Log("Arrow spawning.");
            // TODO:
            // object pooling for arrows

            var _arrow = Instantiate(arrowPrefab);
            grabber.heldObject = _arrow;
            _arrow.transform.parent = grabber.grabPoint;
            _arrow.transform.localPosition = Vector3.zero;
            _arrow.transform.localRotation = Quaternion.identity;
        }

        private bool IsHoldingBow()
        {
            if (leftHandGrabber.heldObject != null &&
                leftHandGrabber.heldObject.GetComponent<Bow>() != null)
            {
                return true;
            }

            if (rightHandGrabber.heldObject != null &&
                rightHandGrabber.heldObject.GetComponent<Bow>() != null)
            {
                return true;
            }

            return false;
        }

        private void OnTriggerEnter(Collider other)
        {
            VrConsole.Log("Quiver trigger entered by: " + other.name);
            var _grabber = other.GetComponent<HandGrabber>();
            if (_grabber == null) return;
            if (_grabber == leftHandGrabber)
            {
                canGrabLeft = true;
            }
            if (_grabber == rightHandGrabber)
            {
                canGrabRight = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            VrConsole.Log("Quiver trigger exited by: " + other.name);
            var _grabber = other.GetComponent<HandGrabber>();
            if (_grabber == null) return;
            if (_grabber == leftHandGrabber)
            {
                canGrabLeft = false;
            }
            if (_grabber == rightHandGrabber)
            {
                canGrabRight = false;
            }
        }
    }
}
