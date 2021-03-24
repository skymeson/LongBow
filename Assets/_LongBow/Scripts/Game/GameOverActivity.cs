/// <summary>
/// Do all the things that need to happen when the game ends.
/// Moved into it's own component to reduce the game manager size.
/// </summary>
namespace LongBow
{
    using UnityEngine;

    public class GameOverActivity : MonoBehaviour
    {
        public void OnGameWon()
        {
            // alert the player on their headset
            UiHeadsetMessages.Instance.Log("All waves have been defeated!");
            // set the in-game ui
            UiGameMessages.Instance.Log("All waves have been defeated!");
        }

        public void OnGameLost()
        {
            // alert the player on their headset
            UiHeadsetMessages.Instance.Log("You have been defeated!");
            // set the in-game ui
            UiGameMessages.Instance.Log("You have been defeated!");
        }
    }
}
