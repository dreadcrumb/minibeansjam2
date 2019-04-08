using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScript : MonoBehaviour
{
	public Animator anim;
	private readonly int TriggerHash = Animator.StringToHash("TriggerMenuReset");

	// Update is called once per frame
	void Update()
	{

	}

	public void PlayGame()
	{
		LevelSave.Level = 0;
		SceneManager.LoadScene(1);
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void PlayLevel(int scene)
	{
		LevelSave.Level = scene;
		SceneManager.LoadScene(1);
	}

	public void TriggerMenuBack()
	{
		anim.SetTrigger(TriggerHash);
	}

	public void TriggerMenuLevels()
	{
		anim.SetTrigger(TriggerHash);
	}
}
