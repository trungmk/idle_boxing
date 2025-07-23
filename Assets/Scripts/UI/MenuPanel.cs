using UnityEngine;
using Core;
using System;

public class MenuPanel : PanelView
{
    public Action<FightMode> OnFightModeSelected;

    protected override void OnPanelShowed(params object[] args)
    {
        
    }

    public void Button_SelectMode(int fightMode)
    {
        if (OnFightModeSelected != null)
        {
            OnFightModeSelected((FightMode) fightMode);
        }
    }    
}
