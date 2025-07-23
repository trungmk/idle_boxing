using Core;
using UnityEngine;

public class MenuSceneController : SceneController
{
    public override void OnLoaded()
    {
        UIManager.Instance.Show<MenuPanel>()
            .OnShowCompleted(view =>
            {
                MenuPanel menuPanel = view as MenuPanel;
                menuPanel.OnFightModeSelected += Handle_FightModeSelected;
            });
    }

    private void Handle_FightModeSelected(FightMode fightMode)
    {
        switch (fightMode)
        {
            case FightMode.OneVsOne:
                break;

            case FightMode.OneVsMany:
                break;

            case FightMode.ManyVsMany:
                break;
        }
    }
}
