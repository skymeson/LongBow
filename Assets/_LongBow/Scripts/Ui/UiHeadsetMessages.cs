/// <summary>
/// This is used to show messages to the player on a headset-attached canvas.
/// </summary>
namespace LongBow
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;

    public class UiHeadsetMessages : MonoBehaviour
    {
        private Queue<string> messageQueue = new Queue<string>();
        public static UiHeadsetMessages Instance { get; private set; }

        [SerializeField] private GameObject toggleObject = default;
        [SerializeField] private float messageDisplayTime = 3;
        [SerializeField] private float betweenMessageTime = 0.2f;
        [SerializeField] private InputActionReference debugInput = default;

        private Text messageText;
        private WaitForSeconds delay;
        private WaitForSeconds show;
        private bool isActive = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                messageText = GetComponentInChildren<Text>();
                delay = new WaitForSeconds(messageDisplayTime);
                show = new WaitForSeconds(betweenMessageTime);
            }
            else
            {
                Debug.LogError("You ended up with multiple headset message canvases (canvii?).", this);
            }
        }

        private void Start()
        {
            toggleObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (debugInput != null)
            {
                debugInput.action.Enable();
                debugInput.action.performed += Action_performed;
            }
        }

        private void OnDisable()
        {
            if (debugInput != null)
            {
                debugInput.action.Disable();
                debugInput.action.performed -= Action_performed;
            }
        }

        private void Action_performed(InputAction.CallbackContext obj)
        {
            Log("Hi.  This is a test message.");
        }

        /// <summary>
        /// Adds a messages to the players headset ui.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public void Log(string message)
        {
            messageQueue.Enqueue(message);
            if (isActive) return;
            StartCoroutine(ShowMessagesRoutine());
        }

        private IEnumerator ShowMessagesRoutine()
        {
            isActive = true;

            while (messageQueue.Count > 0)
            {
                messageText.text = messageQueue.Dequeue();
                toggleObject.SetActive(true);
                yield return delay;
                toggleObject.SetActive(false);
                yield return show;
            }

            isActive = false;
        }
    }
}
