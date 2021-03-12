namespace LongBow
{
    using UnityEngine;

    public class AvatarItemLink : MonoBehaviour
    {
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
