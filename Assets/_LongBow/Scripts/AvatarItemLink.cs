/// <summary>
/// Add this component to a tracked vr item to create a link between it and it's related network avatar item.
/// </summary>
namespace LongBow
{
    using UnityEngine;

    public class AvatarItemLink : MonoBehaviour
    {
        /// <summary>
        /// The possible vr items this could be.
        /// </summary>
        public enum AvatarItems
        {
            Head = 0,
            LeftHand = 1,
            RightHand = 2
        }

        [SerializeField] private AvatarItems thisItem = default;

        public AvatarItems AvatarItem { get; private set; }

        private void Awake()
        {
            AvatarItem = thisItem;
        }
    }
}
