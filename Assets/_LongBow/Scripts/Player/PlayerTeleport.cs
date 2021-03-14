/// <summary>
/// Allows player to teleport.  This is designed to be used with a single controller.
/// Requires a separate line renderer and target object to be setup.
/// </summary>
namespace LongBow
{
    using ScriptableObjectArchitecture;
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerTeleport : MonoBehaviour
    {
        // TODO:
        // the line renderer and validity is a bit janky
        // implement teleport anchors
        // implement invalid teleport component

        #region variables

        [Header("Object Links")]
        [SerializeField] private Transform destinationTarget = default;
        [SerializeField] private GameObject targetMarker = default;
        [SerializeField] private Transform playerToTeleport = default;
        [SerializeField] private Transform startingLineTransform = default;

        [Header("Input")]
        [SerializeField] private InputActionReference teleportAction = default;

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
        [SerializeField] private LayerMask CollisionLayers;
        [SerializeField] private LayerMask ValidLayers;

        [Header("Settings")]
        [SerializeField] private int segmentCount = 100;
        [SerializeField] private float simulationVelocity = 500f;
        [SerializeField] private float segmentScale = 0.5f;
        [SerializeField] private float teleportDelay = 0.2f;
        [SerializeField] private float maxSlope = 60f;
        [SerializeField] private int allowedInvalidFrames = 10;

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
        private Action onCanTeleportValueChanged;
        private Action onTeleportEvent;

        #endregion

        private void Awake()
        {
            onTeleportEvent = delegate { OnBeginTeleportEvent(); };
            onCanTeleportValueChanged = delegate { OnCanTeleportValueChanged(); };
        }

        private void Start()
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
            teleportAction.action.Enable();
            teleportAction.action.performed += BeginTeleportActionPerformed;
            teleportAction.action.canceled += EndTeleportActionPerformed;
            beginTeleportEvent.AddListener(onTeleportEvent);
            canTeleportVariable.AddListener(onCanTeleportValueChanged);
            lineRenderer.enabled = false;
            teleportStarted = false;
            isAiming = false;
            wasAiming = false;
            isValidDestination = false;
            targetMarker.SetActive(false);
        }

        private void OnDisable()
        {
            teleportAction.action.Disable();
            teleportAction.action.performed -= BeginTeleportActionPerformed;
            teleportAction.action.canceled -= EndTeleportActionPerformed;
            beginTeleportEvent.RemoveListener(onTeleportEvent);
            canTeleportVariable.RemoveListener(onCanTeleportValueChanged);
            teleportStarted = false;
            isAiming = false;
            wasAiming = false;
            isValidDestination = false;
        }

        /// <summary>
        /// Call this to force the player to teleport to a location.
        /// </summary>
        /// <param name="destination">The destination.</param>
        public void ForcePlayerTeleport(Vector3 destination)
        {
            CancelTeleportAttempt();
            StartCoroutine(TeleportRoutine(destination));
            isValidDestination = false;
        }

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
                //_updatedColor.a = 0;
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
            Vector3 _destination = destinationTarget.position;
            isAiming = false;
            lineRenderer.enabled = false;
            targetMarker.SetActive(false);
            if (!isValidDestination)
            {
                cancelTeleportEvent?.Raise();
                return;
            }
            StartCoroutine(TeleportRoutine(_destination));
            isValidDestination = false;
        }

        private IEnumerator TeleportRoutine(Vector3 destination)
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
                if (hitObject != null)
                {
                    segments[i] = hitVector;
                    continue;
                }

                float segTime = (segVelocity.sqrMagnitude != 0) ? segmentScale / segVelocity.magnitude : 0;
                segVelocity = segVelocity + Physics.gravity * segTime;

                if (Physics.Raycast(segments[i - 1], segVelocity, out hit, segmentScale, CollisionLayers))
                {
                    hitObject = hit.collider;
                    segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;
                    segVelocity = segVelocity - Physics.gravity * (segmentScale - hit.distance) / segVelocity.magnitude;
                    hitAngle = Vector3.Angle(transform.up, hit.normal);
                    targetMarker.transform.position = segments[i];
                    targetMarker.transform.rotation =
                        Quaternion.FromToRotation(targetMarker.transform.up, hit.normal) *
                        targetMarker.transform.rotation;
                    hitVector = segments[i];
                }
                else
                {
                    segments[i] = segments[i - 1] + segVelocity * segTime;
                }
            }

            isValidDestination = hitObject != null;

            if (isValidDestination && !isDestination)
            {
                if (hitAngle > maxSlope)
                {
                    isValidDestination = false;
                }

                // TODO:
                //if (_hitObject.GetComponent<InvalidTeleportArea>() != null)
                //{
                //    isValidDestination = false;
                //}

                var _validLayers = ValidLayers == (ValidLayers | (1 << hitObject.gameObject.layer));
                if (!_validLayers)
                {
                    isValidDestination = false;
                }
            }

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