using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheHangingHouse.Animations;

public class FadeTransitionPage : Page
{
    [Header("Self Paramters")]
    [SerializeField] private CanvasGroup m_canvasGroup;

    [Header("Animation Parameters")]
    [SerializeField] private AnimationKey m_animationKey = AnimationKey.EaseInOut * 2;

    private new void OnValidate()
    {
        m_canvasGroup ??= GetComponent<CanvasGroup>();
    }

    public override void OnBeforeShow()
    {
        m_canvasGroup.alpha = 0;
        gameObject.SetActive(true);
    }

    public override void OnAfterShow()
    {

    }

    public override void OnBeforeHide()
    {

    }

    public override void OnAfterHide()
    {
        gameObject.SetActive(false);
    }

    public override IEnumerator ShowingClip(Action callback)
    {
        yield return CoroutineClips.FadeClip(
            m_canvasGroup, 
            true,
            m_animationKey,
            callback
            );
    }

    public override IEnumerator HidingClip(Action callback)
    {
        yield return CoroutineClips.FadeClip(
            m_canvasGroup,
            false,
            m_animationKey,
            callback
            );
    }

    public override IEnumerator NormalClip()
    {
        throw new NotImplementedException();
    }

}
