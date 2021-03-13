/// <summary>
/// Used to move the menu to an approprate location when the player teleports.
/// </summary>
namespace LongBow
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class MoveMenuOnTeleport : MonoBehaviour
    {
        private List<Transform> anchorTransforms;
        private Transform playerTransform;
        private Transform menuTransform;
        private WaitForSeconds delay;

        private void Awake()
        {
            playerTransform = Camera.main.transform;
            menuTransform = transform;
            delay = new WaitForSeconds(0.5f);
        }

        private void Start()
        {
            var _menuAnchors = FindObjectsOfType<MenuTeleportAnchor>();
            anchorTransforms = _menuAnchors.Select(x => x.transform).ToList();

            if (playerTransform == null || anchorTransforms.Count == 0)
            {
                Debug.LogError("Scene not setup properly for menu movement on teleport.", this);
                this.enabled = false;
            }

            // set proper location on scene start
            // so we don't have to do it in the editor
            OnPlayerTeleportEvent();
        }

        /// <summary>
        /// Call when the player teleports to move the menu with them.
        /// </summary>
        public void OnPlayerTeleportEvent()
        {
            StopAllCoroutines();
            StartCoroutine(MoveMenuRoutine());
        }

        private IEnumerator MoveMenuRoutine()
        {
            yield return delay;
            MoveMenu();
        }

        private void MoveMenu()
        {
            Transform _closestAnchor = null;
            float _leastDistance = 100;

            foreach (var anchor in anchorTransforms)
            {
                float _distance = Vector3.Distance(playerTransform.position, anchor.position);
                if (_distance < _leastDistance)
                {
                    _closestAnchor = anchor;
                    _leastDistance = _distance;
                }
            }

            if (_closestAnchor == null)
            {
                Debug.LogError("You messed up the menu teleport code.", this);
                return;
            }

            menuTransform.position = _closestAnchor.position;
            menuTransform.rotation = _closestAnchor.rotation;
        }
    }
}