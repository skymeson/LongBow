/// <summary>
/// This makes the magic happen.
/// </summary>
namespace LongBow
{
    using Photon.Pun;
    using ScriptableObjectArchitecture;
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        // TODO:
        // this whole thing is WIP

        [Header("Variables")]
        [SerializeField] private IntReference currentHealth = default;
        [Header("Settings")]
        [SerializeField] private int startingHealth = 10;
        [Header("Object Links")]
        [SerializeField] private Transform startingPosition = default;
        [Header("Events")]
        [SerializeField] private GameEvent gameWonEvent = default;
        [SerializeField] private GameEvent gameLostEvent = default;

        public static GameManager Instance { get; private set; }
        private PhotonView view;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
            }

            Instance = this;
            view = gameObject.AddComponent<PhotonView>();
            view.ViewID = 999;
        }

        private void Start()
        {
            currentHealth.Value = startingHealth;
        }

        public Transform GetStartingPosition
        {
            get { return startingPosition; }
        }

        public void OnGameWon()
        {
            // all waves are spawned, all mobs are defeated, gate still has health
            gameWonEvent?.Raise();
        }

        public void OnGameLost()
        {
            // gate health <= 0
            gameLostEvent?.Raise();
        }
    }
}
