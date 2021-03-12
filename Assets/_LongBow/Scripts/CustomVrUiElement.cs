namespace LongBow
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class CustomVrUiElement : MonoBehaviour
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

        public void BeginHover()
        {
            pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(thisObject, pointer, ExecuteEvents.pointerEnterHandler);
        }

        public void EndHover()
        {
            if (pointer != null)
            {
                ExecuteEvents.Execute(thisObject, pointer, ExecuteEvents.pointerExitHandler);
                pointer = null;
            }
        }

        public void OnTriggerButton()
        {
            EndHover();
            if (button != null)
            {
                button.onClick.Invoke();
                Debug.Log(gameObject.name + " was clicked");
            }
            if (toggle != null)
            {
                var _currentValue = toggle.isOn;
                toggle.onValueChanged.Invoke(!_currentValue);
                toggle.isOn = !_currentValue;
                Debug.Log(gameObject.name + " was toggled");
            }
        }
    }
}
