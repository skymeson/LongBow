/// <summary>
/// Fade the user camera to black.  Useful for reducing motion sickness.
/// </summary>
namespace LongBow
{
    using ScriptableObjectArchitecture;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    namespace Longbow
    {
        public class UiCameraFade : MonoBehaviour
        {
            [SerializeField] private float defaultFadeTime = 0.2f;
            [SerializeField] private GameEvent fadeCompleteEvent = default;

            private Image image;
            private bool isFading = false;
            private bool isClearingFade = false;
            private float currentFadeTime = 0;
            private float currentAlpha = 0;

            private void Awake()
            {
                image = GetComponentInChildren<Image>();
            }

            private void Start()
            {
                SetColor();
            }

            private void Update()
            {
                if (!isFading && !isClearingFade) return;
                if (isFading && isClearingFade)
                {
                    Debug.LogError("You are fading and clearing at the same time.", this);
                }

                if (isFading)
                {
                    currentAlpha = Mathf.MoveTowards(currentAlpha, 1, (1 / currentFadeTime) * Time.deltaTime);
                    SetColor();

                    if (currentAlpha >= 1)
                    {
                        OnFadeComplete();
                    }
                }
                else if (isClearingFade)
                {
                    currentAlpha = Mathf.MoveTowards(currentAlpha, 0, (1 / currentFadeTime) * Time.deltaTime);
                    SetColor();

                    if (currentAlpha <= 0)
                    {
                        OnClearFadeComplete();
                    }
                }
            }

            /// <summary>
            /// Fades the screen and immediately clears it.
            /// Generally used for player teleportation.
            /// </summary>
            /// <param name="fadeTime">The time it takes to fade.  0 will use default time.</param>
            public void FadeAndClear(float fadeTime = 0)
            {
                BeginFade(fadeTime);
                StartCoroutine(ClearFadeRoutine());
            }

            private IEnumerator ClearFadeRoutine()
            {
                yield return new WaitForSeconds(currentFadeTime);
                yield return null;
                ClearFade();
            }

            /// <summary>
            /// Fades the screen.
            /// Need to call ClearFade to eventually remove fade.
            /// </summary>
            /// <param name="fadeTime">The time it takes to fade.  0 will use default time.</param>
            public void BeginFade(float fadeTime = 0)
            {
                currentFadeTime = fadeTime > 0 ?
                    fadeTime : defaultFadeTime;
                isFading = true;
            }

            /// <summary>
            /// Clears the faded screen.
            /// </summary>
            public void ClearFade(float fadeTime = 0)
            {
                currentFadeTime = fadeTime > 0 ?
                    fadeTime : defaultFadeTime;
                isClearingFade = true;
            }

            private void OnFadeComplete()
            {
                isFading = false;
                currentAlpha = 1;
                SetColor();
                fadeCompleteEvent?.Raise();
            }

            private void OnClearFadeComplete()
            {
                isClearingFade = false;
                currentAlpha = 0;
                SetColor();
            }

            private void SetColor()
            {
                var _color = Color.black;
                _color.a = currentAlpha;
                image.color = _color;
            }
        }
    }
}