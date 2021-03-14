namespace LongBow
{
    using ExitGames.Client.Photon;
    using Photon.Pun;
    using ScriptableObjectArchitecture;
    using UnityEngine;

    public class NetworkEventManager : MonoBehaviour
    {
        public const byte LoadGameSceneEvent = 0;
        [SerializeField] private IntGameEvent loadSceneEvent = default;

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        public void OnEvent(EventData eventData)
        {
            byte eventCode = eventData.Code;
            object[] data = (object[])eventData.CustomData;

            switch (eventCode)
            {
                case LoadGameSceneEvent:
                    OnLoadSceneEvent(data);
                    break;
            }
        }

        private void OnLoadSceneEvent(object[] data)
        {
            int sceneIndex = (int)data[0];
            loadSceneEvent.Raise(sceneIndex);
            Debug.Log("PUN Event: scene change to: " + sceneIndex);
        }
    }
}
