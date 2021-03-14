/// <summary>
/// Handle all the logic needed for the game menu.
/// </summary>
namespace LongBow
{
    using Photon.Pun;
    using UnityEngine;
    using UnityEngine.UI;

    // TODO:
    // add mic settings to this menu also
    // player should be able to mute and echo while in game

    public class GameMenu : MonoBehaviour
    {
        [Header("Object Links")]
        [SerializeField] private GameObject menuPanel = default;
        [SerializeField] private Text playerNamesText = default;

        private void Start()
        {
            UpdatePlayerList();
            menuPanel.SetActive(true);
        }

        private void UpdatePlayerList()
        {
            string _playerList = "";
            if (PhotonNetwork.InRoom)
            {
                foreach (var player in PhotonNetwork.CurrentRoom.Players)
                {
                    _playerList += player.Value.NickName;
                    if (player.Value.IsMasterClient)
                    {
                        _playerList += "*";
                    }
                    _playerList += "\n";
                }
            }
            playerNamesText.text = _playerList;
        }
    }
}
