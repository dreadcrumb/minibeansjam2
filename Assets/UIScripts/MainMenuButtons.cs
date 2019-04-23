using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.UIScripts
{
	public class MainMenuButtons : MonoBehaviour
	{
		public Transform levelPanel;

		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update () {
		
		}

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
