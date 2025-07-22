using Core;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SplashTransition : BaseScreenTransition
{
    [SerializeField]
    private TMP_Text _tapToPlayText;

    [SerializeField]
    private Button _tapToPlayButton;

    public Action OnTapToPlayClicked;

    private Tween _flashTween;

    public void OnTapToPlay_Button()
    {
        OnTapToPlayClicked?.Invoke();
    }

    public void StartToEnableButton()
    {
        _tapToPlayText.gameObject.SetActive(true);
        _tapToPlayButton.interactable = true;
        _flashTween = _tapToPlayText.DOFade(0.1f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    protected override void OnTransitionScreenHided(params object[] args)
    {
        _flashTween?.Kill();
    }
}
