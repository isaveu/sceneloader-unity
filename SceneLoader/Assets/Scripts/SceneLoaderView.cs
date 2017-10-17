using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SceneLoaderView : MonoBehaviour {

    #region Childs

    public Slider ProgressBar;

    public Text TxtLoadingInfo;
    public Text TxtLoadingProgress;
    public Text TxtLoadingCompleteInfo;

    public Button BtnContinue;

    #endregion

    private bool _allowContinue = false;
    public bool AllowContinue { get { return _allowContinue; } }

    /**************************************************/

    #region OnEnable

    private void OnEnable() {
        _allowContinue = false;
        SetProgressValue(0);
        SetCompleteInfoVisible(false);

        AddEvents();
    }

    #endregion

    #region OnDisable

    private void OnDisable() {
        RemoveEvents();
    }

    #endregion


    #region Set: ProgressValue

    public void SetProgressValue(float value) {
        ProgressBar.value = value;
        TxtLoadingProgress.text = value * 100 + "%";
    }

    #endregion

    #region Set: CompleteInfoVisible

    public void SetCompleteInfoVisible(bool value) {
        TxtLoadingCompleteInfo.gameObject.SetActive(value);
    }

    #endregion


    #region Event: OnBtnContinueClick

    private void OnBtnContinueClick() {
        if (SceneLoader.Instance.IsLoadingComplete) {
            _allowContinue = true;
        }
    }

    #endregion


    #region Events: Add, Remove

    private void AddEvents() {
        BtnContinue.onClick.AddListener(new UnityAction(OnBtnContinueClick));
    }

    private void RemoveEvents() {
        BtnContinue.onClick.RemoveListener(OnBtnContinueClick);
    }

    #endregion

}