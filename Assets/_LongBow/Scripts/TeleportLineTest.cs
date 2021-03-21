namespace LongBow
{
    using UnityEngine;

    public class TeleportLineTest : MonoBehaviour
    {
        [Header("Object Links")]
        [SerializeField] private Transform destinationTarget = default;
        [SerializeField] private GameObject targetMarker = default;
        [SerializeField] private GameObject invalidTargetMarker = default;
        [SerializeField] private Transform playerToTeleport = default;
        [SerializeField] private Transform startingLineTransform = default;

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
        [SerializeField] private float simulationGravity = 10;

        private bool isAiming = false;
        private bool isValidDestination = false;
        private int invalidFrames = 0;
        private float initialLineWidth;
        private Collider hitObject;
        private Vector3 hitVector;
        private float hitAngle;
        private RaycastHit hit;

        private void Start()
        {
            initialLineWidth = lineRenderer.startWidth;
            if (lineRenderer.transform.parent != null)
            {
                lineRenderer.transform.parent = null;
                lineRenderer.transform.position = Vector3.zero;
                lineRenderer.transform.rotation = Quaternion.identity;
            }

            isAiming = true;
            lineRenderer.enabled = true;
        }

        private void Update()
        {
            if (isAiming)
            {
                lineRenderer.enabled = true;
                targetMarker.SetActive(isValidDestination);
                invalidTargetMarker.SetActive(!isValidDestination);

                //Color _updatedColor = isValidDestination ? validTeleportColor : invalidTeleportColor;
                if (!isValidDestination && invalidFrames < allowedInvalidFrames)
                {
                    //_updatedColor = validTeleportColor;
                    invalidTargetMarker.SetActive(false);
                }

                //_updatedColor.a = 1;
                //lineRenderer.startColor = _updatedColor;
                //lineRenderer.endColor = _updatedColor;
                //lineRenderer.startWidth = initialLineWidth;
            }
        }

        private void FixedUpdate()
        {
            if (isAiming)
            {
                CalculateTeleportLine();
            }
        }

        private void CalculateTeleportLine()
        {
            isValidDestination = false;
            hitObject = null;

            Vector3[] segments = new Vector3[segmentCount];
            segments[0] = startingLineTransform.position;
            Vector3 segVelocity = startingLineTransform.forward * simulationVelocity * Time.deltaTime;

            for (int i = 1; i < segmentCount; i++)
            {
                if (hitObject != null)
                {
                    segments[i] = hitVector;
                    continue;
                }

                float segTime = (segVelocity.sqrMagnitude != 0) ? segmentScale / segVelocity.magnitude : 0;
                segVelocity += (Vector3.down * simulationGravity) * segTime;

                if (Physics.Raycast(segments[i - 1], segVelocity, out hit, segmentScale, CollisionLayers))
                {
                    hitObject = hit.collider;
                    segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;
                    segVelocity -= (Vector3.down * simulationGravity) * (segmentScale - hit.distance) / segVelocity.magnitude;
                    hitAngle = Vector3.Angle(playerToTeleport.up, hit.normal);

                    targetMarker.transform.position = segments[i];
                    targetMarker.transform.rotation = Quaternion.FromToRotation(targetMarker.transform.up, hit.normal) * targetMarker.transform.rotation;

                    invalidTargetMarker.transform.position = segments[i];
                    invalidTargetMarker.transform.rotation = Quaternion.FromToRotation(invalidTargetMarker.transform.up, hit.normal) * invalidTargetMarker.transform.rotation;

                    hitVector = segments[i];
                }
                else
                {
                    segments[i] = segments[i - 1] + segVelocity * segTime;
                }
            }

            isValidDestination = (hitObject != null);

            if (isValidDestination)
            {
                // check slope
                if (hitAngle > maxSlope)
                {
                    Debug.Log("Hit Angle: " + hitAngle);
                    isValidDestination = false;
                }
                // check layer
                var _isValidLayer = (ValidLayers == (ValidLayers | (1 << hitObject.gameObject.layer)));
                if (!_isValidLayer)
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
