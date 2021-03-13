/// <summary>
/// Handles the Photon Voice settings.
/// </summary>
namespace LongBow
{
    using Photon.Voice.Unity;
    using UnityEngine;

    public class VoiceChatManager : MonoBehaviour
    {
        private static VoiceChatManager instance;
        private Recorder recorder;

        private readonly string micEnabledKey = PlayerPrefsKeys.MicEnabled;
        private readonly string echoKey = PlayerPrefsKeys.MicEcho;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                recorder = GetComponent<Recorder>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            OnSettingsUpdated();
        }

        /// <summary>
        /// Call when mic settings are altered to match the recorder settings.
        /// </summary>
        public void OnSettingsUpdated()
        {
            bool _micEnabled = true;
            bool _micEcho = true;
            if (PlayerPrefs.HasKey(micEnabledKey))
            {
                _micEnabled = PlayerPrefs.GetInt(micEnabledKey) == 1 ? true : false;
            }
            if (PlayerPrefs.HasKey(echoKey))
            {
                _micEcho = PlayerPrefs.GetInt(echoKey) == 1 ? true : false;
            }

            recorder.TransmitEnabled = _micEnabled;
            recorder.DebugEchoMode = _micEcho;

            //Debug.Log("Mic Enabled: " + _micEnabled);
            //Debug.Log("Echo Mic: " + _micEcho);
        }
    }
}
