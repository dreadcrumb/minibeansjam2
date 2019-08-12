using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.UIScripts
{
	public class MainMenuButtons : MonoBehaviour
	{
		public Transform levelPanel;

		public void PlayGame()
		{
			SceneManager.LoadScene(2);
		}

		public void QuitGame()
		{
			Application.Quit();
		}

		public void SelectLevel()
		{
		}
	}
}
