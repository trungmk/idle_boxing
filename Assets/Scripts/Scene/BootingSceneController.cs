using Core;
using UnityEngine;
using MEC;
using System.Collections;
using System.Collections.Generic;

public class BootingSceneController : SceneController
{
    public override void OnLoaded()
    {
        UIManager.Instance.Show<SplashPanel>();
        Timing.RunCoroutine(ChangeScene());
    }

    private IEnumerator<float> ChangeScene()
    {
        yield return Timing.WaitForSeconds(2f);

        CoreSceneManager.Instance.ChangeScene(ContextNameGenerated.CONTEXT_GAME);

        UIManager.Instance.Hide<SplashPanel>(isDisable: true, isDestroy: true);
    }
}
