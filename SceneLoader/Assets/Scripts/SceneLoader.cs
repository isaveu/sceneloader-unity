using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    #region Singleton Instance

    private static SceneLoader _instance = null;
    public static SceneLoader Instance { get { return _instance; } }

    #endregion

    #region Views

    public SceneLoaderView SceneLoaderView;

    #endregion

    public SceneLoaderMode Mode = SceneLoaderMode.Straight;
    public KeyCode PlayerActionKey = KeyCode.X;
    public Color FadeColor = Color.black;
    public float FadeSpeed = 0.5f;

    private AsyncOperation _loadingProgress;
    private bool _allowContinue = false;

    private float _fadeAlpha = 0.0f;
    private Texture2D _fadeTexture;
    private SceneLoaderFadeDirection _fadeDirection = SceneLoaderFadeDirection.Out;

    private bool _isLoadingComplete = false;
    public bool IsLoadingComplete {get { return _isLoadingComplete; } }

    /**************************************************/

    #region Awake

    private void Awake() {
        _instance = this;

        CreateFadeTexture();
    }

    #endregion

    #region LoadSceneWith: Index

    public void LoadScene(int sceneIndex) {
        if (!IsLoadingComplete) {
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }
    }

    private IEnumerator LoadSceneAsync(int sceneIndex) {
        _loadingProgress = SceneManager.LoadSceneAsync(sceneIndex);
        _loadingProgress.allowSceneActivation = false;

        while (!_loadingProgress.isDone) {
            SceneLoaderView.SetProgressValue(_loadingProgress.progress);

            if (_loadingProgress.progress == 0.9f) {
                _isLoadingComplete = true;
                SceneLoaderView.SetProgressValue(1);
                
                switch (Mode) {
                    case SceneLoaderMode.Straight:
                        _allowContinue = true;
                        break;

                    case SceneLoaderMode.WaitForPlayerAction:
                        SceneLoaderView.SetCompleteInfoVisible(true);

                        if (SceneLoaderView.AllowContinue) {
                            _allowContinue = true;
                        }

                        if (Input.GetKeyDown(PlayerActionKey)) {
                            _allowContinue = true;
                        }
                        break;
                }
                
            }

            yield return null;
        }
    }

    #endregion

    #region OnGUI

    private void OnGUI() {
        if (!_allowContinue) {
            return;
        }

        #region GUI

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, _fadeAlpha);
        GUI.depth = -1000;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _fadeTexture);

        #endregion

        #region Set: _fadeAlpha

        if (_fadeDirection == SceneLoaderFadeDirection.In) {
            _fadeAlpha = Mathf.Lerp(_fadeAlpha, -0.1f, FadeSpeed * Time.deltaTime);

        } else if (_fadeDirection == SceneLoaderFadeDirection.Out) {
            _fadeAlpha = Mathf.Lerp(_fadeAlpha, 1.1f, FadeSpeed * Time.deltaTime);

        }

        #endregion

        #region IsComplete Fade

        if ((_fadeAlpha >= 1) && (_fadeDirection == SceneLoaderFadeDirection.Out)) {
            _isLoadingComplete = false;

            _loadingProgress.allowSceneActivation = true;
            _fadeDirection = SceneLoaderFadeDirection.In;

            DontDestroyOnLoad(gameObject);

        } else if ((_fadeAlpha <= 0) && (_fadeDirection == SceneLoaderFadeDirection.In)) {
            Destroy(gameObject);
        }

        #endregion

    }

    private void CreateFadeTexture() {
        _fadeTexture = new Texture2D(1, 1);
        _fadeTexture.SetPixel(0, 0, FadeColor);
        _fadeTexture.Apply();
    }

    #endregion

}