namespace LongBow
{
    using ScriptableObjectArchitecture;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class CustomTeleportController : MonoBehaviour
    {
        // TODO:
        // the line renderer and validity is a bit janky
        // implement Teleport Anchors
        // implement direction facing when teleporting

        #region variables

        [Header("Object Links")]
        [SerializeField] private Transform destinationTarget = default;
        [SerializeField] private GameObject targetMarker = default;
        [SerializeField] private Transform directionIndicator = default;
        [SerializeField] private Transform playerToTeleport = default;
        [SerializeField] private Transform startingLineTransform = default;

        [Header("Input")]
        [SerializeField] private InputActionReference beginTeleportAction = default;
        [SerializeField] private InputActionReference endTeleportAction = default;

        [Header("Events")]
        [SerializeField] private GameEvent beginTeleportEvent = default;
        [SerializeField] private GameEvent afterTeleportEvent = default;
        [SerializeField] private GameEvent cancelTeleportEvent = default;

        [Header("Variables")]
        [SerializeField] private BoolReference canTeleportVariable = default;

        [Header("Line Renderer")]
        [SerializeField] private LineRenderer lineRenderer = default;
        [SerializeField] private Color validTeleportColor = Color.green;
        [SerializeField] private Color invalidTeleportColor = Color.red;

        [Header("Layers")]

        [Tooltip("Raycast layers to use when determining collision")]
        [SerializeField] private LayerMask CollisionLayers;
        [Tooltip("Raycast layers to use when determining if the collided object is a valid teleport. If it is not valid then the line will be red and unable to teleport.")]
        [SerializeField] private LayerMask ValidLayers;

        [Header("Settings")]
        [SerializeField] private float maxRange = 20f;
        [Tooltip("More segments means a smoother line, at the cost of performance.")]
        [SerializeField] private int segmentCount = 100;
        [Tooltip("How much velocity to apply when calculating a parabola. Set to a very high number for a straight line.")]
        [SerializeField] private float simulationVelocity = 500f;
        [Tooltip("Scale of each segment used when calculating parabola")]
        [SerializeField] private float segmentScale = 0.5f;
        [Tooltip("Seconds to wait before initiating teleport. Useful if you want to fade the screen  before teleporting.")]
        [SerializeField] private float teleportDelay = 0.2f;
        [Tooltip("Max Angle / Slope the teleport marker can be to be considered a valid teleport.")]
        [SerializeField] private float maxSlope = 60f;
        [Tooltip("How long to wait until changing a valid teleport line to invalid.")]
        [SerializeField] private int allowedInvalidFrames = 10;

        [Header("Debug")]
        [SerializeField] private string debugString = default;

        private bool teleportStarted = false;
        private bool isAiming = false;
        private bool wasAiming = false;
        private bool isValidDestination = false;
        private WaitForSeconds routineDelay;
        private int invalidFrames = 0;
        private float initialLineWidth;

        private Collider hitObject;
        private Vector3 hitVector;
        private float hitAngle;
        private RaycastHit hit;

        #endregion

        private void Awake()
        {
            routineDelay = new WaitForSeconds(teleportDelay);
            initialLineWidth = lineRenderer.startWidth;
            if (lineRenderer.transform.parent != null)
            {
                lineRenderer.transform.parent = null;
                lineRenderer.transform.position = Vector3.zero;
                lineRenderer.transform.rotation = Quaternion.identity;
            }
        }

        private void OnEnable()
        {
            beginTeleportAction.action.Enable();
            beginTeleportAction.action.performed += BeginTeleportActionPerformed;
            beginTeleportAction.action.canceled += EndTeleportActionPerformed;
            //beginTeleportEvent.onRaise.AddResponse(OnBeginTeleportEvent);
            beginTeleportEvent.AddListener(OnBeginTeleportEvent);
            //canTeleportVariable.onChangeValue.AddResponse(OnCanTeleportValueChanged);
            canTeleportVariable.AddListener(OnCanTeleportValueChanged);
            lineRenderer.enabled = false;
            teleportStarted = false;
            isAiming = false;
            wasAiming = false;
            isValidDestination = false;
            targetMarker.SetActive(false);
        }

        private void OnDisable()
        {
            beginTeleportAction.action.Disable();
            beginTeleportAction.action.performed -= BeginTeleportActionPerformed;
            beginTeleportAction.action.canceled -= EndTeleportActionPerformed;
            //beginTeleportEvent.onRaise.RemoveResponse(OnBeginTeleportEvent);
            beginTeleportEvent.RemoveListener(OnBeginTeleportEvent);
            //canTeleportVariable.onChangeValue.RemoveResponse(OnCanTeleportValueChanged);
            canTeleportVariable.RemoveListener(OnCanTeleportValueChanged);
            teleportStarted = false;
            isAiming = false;
            wasAiming = false;
            isValidDestination = false;
        }

        //private void OnCanTeleportValueChanged(bool value)
        //{
        //    if (!value) CancelTeleportAttempt();
        //}
        private void OnCanTeleportValueChanged()
        {
            if (!canTeleportVariable.Value) CancelTeleportAttempt();
        }

        private void BeginTeleportActionPerformed(InputAction.CallbackContext obj)
        {
            if (!canTeleportVariable.Value) return;
            teleportStarted = true;
            beginTeleportEvent?.Raise();
        }

        private void EndTeleportActionPerformed(InputAction.CallbackContext obj)
        {
            isAiming = false;
        }

        private void OnBeginTeleportEvent()
        {
            var _thisControllerToggled = teleportStarted;
            CancelTeleportAttempt();
            if (!_thisControllerToggled) return;
            BeginTeleportAttempt();
        }

        private void BeginTeleportAttempt()
        {
            isAiming = true;
        }

        private void CancelTeleportAttempt()
        {
            lineRenderer.enabled = false;
            teleportStarted = false;
            wasAiming = false;
            isAiming = false;
            isValidDestination = false;
            targetMarker.SetActive(false);
        }

        private void Update()
        {
            if (wasAiming && !isAiming)
            {
                AttemptTeleport();
            }
            wasAiming = isAiming;

            if (isAiming)
            {
                lineRenderer.enabled = true;
                Color _updatedColor = isValidDestination ? validTeleportColor : invalidTeleportColor;
                if (!isValidDestination && invalidFrames < allowedInvalidFrames)
                {
                    _updatedColor = validTeleportColor;
                }
                _updatedColor.a = 1;
                lineRenderer.startColor = _updatedColor;
                _updatedColor.a = 0;
                lineRenderer.endColor = _updatedColor;
                lineRenderer.startWidth = initialLineWidth;
                targetMarker.SetActive(isValidDestination);
            }
        }

        private void FixedUpdate()
        {
            if (isAiming)
            {
                CalculateTeleportLine();
            }
        }

        private void AttemptTeleport()
        {
            //Debug.Log(debugString);
            //VrConsole.Log(debugString);
            Vector3 _destination = destinationTarget.position;
            Quaternion _rotation = directionIndicator.rotation;
            isAiming = false;
            lineRenderer.enabled = false;
            targetMarker.SetActive(false);
            if (!isValidDestination)
            {
                cancelTeleportEvent?.Raise();
                return;
            }
            StartCoroutine(TeleportRoutine(_destination, _rotation));
            isValidDestination = false;
        }

        private IEnumerator TeleportRoutine(Vector3 destination, Quaternion rotation)
        {
            yield return routineDelay;
            playerToTeleport.position = destination;
            afterTeleportEvent?.Raise();
        }

        private void CalculateTeleportLine()
        {
            isValidDestination = false;
            bool isDestination = false;
            hitObject = null;

            Vector3[] segments = new Vector3[segmentCount];
            segments[0] = startingLineTransform.position;
            Vector3 segVelocity = startingLineTransform.forward *
                simulationVelocity *
                Time.fixedUnscaledDeltaTime;

            for (int i = 1; i < segmentCount; i++)
            {

                // Hit something, so assign all future segments to this segment
                if (hitObject != null)
                {
                    segments[i] = hitVector;
                    continue;
                }

                // Time it takes to traverse one segment of length segScale (careful if velocity is zero)
                float segTime = (segVelocity.sqrMagnitude != 0) ? segmentScale / segVelocity.magnitude : 0;

                // Add velocity from gravity for this segment's timestep
                segVelocity = segVelocity + Physics.gravity * segTime;

                // Check to see if we're going to hit a physics object
                if (Physics.Raycast(segments[i - 1], segVelocity, out hit, segmentScale, CollisionLayers))
                {

                    // remember who we hit
                    hitObject = hit.collider;

                    // set next position to the position where we hit the physics object
                    segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;

                    // correct ending velocity, since we didn't actually travel an entire segment
                    segVelocity = segVelocity - Physics.gravity * (segmentScale - hit.distance) / segVelocity.magnitude;

                    hitAngle = Vector3.Angle(transform.up, hit.normal);

                    // Align marker to hit normal
                    targetMarker.transform.position = segments[i]; // hit.point;
                    targetMarker.transform.rotation =
                        Quaternion.FromToRotation(targetMarker.transform.up, hit.normal) *
                        targetMarker.transform.rotation;

                    hitVector = segments[i];
                }
                // Nothing hit, continue line by settings next segment to the last
                else
                {
                    segments[i] = segments[i - 1] + segVelocity * segTime;
                }
            }

            isValidDestination = hitObject != null;

            // Make sure teleport location is valid
            if (isValidDestination && !isDestination)
            {

                // Angle too steep
                if (hitAngle > maxSlope)
                {
                    isValidDestination = false;
                }

                // Hit a restricted zone
                //if (_hitObject.GetComponent<InvalidTeleportArea>() != null)
                //{
                //    isValidDestination = false;
                //}

                // Something in the way via raycast
                var _validLayers = ValidLayers == (ValidLayers | (1 << hitObject.gameObject.layer));
                if (!_validLayers)
                {
                    isValidDestination = false;
                }
            }

            // Render the positions as a line
            lineRenderer.positionCount = segmentCount;
            for (int i = 0; i < segmentCount; i++)
            {
                lineRenderer.SetPosition(i, segments[i]);
            }

            if (!isValidDestination)
            {
                invalidFrames++;
            }
            else
            {
                invalidFrames = 0;
            }
        }
    }
}