namespace LongBow
{
    using Photon.Pun;
    using Photon.Realtime;
    using ScriptableObjectArchitecture;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class MainMenu : MonoBehaviourPunCallbacks
    {
        [Header("Object Links")]
        [SerializeField] private GameObject joinButton = default;
        [SerializeField] private GameObject leaveButton = default;
        [SerializeField] private GameObject menuPanel = default;
        [SerializeField] private Text playerNamesText = default;
        [SerializeField] private Toggle micToggle = default;
        [SerializeField] private Toggle echoToggle = default;
        [Header("Events")]
        [SerializeField] private GameEvent uiJoinRoomEvent = default;
        [SerializeField] private GameEvent uiLeaveRoomEvent = default;
        [SerializeField] private GameEvent settingsUpdated = default;

        private UiKeyboard keyboard;
        private readonly string micEnabledKey = PlayerPrefsKeys.MicEnabled;
        private readonly string echoKey = PlayerPrefsKeys.MicEcho;

        private void Awake()
        {
            keyboard = GetComponent<UiKeyboard>();
            keyboard.CallbackMenu = this;

            var _cam = Camera.main;
            var _canvas = GetComponentInChildren<Canvas>();
            if (_cam && _canvas)
            {
                _canvas.worldCamera = _cam;
            }
        }

        private void Start()
        {
            UpdatePlayerList();
            menuPanel.SetActive(true);
            if (PhotonNetwork.InRoom)
            {
                joinButton.SetActive(false);
                leaveButton.SetActive(true);
            }
            else
            {
                leaveButton.SetActive(false);
                joinButton.SetActive(true);
            }

            bool _micEnabled = true;
            bool _micEcho = true;
            if (PlayerPrefs.HasKey(micEnabledKey))
            {
                _micEnabled = PlayerPrefs.GetInt(micEnabledKey) == 1 ? true : false;
            }
            micToggle.isOn = _micEnabled;
            if (PlayerPrefs.HasKey(echoKey))
            {
                _micEcho = PlayerPrefs.GetInt(echoKey) == 1 ? true : false;
            }
            echoToggle.isOn = _micEcho;
        }

        public override void OnEnable()
        {
            micToggle.onValueChanged.AddListener(OnMicToggled);
            echoToggle.onValueChanged.AddListener(OnEchoToggled);
            base.OnEnable();
        }

        public override void OnDisable()
        {
            micToggle.onValueChanged.RemoveListener(OnMicToggled);
            echoToggle.onValueChanged.RemoveListener(OnEchoToggled);
            base.OnDisable();
        }

        private void OnMicToggled(bool value)
        {
            int _enabled = value ? 1 : 0;
            PlayerPrefs.SetInt(micEnabledKey, _enabled);
            settingsUpdated?.Raise();
        }

        private void OnEchoToggled(bool value)
        {
            int _echo = value ? 1 : 0;
            PlayerPrefs.SetInt(echoKey, _echo);
            settingsUpdated.Raise();
        }

        public void OnJoinGamePressed()
        {
            joinButton.SetActive(false);
            menuPanel.SetActive(false);
            //keyboard.EnableKeyboard("PlayerName");
            StartCoroutine(DelayPanelRoutine("PlayerName"));
        }

        public void OnKeyboardClosed(string key)
        {
            if (key == "PlayerName")
            {
                //keyboard.EnableKeyboard("RoomName");
                StartCoroutine(DelayPanelRoutine("RoomName"));
            }
            else if (key == "RoomName")
            {
                uiJoinRoomEvent?.Raise();
                joinButton.SetActive(false);
                leaveButton.SetActive(false);
                menuPanel.SetActive(true);
            }
        }

        private IEnumerator DelayPanelRoutine(string keyboardMenu)
        {
            yield return new WaitForSeconds(0.3f);
            keyboard.EnableKeyboard(keyboardMenu);
        }

        public void OnLeaveGamePressed()
        {
            uiLeaveRoomEvent?.Raise();
        }

        private void UpdatePlayerList()
        {
            string _playerList = "";
            if (PhotonNetwork.InRoom)
            {
                foreach (var player in PhotonNetwork.CurrentRoom.Players)
                {
                    _playerList += player.Value.NickName;
                    if (player.Value.IsMasterClient)
                    {
                        _playerList += "*";
                    }
                    _playerList += "\n";
                }
            }
            playerNamesText.text = _playerList;
        }

        public override void OnJoinedRoom()
        {
            UpdatePlayerList();
            joinButton.SetActive(false);
            leaveButton.SetActive(true);
            Debug.Log("Joined room " + PhotonNetwork.CurrentRoom.Name + " in region " + PhotonNetwork.CloudRegion + " as " + PhotonNetwork.NickName + ".");
        }

        public override void OnLeftRoom()
        {
            Debug.Log("Left room.");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            leaveButton.SetActive(false);
            joinButton.SetActive(true);
            UpdatePlayerList();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            UpdatePlayerList();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            UpdatePlayerList();
        }

        public void OnNetworkFail()
        {
            UpdatePlayerList();
            leaveButton.SetActive(false);
            joinButton.SetActive(true);
            menuPanel.SetActive(true);
        }
    }
}
