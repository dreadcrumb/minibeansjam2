using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MENU : MonoBehaviour 
{

	public CanvasGroup mainCG;

	public CanvasGroup settingsCG;

	public bool doSettings = false;
	public bool openSettings = false;
	public bool removeSettings = false;
	public bool openMain = false;

	public Dropdown settingDropDown;

	void Update () 
	{
		if (doSettings) 
		{
			if (mainCG.alpha > 0.01F) {
				
				mainCG.alpha = Mathf.Lerp (mainCG.alpha, 0.0F, 0.1F);
				mainCG.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(mainCG.GetComponent<RectTransform>().anchoredPosition, new Vector2(0F,500F),0.1F);
			} else {
				mainCG.alpha = 0.0F;
				mainCG.blocksRaycasts = false;
				doSettings = false;
				openSettings = true;
			}
		}
		if (openSettings) 
		{
			if (settingsCG.alpha < 0.99F) {
				settingsCG.alpha = Mathf.Lerp (settingsCG.alpha, 1.0F, 0.1F);
			} else {
				settingsCG.alpha = 1.0F;
				settingsCG.blocksRaycasts = true;
				openSettings = false;
			}
		}
		if (removeSettings) 
		{
			if (settingsCG.alpha > 0.01F) {
				settingsCG.alpha = Mathf.Lerp (settingsCG.alpha, 0.0F, 0.1F);
			} else {
				settingsCG.alpha = 0.0F;
				settingsCG.blocksRaycasts = false;
				removeSettings = false;
				openMain = true;
			}
		}
		if (openMain) 
		{
			if (mainCG.alpha < 0.99F) {

				mainCG.alpha = Mathf.Lerp (mainCG.alpha, 1.0F, 0.1F);
				mainCG.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(mainCG.GetComponent<RectTransform>().anchoredPosition, new Vector2(0F,0F),0.1F);
			} else {
				mainCG.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
				mainCG.alpha = 1.0F;
				mainCG.blocksRaycasts = true;
				openMain = false;
			}
		}
	}

	public void GoSettings ()
	{
		if (!doSettings)
			doSettings = true;
	}
	public void GoMain ()
	{
		if (!removeSettings)
			removeSettings = true;
	}

	public void ChangeQuality ()

	{
		QualitySettings.SetQualityLevel (settingDropDown.value, true);
	}

	public void LoadGame (int levelNumber)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene (levelNumber);
	}

	public void QuitGame ()
	{
		Debug.Log ("Quitting");
		Application.Quit ();
	}

}