using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_INGAME : MonoBehaviour 
{

	public bool startInGameMenu = false;
	public bool goToSettings01 = false;
	public bool openSettings = false;
	public bool closeSettings = false;
	public bool openSubMenu = false;
	public bool closeInGameMenu = false;

	public CanvasGroup inGameCG;
	public CanvasGroup mainMenuCG;

	public CanvasGroup subMainCG;
	public CanvasGroup subSettingsCG;

	public Dropdown settingDropDown;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (startInGameMenu) {
			if(mainMenuCG.alpha < 0.99F)
			{
				mainMenuCG.alpha = Mathf.Lerp (mainMenuCG.alpha, 1F, 0.03F);
			} else {
				mainMenuCG.alpha = 1F;
				mainMenuCG.blocksRaycasts = true;
				subMainCG.blocksRaycasts = true;
				subSettingsCG.blocksRaycasts = false;
				inGameCG.blocksRaycasts = false;
				startInGameMenu = false;
			}
		}

		if (goToSettings01) {
		
			if (subMainCG.alpha > 0.01F) {
				subMainCG.alpha = Mathf.Lerp (subMainCG.alpha, 0F, 0.05F);
			} else {
				subMainCG.alpha = 0F;
				subMainCG.blocksRaycasts = false;
				goToSettings01 = false;
				openSettings = true;
			}
		
		}

		if (openSettings) {
		
			if (subSettingsCG.alpha < 0.99F) {
				subSettingsCG.alpha = Mathf.Lerp (subSettingsCG.alpha, 1F, 0.1F);
			} else {
				subSettingsCG.alpha = 1F;
				subSettingsCG.blocksRaycasts = true;
				openSettings = false;
				subMainCG.blocksRaycasts = false;
			}
		}

		if (closeSettings) {
		
			if (subSettingsCG.alpha > 0.01F) {
				subSettingsCG.alpha = Mathf.Lerp (subSettingsCG.alpha, 0F, 0.1F);
			} else {
				subSettingsCG.alpha = 0F;
				subSettingsCG.blocksRaycasts = false;
				openSubMenu = true;
				closeSettings = false;
			}
		
		}
		if (openSubMenu) {
		
			if (subMainCG.alpha < 0.99F) {
				subMainCG.alpha = Mathf.Lerp(subMainCG.alpha, 1F, 0.1F);
			} else {
				subMainCG.blocksRaycasts = true;
				subMainCG.alpha = 1F;
				openSubMenu = false;
			}
		
		}
		if (closeInGameMenu) {
			if(mainMenuCG.alpha > 0.01F)
			{
				mainMenuCG.alpha = Mathf.Lerp (mainMenuCG.alpha, 0F, 0.04F);
			} else {
				mainMenuCG.alpha = 0F;
				mainMenuCG.blocksRaycasts = false;
				subMainCG.blocksRaycasts = false;
				subSettingsCG.blocksRaycasts = false;
				inGameCG.blocksRaycasts = true;
				closeInGameMenu = false;
			}
		}
	}

	public void StartInGameMenu()
	{
		if (!startInGameMenu) {
			startInGameMenu = true;
		}
	}

	public void GoToSettings()
	{
		if(!goToSettings01)
		{
			goToSettings01 = true;
		}
	}
	public void ChangeQuality ()

	{
		QualitySettings.SetQualityLevel (settingDropDown.value, true);
	}
	public void CloseSettings ()
	{
		if(!closeSettings)
			closeSettings = true;
	}
	public void CloseInGameMenu()
	{
		if (!closeInGameMenu)
			closeInGameMenu = true;
	}
	public void LoadMenu (int levelNumber)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (levelNumber);
	}
}
