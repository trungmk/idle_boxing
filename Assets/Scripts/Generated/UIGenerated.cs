using Core;

public struct UIName
{
	public const int IN_GAME_PANEL = -26392309;

	public const int MENU_PANEL = 1033288777;

	public const int SPLASH_SCREEN_TRANSITION = 686678414;

}

public class UIRegistration
{
	[UnityEngine.RuntimeInitializeOnLoadMethod]
	static void AssignUI()
	{
		UIHandler.AddView (-26392309, "InGamePanel", typeof(InGamePanel), "Assets/Prefabs/UI/Panel/InGamePanel.prefab", "Assets/Panel/InGamePanel", UILayer.Panel);

		UIHandler.AddView (1033288777, "MenuPanel", typeof(MenuPanel), "Assets/Prefabs/UI/Panel/MenuPanel.prefab", "Assets/Panel/MenuPanel", UILayer.Panel);

		UIHandler.AddView (686678414, "SplashScreenTransition", typeof(SplashTransition), "Assets/Prefabs/UI/ScreenTransition/SplashScreenTransition.prefab", "Assets/ScreenTransition/SplashScreenTransition", UILayer.ScreenTransition);

	}
}