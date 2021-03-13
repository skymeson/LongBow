/// <summary>
/// Used to raycast and interact with UI elements with the CustomUiElement component.
/// </summary>
namespace LongBow
{
    using ScriptableObjectArchitecture;
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class CustomUiPointer : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference uiInputAction = default;
        
        [Header("Object Links")]
        [SerializeField] private LineRenderer lineRenderer = default;
        [SerializeField] private GameObject reticleObject = default;

        [Header("Variables")]
        [SerializeField] private BoolReference canInteractVariable = default;

        [Header("Settings")]
        [SerializeField] private LayerMask uiLayer = default;
        [SerializeField] private LayerMask blockingLayers = default;
        [SerializeField] private int deselectAfterFrames = 10;
        
        private Transform reticleTransform = default;
        private CustomUiElement currentElement = null;
        private Transform thisTransform;
        private int invalidFrames = 0;
        private bool canInteract = true;
        private Action onCanInteractVariableChanged;

        private void Awake()
        {
            thisTransform = transform;
            onCanInteractVariableChanged = delegate { OnChanged(); };
        }

        private void Start()
        {

            lineRenderer.enabled = false;
            invalidFrames = 0;
            canInteract = true;
            reticleTransform = reticleObject.transform;
        }

        private void OnEnable()
        {
            uiInputAction.action.Enable();
            uiInputAction.action.performed += UiActionPerformed;
            canInteractVariable.AddListener(onCanInteractVariableChanged);
            lineRenderer.enabled = false;
            reticleObject.SetActive(false);
            invalidFrames = 0;
            canInteract = true;
        }

        private void OnDisable()
        {
            uiInputAction.action.Disable();
            canInteractVariable.RemoveListener(onCanInteractVariableChanged);
        }

        private void OnChanged() 
        { 
            canInteract = canInteractVariable.Value; 
        }

        private void UiActionPerformed(InputAction.CallbackContext obj)
        {
            if (currentElement == null) return;
            currentElement.OnTriggerButton();
            currentElement = null;
            invalidFrames = 0;
        }

        private void Update()
        {
            if (!canInteract) return;

            if (Physics.Raycast(thisTransform.position, thisTransform.forward, out RaycastHit _hit, 1000.0f, uiLayer))
            {
                var _distance = _hit.distance;
                if (Physics.Raycast(thisTransform.position, thisTransform.forward, out RaycastHit _blockingHit, 1000.0f, blockingLayers))
                {
                    var _blockingDistance = _blockingHit.distance;
                    if (_blockingDistance < _distance) return;
                }

                lineRenderer.enabled = true;
                reticleObject.SetActive(true);
                lineRenderer.positionCount = 2;
                var _hitTransform = _hit.transform;
                lineRenderer.SetPosition(0, thisTransform.position);
                lineRenderer.SetPosition(1, _hitTransform.position);
                reticleTransform.position = _hitTransform.position;
                reticleTransform.rotation = Quaternion.FromToRotation(reticleTransform.up, _hit.normal) * reticleTransform.rotation;

                var _foundElement = _hitTransform.GetComponent<CustomUiElement>();
                if (_foundElement == null)
                {
                    currentElement = null;
                    return;
                }

                if (currentElement == null)
                {
                    currentElement = _foundElement;
                    currentElement.BeginHover();
                    return;
                }
                else
                {
                    if (currentElement != _foundElement)
                    {
                        invalidFrames++;
                        if (invalidFrames > deselectAfterFrames)
                        {
                            invalidFrames = 0;
                            currentElement.EndHover();
                            currentElement = _foundElement;
                            currentElement.BeginHover();
                        }
                    }
                    return;
                }
            }

            if (currentElement != null)
            {
                invalidFrames++;
                if (invalidFrames > deselectAfterFrames)
                {
                    invalidFrames = 0;
                    currentElement.EndHover();
                    currentElement = null;
                    lineRenderer.enabled = false;
                    reticleObject.SetActive(false);
                }
                return;
            }

            //lineRenderer.enabled = false;
            //reticleObject.SetActive(false);
        }

        public void EnableUiInteraction()
        {
            canInteract = true;
        }

        /// <summary>
        /// Use to disable the ability for the user to interact iwth custom ui elements.
        /// </summary>
        public void DisableUiInteraction()
        {
            if (currentElement != null)
            {
                currentElement.EndHover();
                currentElement = null;
            }
            invalidFrames = 0;
            lineRenderer.enabled = false;
            reticleObject.SetActive(false);
            canInteract = false;
        }
    }
}
