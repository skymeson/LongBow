namespace LongBow
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class GroundManager : MonoBehaviour
    {
        [SerializeField] private GameObject groundOne = default;
        [SerializeField] private GameObject groundTwo = default;
        [SerializeField] private InputActionReference switchInput = default;

        private void OnEnable()
        {
            switchInput.action.Enable();
            switchInput.action.performed += Action_performed;
        }

        private void OnDisable()
        {
            switchInput.action.Disable();
            switchInput.action.performed -= Action_performed;
        }

        private void Action_performed(InputAction.CallbackContext obj)
        {
            if (groundOne.activeSelf)
            {
                groundOne.SetActive(false);
                groundTwo.SetActive(true);
            }
            else
            {
                groundTwo.SetActive(false);
                groundOne.SetActive(true);
            }
        }
    }
}
