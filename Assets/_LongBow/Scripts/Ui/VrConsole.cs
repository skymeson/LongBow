﻿/// <summary>
/// A worldspace canvas and text used to show an in-game console.
/// </summary>
namespace LongBow
{
    using UnityEngine;
    using UnityEngine.UI;

    public class VrConsole : MonoBehaviour
    {
        private Text text;
        private static VrConsole instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                text = GetComponentInChildren<Text>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Add a message to the console.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Log(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            if (instance == null || instance.text == null) return;

            var _currentLog = instance.text.text;
            string _updatedLog = message + "\n" + _currentLog;
            instance.text.text = _updatedLog;

            // TODO:
            // set a max number of messages and clear out the older ones
        }
    }
}