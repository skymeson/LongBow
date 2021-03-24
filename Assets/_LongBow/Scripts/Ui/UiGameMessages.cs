/// <summary>
/// This is used to show messages to the player on an in-game worldspace canvas.
/// </summary>
namespace LongBow
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UiGameMessages : MonoBehaviour
    {
        public static UiGameMessages Instance { get; private set; }

        [SerializeField] private GameObject toggleObject = default;

        private Text messageText;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                messageText = GetComponentInChildren<Text>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Adds a message to the UI.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public void Log(string message)
        {
            messageText.text = message;
            toggleObject.SetActive(true);
        }

        /// <summary>
        /// Hide the ui object.
        /// </summary>
        public void Hide()
        {
            toggleObject.SetActive(false);
            messageText.text = "";
        }
    }
}
