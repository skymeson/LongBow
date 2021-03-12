namespace LongBow
{
    using ScriptableObjectArchitecture;
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class CustomRotationControllerV2 : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference rotateAction = default;
        [Header("Settings")]
        [SerializeField] private float snapAmount = 15f;
        [SerializeField] private float deadZone = 0.2f;
        [Header("Object Links")]
        [SerializeField] private Transform playerToRotate = default;
        [Header("Events")]
        [SerializeField] private GameEvent rotationEvent = default;

        private float rotationAmount = 0;
        private float xAxis;
        private float previousXInput;

        private void OnEnable()
        {
            rotateAction.action.Enable();
        }

        private void OnDisable()
        {
            rotateAction.action.Disable();
        }

        private void Update()
        {
            xAxis = GetAxisInput();
            DoSnapRotation(xAxis);
            previousXInput = xAxis;
        }

        private float GetAxisInput()
        {
            float _lastVal = 0;
            float _axisVal = rotateAction.action.ReadValue<Vector2>().x;

            if (_lastVal == 0)
            {
                _lastVal = _axisVal;
            }
            else if (_axisVal != 0 && _axisVal > _lastVal)
            {
                _lastVal = _axisVal;
            }
            return _lastVal;
        }

        private void DoSnapRotation(float xInput)
        {
            rotationAmount = 0;

            if (xInput >= deadZone && previousXInput < deadZone)
            {
                rotationAmount += snapAmount;
            }
            else if (xInput <= -deadZone && previousXInput > -deadZone)
            {
                rotationAmount -= snapAmount;
            }

            if (Math.Abs(rotationAmount) > 0)
            {
                playerToRotate.rotation = Quaternion.Euler(new Vector3(playerToRotate.eulerAngles.x, playerToRotate.eulerAngles.y + rotationAmount, playerToRotate.eulerAngles.z));
                rotationEvent?.Raise();
            }

        }
    }
}
