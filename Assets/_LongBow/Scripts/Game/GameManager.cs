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
        [Header("Variables")]
        [SerializeField] private IntReference currentHealth = default;
        [Header("Settings")]
        [SerializeField] private int startingHealth = 10;

        private PhotonView view;

        private void Awake()
        {
            view = gameObject.AddComponent<PhotonView>();
            view.ViewID = 999;
        }

        private void Start()
        {
            currentHealth.Value = startingHealth;
        }
    }
}
