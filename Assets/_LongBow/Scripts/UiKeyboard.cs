namespace LongBow
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UiKeyboard : MonoBehaviour
    {
        private string currentKey = null;
        private string currentValue = null;
        [SerializeField] private GameObject keyboardObject = default;
        [SerializeField] private Text textDisplay = default;
        [SerializeField] private Text instructionDisplay = default;

        public MainMenu CallbackMenu { get; set; }

        private void Start()
        {
            DisableKeyboard();
        }

        public void EnableKeyboard(string key)
        {
            currentKey = key;
            keyboardObject.SetActive(true);
            textDisplay.text = "";
            if (currentKey == "PlayerName")
            {
                instructionDisplay.text = "Enter your name:";
            }
            else if (currentKey == "RoomName")
            {
                instructionDisplay.text = "Enter the room name:";
            }
            if (PlayerPrefs.HasKey(currentKey))
            {
                var _ppString = PlayerPrefs.GetString(currentKey);
                currentValue = _ppString;
                textDisplay.text = _ppString;
            }
        }

        private void DisableKeyboard()
        {
            currentKey = null;
            currentValue = null;
            textDisplay.text = "";
            instructionDisplay.text = "";
            keyboardObject.SetActive(false);
        }

        public void AddCharacter(string characterToAdd)
        {
            currentValue += characterToAdd;
            textDisplay.text = currentValue;
        }

        public void DeleteCharacter()
        {
            if (currentValue.Length == 0) return;
            string _updatedString = currentValue.Substring(0, currentValue.Length - 1);
            currentValue = _updatedString;
            textDisplay.text = _updatedString;
        }

        public void SubmitString()
        {
            if (string.IsNullOrEmpty(currentValue)) return;
            PlayerPrefs.SetString(currentKey, currentValue);
            Debug.Log("Setting " + currentKey + " to " + currentValue);
            string _updatedKey = currentKey;
            DisableKeyboard();
            CallbackMenu?.OnKeyboardClosed(_updatedKey);
        }

        public void OnNetworkFail()
        {
            DisableKeyboard();
        }
    }
}
