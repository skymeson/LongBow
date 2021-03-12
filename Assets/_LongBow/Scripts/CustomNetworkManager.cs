namespace LongBow
{
    using Photon.Pun;
    using ScriptableObjectArchitecture;
    using UnityEngine;

    public class CustomNetworkManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject networkPrefab = default;
        [SerializeField] private GameEvent networkFailEvent = default;

        private readonly string ppName = PlayerPrefsKeys.PlayerName;
        private readonly string rName = PlayerPrefsKeys.RoomName;

        public static CustomNetworkManager Instance { get; private set; }
        public static string PlayerName { get; set; }
        public static string RoomName { get; set; }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void JoinRoom()
        {
            if (PhotonNetwork.InRoom) return;
            if (!PlayerPrefs.HasKey(ppName) || !PlayerPrefs.HasKey(rName)) return;

            string _playername = PlayerPrefs.GetString(ppName);
            string _roomname = PlayerPrefs.GetString(rName);

            if (string.IsNullOrEmpty(_playername) || string.IsNullOrEmpty(_roomname)) return;

            PlayerName = _playername;
            RoomName = _roomname;

            Debug.Log("Player: " + PlayerName);
            Debug.Log("Room: " + RoomName);

            PhotonNetwork.NickName = _playername;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }

        public void LeaveRoom()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }

        private void SpawnAvatar()
        {
            PhotonNetwork.Instantiate(networkPrefab.name, Vector3.zero, Quaternion.identity);
            Debug.Log("Spawning network avatar for " + PhotonNetwork.NickName);
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinRoom(RoomName);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            PhotonNetwork.CreateRoom(RoomName, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            networkFailEvent?.Raise();
            Debug.LogError("Failed to create room: " + message, this);
        }

        public override void OnJoinedRoom()
        {
            SpawnAvatar();
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }
    }
}
