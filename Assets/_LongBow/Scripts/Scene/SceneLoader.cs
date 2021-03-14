/// <summary>
/// Used to handle the starting scene and scene transitions
/// </summary>
namespace LongBow
{
    using ScriptableObjectArchitecture;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SceneLoader : MonoBehaviour
    {
        [Header("Scene Index List")]
        [SerializeField] private int playerOnlySceneIndex = 0;
        [SerializeField] private int menuSceneIndex = 1;

        [Header("Variables and Events")]
        [SerializeField] private Vector3GameEvent teleportPlayerEvent = default;
        [SerializeField] private Vector3Reference startLocation = default;

        [Header("Settings")]
        [SerializeField] private Vector3 playerMenuScenePosition = new Vector3(0, -100, 0);

        public static SceneLoader Instance { get; private set; }
        private AsyncOperation sceneLoadOperation = null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            LoadMenuSceneIfNeeded();
        }

        // we need to make sure a scene is always loaded, fallback to the menu
        private void LoadMenuSceneIfNeeded()
        {
            var _loadedSceneCount = SceneManager.sceneCount;
            if (_loadedSceneCount > 1) return;
            if (SceneManager.GetSceneAt(0).buildIndex != playerOnlySceneIndex)
            {
                Debug.LogError("You somehow ended up without having the player scene laoded.", this);
            }
            SceneManager.LoadSceneAsync(menuSceneIndex, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Call to return to the menu.
        /// </summary>
        public void LoadMenuScene()
        {
            // make sure scene isn't already loaded
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).buildIndex == menuSceneIndex)
                {
                    Debug.LogError("Trying to load a scene that is already loaded.", this);
                    return;
                }
            }
            // load menu scene async
            sceneLoadOperation = SceneManager.LoadSceneAsync(menuSceneIndex, LoadSceneMode.Additive);
            sceneLoadOperation.completed += MenuSceneLoadOperationCompleted;
        }

        /// <summary>
        /// Call to load a new scene.
        /// </summary>
        /// <param name="sceneIndex">The index of the scene to load.</param>
        public void LoadGameScene(int sceneIndex)
        {
            // ignore this if it's the player or menu scene
            if (sceneIndex < 2) return;
            // make sure scene isn't already loaded
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).buildIndex == sceneIndex)
                {
                    Debug.LogError("Trying to load a scene that is already loaded.", this);
                    return;
                }
            }
            // load new scene async
            sceneLoadOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            sceneLoadOperation.completed += GameSceneLoadOperationCompleted;
        }

        private void GameSceneLoadOperationCompleted(AsyncOperation obj)
        {
            // teleport player to starting location
            teleportPlayerEvent.Raise(startLocation.Value);
            // unload menu scene
            SceneManager.UnloadSceneAsync(menuSceneIndex);
            // reset
            sceneLoadOperation = null;
        }

        private void MenuSceneLoadOperationCompleted(AsyncOperation obj)
        {
            // teleport player to starting location
            teleportPlayerEvent.Raise(playerMenuScenePosition);
            // unload game scene
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var _sceneIndex = SceneManager.GetSceneAt(i).buildIndex;
                if (_sceneIndex != playerOnlySceneIndex && _sceneIndex != menuSceneIndex)
                {
                    SceneManager.UnloadSceneAsync(_sceneIndex);
                }
            }
            // reset
            sceneLoadOperation = null;
        }
    }
}
