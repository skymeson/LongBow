namespace LongBow
{
    using Photon.Pun;
    using Photon.Voice.Unity;
    using UnityEngine;

    public class NetworkAvatarVoice : MonoBehaviour
    {
        [SerializeField] private GameObject openMouth = default;
        [SerializeField] private GameObject closeMouth = default;
        [SerializeField] private float threshold = 0.4f;

        private PhotonView view;
        private Speaker speaker;
        private bool wasTalking = false;
        private AudioSource audioSource;

        private void Awake()
        {
            view = GetComponent<PhotonView>();
        }

        private void Start()
        {
            SetupAudioSource();
            speaker = GetComponent<Speaker>();
            openMouth.SetActive(false);

            if (view.IsMine)
            {
                this.enabled = false;
            }
        }

        private void Update()
        {
            // move mouth when talking
            var _isTalking = speaker.IsPlaying  && audioSource != null && audioSource.volume > threshold;

            // if going from quiet to loud open mouth
            if (_isTalking && !wasTalking)
            {
                openMouth.SetActive(true);
            }
            // if going from loud to quiet close mouth
            else if (!_isTalking && wasTalking)
            {
                openMouth.SetActive(false);
            }

            wasTalking = _isTalking;
        }

        private void SetupAudioSource()
        {
            audioSource = GetComponent<AudioSource>();
            int _attempts = 0;

            while (audioSource == null && _attempts < 1000)
            {
                _attempts++;
                audioSource = GetComponent<AudioSource>();
            }

            if (audioSource == null)
            {
                Debug.LogError("No audio source found.", this);
                return;
            }

            // SETTINGS FOR VOICE SOURCE
            //_audio.spatialBlend = 0;
            //_audio.spatialize = false;
            //_audio.minDistance = 0;
            //_audio.maxDistance = 500;
            //_audio.panStereo = 0;
            //_audio.reverbZoneMix = 0;
        }
    }
}
