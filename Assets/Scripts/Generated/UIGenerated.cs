using Core;

public struct UIName
{
	public const int SPLASH_SCREEN_PANEL = 1849654287;

}

public class UIRegistration
{
	[UnityEngine.RuntimeInitializeOnLoadMethod]
	static void AssignUI()
	{
		UIHandler.AddView (1849654287, "SplashScreenPanel", typeof(SplashTransition), "Assets/Prefabs/UI/Panel/SplashScreenPanel.prefab", "Assets/Panel/SplashScreenPanel", UILayer.Panel);

	}
}