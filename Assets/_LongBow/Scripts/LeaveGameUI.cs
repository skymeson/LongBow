using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace LongBow
{
    public class LeaveGameUI : MonoBehaviourPunCallbacks
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("A scene selection button is set incorrectly.", this);
                this.enabled = false;
            }
        }

        public override void OnEnable()
        {
            button.onClick.AddListener(LeaveGame);
            if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            {
                button.interactable = false;
            }
            base.OnEnable();
        }

        public override void OnDisable()
        {
            button.onClick.RemoveListener(LeaveGame);
            base.OnDisable();
        }

        private void LeaveGame()
        {
            //startingPositionVariable.Value = startingPosition;
            //loadSceneEvent.Raise(sceneIndex);
            SceneLoader.Instance.LoadMenuScene(); 
        }

    }
}