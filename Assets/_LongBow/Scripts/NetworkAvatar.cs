/// <summary>
/// Used to sync a players avatar (head and hands) over PUN.
/// </summary>
namespace LongBow
{
    using Photon.Pun;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class NetworkAvatar : MonoBehaviour, IPunObservable
    {
        [Header("Object Links")]
        [SerializeField] private Transform headTransform = default;
        [SerializeField] private Transform leftHandTransform = default;
        [SerializeField] private Transform rightHandTransform = default;
        [SerializeField] private Text nameTagText = default;

        [Header("Hidden On Local")]
        [SerializeField] private List<Renderer> renderersToHide = default;

        private Transform localHead;
        private Transform localLeftHand;
        private Transform localRightHand;
        private PhotonView view;
        private Canvas canvas;

        private void Awake()
        {
            view = GetComponent<PhotonView>();
            canvas = GetComponentInChildren<Canvas>();
        }

        private void Start()
        {
            nameTagText.text = view.Owner.NickName;
            if (!view.IsMine) return;

            if (canvas != null)
            {
                canvas.gameObject.SetActive(false);
            }

            try
            {
                var localPlayerItems = FindObjectsOfType<AvatarItemLink>();
                localHead = localPlayerItems.Where(x => x.AvatarItem == AvatarItemLink.AvatarItems.Head).First().transform;
                localLeftHand = localPlayerItems.Where(x => x.AvatarItem == AvatarItemLink.AvatarItems.LeftHand).First().transform;
                localRightHand = localPlayerItems.Where(x => x.AvatarItem == AvatarItemLink.AvatarItems.RightHand).First().transform;

                headTransform.parent = localHead;
                headTransform.localPosition = Vector3.zero;
                headTransform.localRotation = Quaternion.identity;

                leftHandTransform.parent = localLeftHand;
                leftHandTransform.localPosition = Vector3.zero;
                leftHandTransform.localRotation = Quaternion.identity;

                rightHandTransform.parent = localRightHand;
                rightHandTransform.localPosition = Vector3.zero;
                rightHandTransform.localRotation = Quaternion.identity;
            }
            catch (Exception ex)
            {
                Debug.LogError("Local player transforms not set properly: " + ex, this);
            }

            foreach (var renderer in renderersToHide)
            {
                renderer.enabled = false;
            }
        }

        /// <summary>
        /// Send and receive the network stream.
        /// </summary>
        /// <param name="stream">PUN stuff.</param>
        /// <param name="info">PUN stuff.</param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // TODO:
            // lerp to hide lag

            if (stream.IsWriting)
            {
                // send transform info
                stream.SendNext(headTransform.position);
                stream.SendNext(headTransform.rotation);

                stream.SendNext(leftHandTransform.position);
                stream.SendNext(leftHandTransform.rotation);

                stream.SendNext(rightHandTransform.position);
                stream.SendNext(rightHandTransform.rotation);
            }
            else
            {
                // receive transform info
                headTransform.position = (Vector3)stream.ReceiveNext();
                headTransform.rotation = (Quaternion)stream.ReceiveNext();

                leftHandTransform.position = (Vector3)stream.ReceiveNext();
                leftHandTransform.rotation = (Quaternion)stream.ReceiveNext();

                rightHandTransform.position = (Vector3)stream.ReceiveNext();
                rightHandTransform.rotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}