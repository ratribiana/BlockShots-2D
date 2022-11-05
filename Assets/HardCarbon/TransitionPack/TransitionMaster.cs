using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HardCarbon.TransitionPack;

namespace HardCarbon.TransitionPack
{
    [AddComponentMenu("HardCarbon/Transition Master")]
    public class TransitionMaster : BaseSettings
    {
        [SerializeField] TransitionInSettings TransitionInSettings;
        [SerializeField] TransitionOutSettings TransitionOutSettings;
        
        GameObject obj;
        Shader _main;

        private void Awake()
        {
            _main = Resources.Load<Shader>("Shaders/Main") as Shader;

            if (autoRun)
                In();
        }

        #region Transition Calls

        /// <summary>
        /// Transition in with default properties
        /// </summary>
        public void In()
        {
            if (obj == null)
            {
                InitObject();
            }
            StartCoroutine("TransitionIn", TransitionInSettings);
        }


        /// <summary>
        /// Transition in with custom properties
        /// </summary>
        /// <param name="overlay">Overlay texture for the screen</param>
        /// <param name="color">Color of the image</param>
        /// <param name="mask">Mask texture to follow the gradient movement</param>
        /// <param name="invertMask">Invert mask option to invert transition</param>
        /// <param name="softness">Amount of gradient blur during transition</param>
        /// <param name="timeUpdateMethod">Update method of the transition</param>
        public void In(Texture2D overlay,Color color, Texture2D mask, bool invertMask, float softness, TransitionTime timeUpdateMethod)
        {
            TransitionInSettings settings = new TransitionInSettings();
            settings.overlay = overlay;
            settings.color = color;
            settings.mask = mask;
            settings.invertMask = invertMask;
            settings.softness = softness;
            settings.timeUpdateMethod = timeUpdateMethod;
            settings.events = new EventCalls();

            if (obj == null)
            {
                InitObject();
            }
            StartCoroutine("TransitionIn", settings);
        }

        /// <summary>
        /// Transition out with default properties
        /// </summary>
        public void Out()
        {
            if (obj == null)
            {
                InitObject();
            }
            StartCoroutine("TransitionOut", TransitionOutSettings);
        }
        /// <summary>
        /// Transition out with specific scene to open after the transition
        /// </summary>
        /// <param name="sceneName">Name of the scene to open after the transition</param>
        public void Out(string sceneName)
        {
            TransitionOutSettings settings = new TransitionOutSettings();
            settings.overlay = TransitionOutSettings.overlay;
            settings.color = TransitionOutSettings.color;
            settings.mask = TransitionOutSettings.mask;
            settings.invertMask = TransitionOutSettings.invertMask;
            settings.softness = TransitionOutSettings.softness;
            settings.sceneName = sceneName;
            settings.changeScene = (!string.IsNullOrEmpty(sceneName));
            settings.timeUpdateMethod = TransitionOutSettings.timeUpdateMethod;
            settings.events = new EventCalls();

            if (obj == null)
            {
                InitObject();
            }
            StartCoroutine("TransitionOut", settings);
        }

        /// <summary>
        /// Transition out with custom properties
        /// </summary>
        /// <param name="overlay">Overlay texture for the screen</param>
        /// <param name="color">Color of the image</param>
        /// <param name="mask">Mask texture to follow the gradient movement</param>
        /// <param name="invertMask">Invert mask option to invert transition</param>
        /// <param name="softness">Amount of gradient blur during transition</param>
        /// <param name="sceneName">Name of scene to open after the transition</param>
        /// <param name="timeUpdateMethod">Update method of the transition</param>
        public void Out(Texture2D overlay, Color color, Texture2D mask, bool invertMask, float softness, string sceneName, TransitionTime timeUpdateMethod)
        {
            TransitionOutSettings settings = new TransitionOutSettings();
            settings.overlay = overlay;
            settings.color = color;
            settings.mask = mask;
            settings.invertMask = invertMask;
            settings.softness = softness;
            settings.sceneName = sceneName;
            settings.changeScene = (!string.IsNullOrEmpty(sceneName));
            settings.timeUpdateMethod = timeUpdateMethod;
            settings.events = new EventCalls();

            
            if (obj == null)
            {
                InitObject();
            }
            StartCoroutine("TransitionOut", settings);
        }

        private void InitObject()
        {

            #region Instantiation
            GameObject screen = new GameObject("Screen");

            Canvas can = screen.AddComponent<Canvas>();
            screen.AddComponent<GraphicRaycaster>();
            can.renderMode = RenderMode.ScreenSpaceOverlay;
            can.sortingOrder = 999;
            can.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

            Vector2 screenSize = can.GetComponent<RectTransform>().sizeDelta;
            Vector2 objSize = (screenSize.x > screenSize.y)
                            ? new Vector2(screenSize.x, screenSize.x)
                            : new Vector2(screenSize.y, screenSize.y);

            obj = new GameObject("Overlay");
            obj.transform.SetParent(screen.transform);
            obj.AddComponent<RawImage>();   
            obj.AddComponent<CanvasGroup>();
            obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            obj.GetComponent<RectTransform>().sizeDelta = objSize;
            #endregion
        }

        #endregion

        #region Transitions
        IEnumerator TransitionIn(TransitionInSettings settings)
        {
            float timer = duration;
            Material mat = new Material(_main);
            mat.SetTexture("_MainTex", settings.overlay);
            mat.SetTexture("_MaskTex", settings.mask);
            mat.SetFloat("_Amount", 1);
            mat.SetColor("_Color", settings.color);
            if (settings.invertMask)
                mat.EnableKeyword("INVERT_MASK");
            else
                mat.DisableKeyword("INVERT_MASK");
            mat.SetFloat("_Softness", settings.softness);

            RawImage img = obj.GetComponent<RawImage>(); ;

            if (mat.GetTexture("_MaskTex") != null)
            {
                img.texture = mat.GetTexture("_MainTex");
                img.color = mat.GetColor("_Color");
                img.material = mat;
            }
            else
                DestroyImmediate(img);
            CanvasGroup cnvs = obj.GetComponent<CanvasGroup>();
            if (mat.GetTexture("_MaskTex") == null)
            {
                if (!obj.GetComponent<Image>())
                    obj.AddComponent<Image>().color = settings.color;
                cnvs.alpha = 1;
            }
            yield return new WaitForSeconds(delay);

            if(settings.events.OnStart != null)
                settings.events.OnStart.Invoke();

            while(timer > 0)
            {
                if(settings.events.OnUpdate != null)
                    settings.events.OnUpdate.Invoke();

                switch (settings.timeUpdateMethod)
                {
                    case TransitionTime.GameTime:
                        timer -= Time.deltaTime;
                        break;
                    case TransitionTime.UnscaledTime:
                        timer -= Time.unscaledDeltaTime;
                        break;
                }

                if (mat.GetTexture("_MaskTex") != null)
                    mat.SetFloat("_Amount", (timer / duration));
                else
                    cnvs.alpha = timer / duration;


                yield return null;
            }

            obj.GetComponent<CanvasGroup>().interactable = false;
            obj.GetComponent<CanvasGroup>().blocksRaycasts = false;

            if(settings.events.OnFinish != null)
                settings.events.OnFinish.Invoke();

            switch (settings.endAction)
            {
                case TransitionInSettings.EndAction.Destroy:
                    Destroy(obj.transform.parent.gameObject);
                    break;
                case TransitionInSettings.EndAction.Disable:
                    obj.transform.parent.gameObject.SetActive(false);
                    break;
                case TransitionInSettings.EndAction.None:
                    break;
            }

            yield return null;
        }

        IEnumerator TransitionOut(TransitionOutSettings settings)
        {
            float timer = 0;

            Material mat = new Material(_main);
            mat.SetTexture("_MainTex", settings.overlay);
            mat.SetTexture("_MaskTex", settings.mask);
            mat.SetFloat("_Amount", 0);
            mat.SetColor("_Color", settings.color);
            if (settings.invertMask)
                mat.EnableKeyword("INVERT_MASK");
            else
                mat.DisableKeyword("INVERT_MASK");
            mat.SetFloat("_Softness", settings.softness);

            RawImage img = obj.GetComponent<RawImage>();
            if (mat.GetTexture("_MaskTex") != null)
            {
                img.texture = mat.GetTexture("_MainTex");
                img.color = mat.GetColor("_Color");
                img.material = mat;
            }
            else
                DestroyImmediate(img);

            CanvasGroup cnvs = obj.GetComponent<CanvasGroup>();
            if (mat.GetTexture("_MaskTex") == null )
            {
                if(!obj.GetComponent<Image>())
                    obj.AddComponent<Image>().color = settings.color;
                cnvs.alpha = 0;
            }

            yield return new WaitForSeconds(delay);
            
            if(settings.events.OnStart != null)
                settings.events.OnStart.Invoke();

            while (timer < duration)
            {
                if(settings.events.OnUpdate != null)
                    settings.events.OnUpdate.Invoke();
                switch (settings.timeUpdateMethod)
                {
                    case TransitionTime.GameTime:
                        timer += Time.deltaTime;
                        break;
                    case TransitionTime.UnscaledTime:
                        timer += Time.unscaledDeltaTime;
                        break;
                }

                if (mat.GetTexture("_MaskTex") != null)
                    mat.SetFloat("_Amount", (timer / duration));
                else
                    cnvs.alpha = timer / duration;

                yield return null;
            }
            if (settings.events.OnFinish != null)
                settings.events.OnFinish.Invoke();

            if (settings.changeScene)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(settings.sceneName);
                yield return null;
            }

            switch (settings.endAction)
            {
                case TransitionOutSettings.EndAction.Destroy:
                    Destroy(obj.transform.parent.gameObject);
                    break;
                case TransitionOutSettings.EndAction.Disable:
                    obj.transform.parent.gameObject.SetActive(false);
                    break;
                case TransitionOutSettings.EndAction.None:
                    break;
            }
            yield return null;
        }
        #endregion
    }
}