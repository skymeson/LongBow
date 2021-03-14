/// <summary>
/// Add this to a scene selection button to set the index and player position.
/// </summary>
namespace LongBow
{
    using ScriptableObjectArchitecture;
    using UnityEngine;
    using UnityEngine.UI;

    public class SceneSelectInfo : MonoBehaviour
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

        private void OnEnable()
        {
            button.onClick.AddListener(SetSceneInfo);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(SetSceneInfo);
        }

        private void SetSceneInfo()
        {
            startingPositionVariable.Value = startingPosition;
            loadSceneEvent.Raise(sceneIndex);
        }
    }
}
