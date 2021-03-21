/// <summary>
/// Object Pool specifically for arrows.
/// </summary>
namespace LongBow
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ArrowPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab = default;
        [SerializeField] private int startingAmount = 100;

        public static ArrowPool Instance;

        private Stack<GameObject> disabledArrows;
        private List<GameObject> enabledArrows;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            CreatePool();
        }

        private void CreatePool()
        {
            for (int i = 0; i < startingAmount; i++)
            {
                var _arrow = Instantiate(prefab);
                _arrow.SetActive(false);
                disabledArrows.Push(_arrow);
            }
        }

        public GameObject Spawn()
        {
            if (disabledArrows.Count == 0)
            {
                Debug.LogError("No arrows to spawn.  Consider increasing starting amount.", this);
                return null;
            }

            var _arrow = disabledArrows.Pop();
            enabledArrows.Add(_arrow);
            return _arrow;
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            if (disabledArrows.Count == 0)
            {
                Debug.LogError("No arrows to spawn.  Consider increasing starting amount.", this);
                return null;
            }

            var _arrow = disabledArrows.Pop();
            enabledArrows.Add(_arrow);
            _arrow.transform.position = position;
            _arrow.transform.rotation = rotation;
            return _arrow;
        }

        public void Despawn(GameObject arrow)
        {
            if (!enabledArrows.Contains(arrow))
            {
                Debug.LogError("The object you are attempting to despawn was not created by this pool.", this);
                return;
            }

            for (int i = enabledArrows.Count; i > -1; i--)
            {
                if (enabledArrows[i] == arrow)
                {
                    var _arrow = enabledArrows[i];
                    enabledArrows.RemoveAt(i);
                    disabledArrows.Push(_arrow);
                    return;
                }
            }
        }
    }
}