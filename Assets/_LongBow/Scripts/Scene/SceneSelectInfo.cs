/// <summary>
/// Add this to a scene selection button to set the index and player position.
/// </summary>
namespace LongBow
{
    using Photon.Pun;
    using Photon.Realtime;
    using ScriptableObjectArchitecture;
    using UnityEngine;
    using UnityEngine.UI;

    public class SceneSelectInfo : MonoBehaviourPunCallbacks
    {
        [Header("Variables and Events")]
        [SerializeField] private Vector3Reference startingPositionVariable = default;
        [SerializeField] private IntGameEvent loadSceneEvent = default;
        [Header("Settings")]
        [SerializeField] private int sceneIndex = default;
        [SerializeField] private Vector3 startingPosition = default;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            if(button == null)
            {
                Debug.LogError("A scene selection button is set incorrectly.", this);
                this.enabled = false;
            }
        }

        public override void OnEnable()
        {
            button.onClick.AddListener(SetSceneInfo);
            if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            {
                button.interactable = false;
            }
            base.OnEnable();
        }

        public override void OnDisable()
        {
            button.onClick.RemoveListener(SetSceneInfo);
            base.OnDisable();
        }

        private void SetSceneInfo()
        {
            startingPositionVariable.Value = startingPosition;
            loadSceneEvent.Raise(sceneIndex);
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient) return;
            button.interactable = false;
        }

        public override void OnLeftRoom()
        {
            button.interactable = true;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            button.interactable = true;
        }
    }
}
