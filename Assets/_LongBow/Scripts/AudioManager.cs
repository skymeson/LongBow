namespace LongBow
{
    using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        private AudioSource musicSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                musicSource = GetComponent<AudioSource>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (musicSource.clip != null)
            {
                musicSource.Play();
            }
        }
    }
}
