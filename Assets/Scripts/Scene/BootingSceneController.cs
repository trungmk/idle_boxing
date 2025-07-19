using Core;
using UnityEngine;
using MEC;
using System.Collections;
using System.Collections.Generic;

public class BootingSceneController : SceneController
{
    public override void OnLoaded()
    {
        ObjectPooling.Instance.Init(null);

        UIManager.Instance.Show<SplashTransition>();
        Timing.RunCoroutine(ChangeScene());
    }

    private IEnumerator<float> ChangeScene()
    {
        yield return Timing.WaitForSeconds(2f);

        CoreSceneManager.Instance.ChangeScene(ContextNameGenerated.CONTEXT_GAME);

        UIManager.Instance.Hide<SplashTransition>(isDisable: true, isDestroy: true);
    }
}
