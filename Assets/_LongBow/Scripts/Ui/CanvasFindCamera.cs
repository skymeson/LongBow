/// <summary>
/// Add to a worldspace canvas to automatically set the camera to Camera.main.
/// </summary>
namespace LongBow
{
    using UnityEngine;

    public class CanvasFindCamera : MonoBehaviour
    {
        private void Start()
        {
            var _canvas = GetComponent<Canvas>();
            if (_canvas.renderMode == RenderMode.WorldSpace && _canvas.worldCamera == null)
            {
                _canvas.worldCamera = Camera.main;
            }
        }
    }
}