/// <summary>
/// Allows a button or toggle with a collider to be used with a CustomUiPointer
/// </summary>
namespace LongBow
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class CustomUiElement : MonoBehaviour
    {
        private Button button;
        private Toggle toggle;
        private GameObject thisObject;
        private PointerEventData pointer;

        private void Awake()
        {
            thisObject = gameObject;
            button = GetComponent<Button>();
            toggle = GetComponent<Toggle>();
        }

        /// <summary>
        /// Call when the object is first pointed at.
        /// </summary>
        public void BeginHover()
        {
            pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(thisObject, pointer, ExecuteEvents.pointerEnterHandler);
        }

        /// <summary>
        /// Call when the object is no longer being pointed at.
        /// </summary>
        public void EndHover()
        {
            if (pointer != null)
            {
                ExecuteEvents.Execute(thisObject, pointer, ExecuteEvents.pointerExitHandler);
                pointer = null;
            }
        }

        /// <summary>
        /// Call when the object is clicked.
        /// </summary>
        public void OnTriggerButton()
        {
            EndHover();
            if (button != null)
            {
                button.onClick.Invoke();
                //Debug.Log(gameObject.name + " was clicked");
            }
            if (toggle != null)
            {
                var _currentValue = toggle.isOn;
                toggle.onValueChanged.Invoke(!_currentValue);
                toggle.isOn = !_currentValue;
                //Debug.Log(gameObject.name + " was toggled");
            }
        }
    }
}
