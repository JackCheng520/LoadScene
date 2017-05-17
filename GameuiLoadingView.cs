using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

/*===================================================
* 类名称: GameuiLoadingView
* 类描述:
* 创建人: Administrator
* 创建时间: 2017/3/18 23:26:47
* 修改人: 
* 修改时间:
* 版本： @version 1.0
=====================================================*/
namespace Game
{
    public class GameuiLoadingView : BaseUI
    {
        [HideInInspector]
        public Image bg;
        [HideInInspector]
        public Slider loadingBar;
        [HideInInspector]
        public Text loadingProgress;
        [HideInInspector]
        public Text loadingText;

        public CanvasGroup canvasGroup;

        private Action callBack;

        private void Awake()
        {
            bg = transform.FindChild("bg").GetComponent<Image>();
            loadingBar = transform.FindChild("loadingBar").GetComponent<Slider>();
            loadingProgress = transform.FindChild("loadingProgress").GetComponent<Text>();
            loadingText = transform.FindChild("loadingText").GetComponent<Text>();

            canvasGroup = gameObject.GetComponent<CanvasGroup>();

            loadingBar.value = 0;
        }

        public void LoadScene(string _name, Action _onCompleteCallBack)
        {
            SetLoadBG(true);
            StartCoroutine(LoadNormalScene(_name, 0, _onCompleteCallBack));
        }

        public void LoadLoadingScene()
        {
            SetLoadBG(false);
            SceneManager.LoadScene("Loading");
        }


        IEnumerator LoadNormalScene(string sceneName, float startPercent, Action _callBack)
        {
            callBack = _callBack;

            loadingText.text = "加载场景中...";

            int startProgress = (int)(startPercent * 100);
            int displayProgress = startProgress;
            int toProgress = startProgress;
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;
            while (op.progress < 0.9f)
            {
                toProgress = startProgress + (int)(op.progress * (1.0f - startPercent) * 100);
                while (displayProgress < toProgress)
                {
                    ++displayProgress;
                    SetProgress(displayProgress);
                    yield return null;
                }
                yield return null;
            }

            toProgress = 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetProgress(displayProgress);
                yield return null;
            }
            op.allowSceneActivation = true;
            
            Check3DRoot();

            canvasGroup.DOFade(0.2f, 0.3f).OnComplete(OnLoadCompleteCallBack);


        }

        private void OnLoadCompleteCallBack()
        {
            if (callBack != null)
            {
                callBack();
                callBack = null;
            }
        }

        //IEnumerator LoadBundleScene(string sceneName)
        //{
        //    string path = BundleManager.GetBundleLoadPath(BundleManager.PathSceneData, sceneName + ".data");
        //    WWW www = new WWW(path);

        //    loadingText.text = "加载资源包中...";
        //    int displayProgress = 0;
        //    int toProgress = 0;
        //    while (!www.isDone)
        //    {
        //        toProgress = (int)(www.progress * m_BundlePercent * 100);
        //        while (displayProgress < toProgress)
        //        {
        //            ++displayProgress;
        //            SetProgress(displayProgress);
        //            yield return null;
        //        }
        //        yield return null;
        //    }

        //    toProgress = (int)(m_BundlePercent * 100);
        //    while (displayProgress < toProgress)
        //    {
        //        ++displayProgress;
        //        SetProgress(displayProgress);
        //        yield return null;
        //    }

        //    yield return www;
        //    if (null != www.assetBundle)
        //    {
        //        m_LastSceneBundle = www.assetBundle;
        //        yield return StartCoroutine(LoadNormalScene(sceneName, m_BundlePercent));
        //    }
        //}

        private void SetProgress(int progress)
        {
            loadingBar.value = progress * 0.01f;
            loadingProgress.text = progress.ToString() + " %";
        }

        private void SetLoadBG(bool _need)
        {
            if (_need)
            {
                Color c = bg.color;
                c.a = 1;
                bg.color = c;
                canvasGroup.alpha = 1;
            }
            else
            {
                Color c = bg.color;
                c.a = 0;
                bg.color = c;
                canvasGroup.alpha = 0;
            }
            loadingBar.gameObject.SetActive(_need);
            loadingProgress.gameObject.SetActive(_need);
            loadingText.gameObject.SetActive(_need);
        }

        private void Check3DRoot()
        {
            GameObject root3D = GameObject.Find("Root3D");
            if (root3D == null)
            {
                root3D = ResourcesController.Instance.LoadPrefab(LoadResourceType.RES_TYPE_UI_3DROOT + "Root3D");
                root3D.name = "Root3D";
            }
            SceneController sceneCtrl = root3D.GetComponent<SceneController>();
            sceneCtrl.InitScene();
        }
    }
}
